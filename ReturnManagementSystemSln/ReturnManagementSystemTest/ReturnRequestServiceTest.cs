using Microsoft.EntityFrameworkCore;
using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Repositories;
using ReturnManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnManagementSystemTest
{
    public class ReturnRequestServiceTest
    {
        private IReturnRequestService _returnRequestService;
        private IRepository<int, ReturnRequest> _returnRequestRepository;
        private IRepository<int, Order> _orderRepository;
        private IRepository<int, OrderProduct> _orderProductRepository;
        private IRepository<int, Product> _productRepository;
        private IRepository<int, ProductItem> _productItemRepository;
        private IRepository<int, Policy> _policyRepository;
        private IRepository<int, Transaction> _transactionRepository;
        private IProductItemService _productItemService;
        private ReturnManagementSystemContext _context;
        private IPaymentService _paymentService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ReturnManagementSystemContext>()
                .UseInMemoryDatabase(databaseName: "ReturnRequestTestDB_" + Guid.NewGuid().ToString())
                .Options;
            _context = new ReturnManagementSystemContext(options);

            _returnRequestRepository = new ReturnRequestRepository(_context);
            _orderRepository = new OrderRepository(_context);
            _orderProductRepository = new OrderProductRepository(_context);
            _productRepository = new ProductRepository(_context);
            _productItemRepository = new ProductItemRepository(_context);
            _policyRepository = new PolicyRepository(_context);
            _transactionRepository = new TransactionRepository(_context);
            _productItemService = new ProductItemService(_productItemRepository, _productRepository);
            _paymentService = new PaymentService(_transactionRepository);

            _returnRequestService = new ReturnRequestService(
                _returnRequestRepository,
                _orderRepository,
                _orderProductRepository,
                _productRepository,
                _productItemRepository,
                _policyRepository,
                _productItemService,
                _paymentService
            );
        }

        [Test]
        public async Task OpenReturnRequestTestSuccess()
        {
            var order = new Order
            {
                OrderId = 1,
                UserId = 1,
                OrderDate = DateTime.Now.AddDays(-1),
                OrderStatus = "Delivered"
            };
            await _orderRepository.Add(order);

            var orderProduct = new OrderProduct
            {
                OrderId = 1,
                ProductId = 1,
                SerialNumber = "SN1"
            };
            await _orderProductRepository.Add(orderProduct);

            var policy = new Policy
            {
                ProductId = 1,
                PolicyType = "Return",
                Duration = 30
            };
            await _policyRepository.Add(policy);

            var returnRequestDTO = new ReturnRequestDTO
            {
                UserId = 1,
                OrderId = 1,
                ProductId = 1,
                ReturnPolicy = "Return",
                Reason = "Defective"
            };

            var result = await _returnRequestService.OpenReturnRequest(returnRequestDTO);

            Assert.IsNotNull(result);
            Assert.That(result.Status, Is.EqualTo("Pending"));
        }

        [Test]
        public void OpenReturnRequestTestInvalidOrder()
        {
            var returnRequestDTO = new ReturnRequestDTO
            {
                UserId = 1,
                OrderId = 999,
                ProductId = 1,
                ReturnPolicy = "Return",
                Reason = "Defective"
            };

            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _returnRequestService.OpenReturnRequest(returnRequestDTO));
        }

        [Test]
        public async Task OpenReturnRequestTestProductNotFoundInOrder()
        {
            var order = new Order
            {
                OrderId = 1,
                UserId = 1,
                OrderDate = DateTime.Now.AddDays(-1),
                OrderStatus = "Delivered"
            };
            await _orderRepository.Add(order);

            var returnRequestDTO = new ReturnRequestDTO
            {
                UserId = 1,
                OrderId = 1,
                ProductId = 999,
                ReturnPolicy = "Return",
                Reason = "Defective"
            };

            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _returnRequestService.OpenReturnRequest(returnRequestDTO));
        }

        [Test]
        public async Task OpenReturnRequestTestPolicyNotFound()
        {
            var order = new Order
            {
                OrderId = 1,
                UserId = 1,
                OrderDate = DateTime.Now.AddDays(-1),
                OrderStatus = "Delivered"
            };
            await _orderRepository.Add(order);

            var orderProduct = new OrderProduct
            {
                OrderId = 1,
                ProductId = 1,
                SerialNumber = "SN1"
            };
            await _orderProductRepository.Add(orderProduct);

            var returnRequestDTO = new ReturnRequestDTO
            {
                UserId = 1,
                OrderId = 1,
                ProductId = 1,
                ReturnPolicy = "InvalidPolicy",
                Reason = "Defective"
            };

            Assert.ThrowsAsync<InvalidReturnRequest>(async () => await _returnRequestService.OpenReturnRequest(returnRequestDTO));
        }

        [Test]
        public async Task OpenReturnRequestTestPolicyDurationExceeded()
        {
            var order = new Order
            {
                OrderId = 1,
                UserId = 1,
                OrderDate = DateTime.Now.AddDays(-31),
                OrderStatus = "Delivered"
            };
            await _orderRepository.Add(order);

            var orderProduct = new OrderProduct
            {
                OrderId = 1,
                ProductId = 1,
                SerialNumber = "SN1"
            };
            await _orderProductRepository.Add(orderProduct);

            var policy = new Policy
            {
                ProductId = 1,
                PolicyType = "Return",
                Duration = 30
            };
            await _policyRepository.Add(policy);

            var returnRequestDTO = new ReturnRequestDTO
            {
                UserId = 1,
                OrderId = 1,
                ProductId = 1,
                ReturnPolicy = "Return",
                Reason = "Defective"
            };

            Assert.ThrowsAsync<InvalidReturnRequest>(async () => await _returnRequestService.OpenReturnRequest(returnRequestDTO));
        }

        [Test]
        public async Task CloseReturnRequestTestSuccess()
        {
            var returnRequest = new ReturnRequest
            {
                RequestId = 1,
                UserId = 1,
                OrderId = 1,
                ProductId = 1,
                ReturnPolicy = "Return",
                Reason = "Defective",
                Status = "Pending",
                RequestDate = DateTime.Now
            };
            await _returnRequestRepository.Add(returnRequest);

            var result = await _returnRequestService.CloseReturnRequest(1, new CloseRequestDTO() { Feedback = "Feedback", RequestId = 1 });

            Assert.IsNotNull(result);
            Assert.That(result.Status, Is.EqualTo("Closed"));
            Assert.That(result.Feedback, Is.EqualTo("Feedback"));
        }

        [Test]
        public void CloseReturnRequestTestNotFound()
        {
            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _returnRequestService.CloseReturnRequest(1, new CloseRequestDTO() { Feedback = "Feedback", RequestId = 999 }));
        }

        [Test]
        public async Task UpdateUserSerialNumberTestSuccess()
        {
            var returnRequest = new ReturnRequest
            {
                RequestId = 1,
                UserId = 1,
                OrderId = 1,
                ProductId = 1,
                ReturnPolicy = "Return",
                Reason = "Defective",
                Status = "Pending",
                RequestDate = DateTime.Now
            };
            await _returnRequestRepository.Add(returnRequest);

            var orderProduct = new OrderProduct
            {
                OrderId = 1,
                ProductId = 1,
                SerialNumber = "SN1"
            };
            await _orderProductRepository.Add(orderProduct);

            var result = await _returnRequestService.UpdateUserSerialNumber(new UpdateRequestSerialNumberDTO(){ RquestId =1, SerialNumber ="SN1" });

            Assert.IsNotNull(result);
            Assert.That(result.SerialNumber, Is.EqualTo("SN1"));
            Assert.That(result.Status, Is.EqualTo("Processing"));
        }

        [Test]
        public void UpdateUserSerialNumberTestReturnRequestNotFound()
        {
            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _returnRequestService.UpdateUserSerialNumber(new UpdateRequestSerialNumberDTO() { RquestId = 999, SerialNumber = "SN1" }));
        }

        [Test]
        public async Task UpdateUserSerialNumberTestInvalidSerialNumber()
        {
            var returnRequest = new ReturnRequest
            {
                RequestId = 1,
                UserId = 1,
                OrderId = 1,
                ProductId = 1,
                ReturnPolicy = "Return",
                Reason = "Defective",
                Status = "Pending",
                RequestDate = DateTime.Now
            };
            await _returnRequestRepository.Add(returnRequest);

            Assert.ThrowsAsync<InvalidSerialNumber>(async () => await _returnRequestService.UpdateUserSerialNumber(new UpdateRequestSerialNumberDTO() { RquestId = 1, SerialNumber = "SN9999" }));
        }


        


        [Test]
        public void TechnicalReview_ReturnRequestNotFound_ThrowsObjectNotFoundException()
        {
            TechnicalReviewDTO technicalReviewDTO = new TechnicalReviewDTO() { requestId = 999, feedback = "feedback", process = "Return Good" };

            Assert.ThrowsAsync<ObjectNotFoundException>(async () =>
                await _returnRequestService.TechnicalReview(technicalReviewDTO));
        }

        [Test]
        public void TechnicalReview_InvalidProcess_ThrowsInvalidDataException()
        {
            // Arrange
            var returnRequest = new ReturnRequest
            {
                RequestId = 1,
                UserId = 1,
                OrderId = 1,
                ProductId = 1,
                ReturnPolicy = "Return",
                Reason = "Defective",
                Status = "Pending",
                RequestDate = DateTime.Now,
                SerialNumber = "SN1"
            };
            _returnRequestRepository.Add(returnRequest).Wait();

            TechnicalReviewDTO technicalReviewDTO = new TechnicalReviewDTO() { requestId = 1, feedback = "feedback", process = "Return" };

            // Act & Assert
            Assert.ThrowsAsync<InvalidDataException>(async () =>
                await _returnRequestService.TechnicalReview(technicalReviewDTO));
        }

        [Test]
        public async Task TechnicalReview_ReplaceRepaired_OutOfStock_ThrowsInvalidDataException()
        {
            var product = new Product
            {
                ProductId = 1,
                Name = "Test Product",
                Stock = 0,
                ProductStatus = "Fresh",
                Description = "Product for Testing",
                Price = 1000
            };
            await _productRepository.Add(product);
            var returnRequest = new ReturnRequest
            {
                RequestId = 1,
                UserId = 1,
                OrderId = 1,
                ProductId = 1,
                ReturnPolicy = "Return",
                Reason = "Defective",
                Status = "Pending",
                RequestDate = DateTime.Now,
                SerialNumber = "SN1"
            };
            await _returnRequestRepository.Add(returnRequest);

            var productItem = new ProductItem
            {
                ProductItemId = 1,
                ProductId = 1,
                SerialNumber = "SN1",
                Status = "Ordered"
            };
            await _productItemRepository.Add(productItem);

            var orderProduct = new OrderProduct
            {
                OrderId = 1,
                ProductId = 1,
                SerialNumber = "SN1"
            };
            await _orderProductRepository.Add(orderProduct);

            TechnicalReviewDTO technicalReviewDTO = new TechnicalReviewDTO() { requestId = 1, feedback = "feedback", process = "Replace Repaired" };

            Assert.ThrowsAsync<InvalidDataException>(async () =>
                await _returnRequestService.TechnicalReview(technicalReviewDTO));
        }

        [TestCase(1, "Return Good", "Feedback")]
        [TestCase(1, "Return Bad", "Feedback")]
        [TestCase(1, "Replace Repaired", "Feedback")]
        [TestCase(2, "Replace Repaired", "Feedback")]
        [TestCase(1, "Replace Bad", "Feedback")]
        [TestCase(1, "Repaired", "Feedback")]
        public async Task TechnicalReview_SuccessTests(int requestId, string process, string feedback)
        {
            var product = new Product
            {
                ProductId = 1,
                Name = "Test Product",
                Stock = 0,
                ProductStatus = "Fresh",
                Description = "Product for Testing",
                Price = 1000
            };
            await _productRepository.Add(product);
            if(requestId == 2)
            {
                var product2 = new Product
                {
                    ProductId = 2,
                    Name = "Test Product",
                    Stock = 0,
                    ProductStatus = "Refurbished",
                    Description = "Product for Testing",
                    Price = 750
                };
                await _productRepository.Add(product2);
            }
            var order = new Order
            {
                OrderId = 1,
                UserId = 1,
                OrderDate = DateTime.Now.AddDays(-10),
                TotalAmount = 100,
                OrderStatus = "Delivered",
                OrderProducts = new List<OrderProduct>()
            };
            await _orderRepository.Add(order);

            var returnRequest = new ReturnRequest
            {
                RequestId = 1,
                UserId = 1,
                OrderId = 1,
                ProductId = 1,
                ReturnPolicy = "Return",
                Reason = "Defective",
                Status = "Pending",
                RequestDate = DateTime.Now,
                SerialNumber = "SN1"
            };
            await _returnRequestRepository.Add(returnRequest);

            var productItem = new ProductItem
            {
                ProductItemId = 1,
                ProductId = 1,
                SerialNumber = "SN1",
                Status = "Available"
            };
            await _productItemRepository.Add(productItem);

            var orderProduct = new OrderProduct
            {
                OrderId = 1,
                ProductId = 1,
                SerialNumber = "SN1",
                Price = 1000
            };
            await _orderProductRepository.Add(orderProduct);

            order.OrderProducts.Add(orderProduct);
            await _orderRepository.Update(order);

            if (process == "Replace Repaired" || process == "Replace Bad")
            {
                var replacementProductItem = new ProductItem
                {
                    ProductItemId = 2,
                    ProductId = 1,
                    SerialNumber = "SN2",
                    Status = "Available"
                };
                await _productItemRepository.Add(replacementProductItem);
            }

            TechnicalReviewDTO technicalReviewDTO = new TechnicalReviewDTO() { requestId = 1, feedback = feedback, process = process };

            var result = await _returnRequestService.TechnicalReview(technicalReviewDTO);
             
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Process, Is.EqualTo(process));
            Assert.That(result.Feedback, Is.EqualTo(feedback));
        }


        [Test]
        public async Task GetAllRequestTestNoRequestsFound()
        {
            var ex = Assert.ThrowsAsync<ObjectsNotFoundException>(async () => await _returnRequestService.GetAllReturnRequests());
            Assert.AreEqual("Return Requests Not Found", ex.Message);
        }
        [Test]
        public async Task GetAllUserRequestTestNoRequestsFound()
        {
            var ex = Assert.ThrowsAsync<ObjectsNotFoundException>(async () => await _returnRequestService.GetAllUserReturnRequests(2));
            Assert.AreEqual("Return Requests Not Found", ex.Message);
        }

        [Test]
        public async Task GetAllUserReturnRequestSuccess()
        {
            var order = new Order
            {
                OrderId = 1,
                UserId = 1,
                OrderDate = DateTime.Now.AddDays(-1),
                OrderStatus = "Delivered"
            };
            await _orderRepository.Add(order);

            var orderProduct = new OrderProduct
            {
                OrderId = 1,
                ProductId = 1,
                SerialNumber = "SN1"
            };
            await _orderProductRepository.Add(orderProduct);

            var policy = new Policy
            {
                ProductId = 1,
                PolicyType = "Return",
                Duration = 30
            };
            await _policyRepository.Add(policy);

            var returnRequestDTO = new ReturnRequestDTO
            {
                UserId = 1,
                OrderId = 1,
                ProductId = 1,
                ReturnPolicy = "Return",
                Reason = "Defective"
            };

            await _returnRequestService.OpenReturnRequest(returnRequestDTO);

            var result = await _returnRequestService.GetAllUserReturnRequests(1);

            Assert.IsNotNull(result);
            Assert.That(result.Any());
        }

        [Test]
        public async Task GetAllRquestTestSuccess()
        {
            var order = new Order
            {
                OrderId = 1,
                UserId = 1,
                OrderDate = DateTime.Now.AddDays(-1),
                OrderStatus = "Delivered"
            };
            await _orderRepository.Add(order);

            var orderProduct = new OrderProduct
            {
                OrderId = 1,
                ProductId = 1,
                SerialNumber = "SN1"
            };
            await _orderProductRepository.Add(orderProduct);

            var policy = new Policy
            {
                ProductId = 1,
                PolicyType = "Return",
                Duration = 30
            };
            await _policyRepository.Add(policy);

            var returnRequestDTO = new ReturnRequestDTO
            {
                UserId = 1,
                OrderId = 1,
                ProductId = 1,
                ReturnPolicy = "Return",
                Reason = "Defective"
            };

            await _returnRequestService.OpenReturnRequest(returnRequestDTO);

            var result = await _returnRequestService.GetAllReturnRequests();

            Assert.IsNotNull(result);
            Assert.That(result.Any());
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetRequestTestSuccess()
        {
            var order = new Order
            {
                OrderId = 1,
                UserId = 1,
                OrderDate = DateTime.Now.AddDays(-1),
                OrderStatus = "Delivered"
            };
            await _orderRepository.Add(order);

            var orderProduct = new OrderProduct
            {
                OrderId = 1,
                ProductId = 1,
                SerialNumber = "SN1"
            };
            await _orderProductRepository.Add(orderProduct);

            var policy = new Policy
            {
                ProductId = 1,
                PolicyType = "Return",
                Duration = 30
            };
            await _policyRepository.Add(policy);

            var returnRequestDTO = new ReturnRequestDTO
            {
                UserId = 1,
                OrderId = 1,
                ProductId = 1,
                ReturnPolicy = "Return",
                Reason = "Defective"
            };

            await _returnRequestService.OpenReturnRequest(returnRequestDTO);

            var result = await _returnRequestService.GetReturnRequest(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(returnRequestDTO.UserId, result.UserId);
        }

        [Test]
        public void GetRequestTestNoRequestFound()
        {
            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _returnRequestService.GetReturnRequest(999));
        }
    }
}
