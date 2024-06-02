using Microsoft.EntityFrameworkCore;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnManagementSystemTest
{
    public class RepositoriesTest
    {
        private ReturnManagementSystemContext _context;
        private IRepository<int, Order> _orderRepository;
        private IRepository<int, OrderProduct> _orderProductRepository;
        private IRepository<int, Transaction> _transactionRepository;
        private IRepository<int, Policy> _policyRepository;
        private IRepository<int, Product> _productRepository;
        private IRepository<int, ProductItem> _productItemRepository;
        private IRepository<int, ReturnRequest> _returnRequestRepository;
        private IRepository<int, User> _userRepository;
        private IRepository<int, UserDetail> _userDetailRepository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ReturnManagementSystemContext>()
                .UseInMemoryDatabase(databaseName: "ReturnManagementTestDb")
                .Options;
            _context = new ReturnManagementSystemContext(options);
            _orderRepository = new OrderRepository(_context);
            _orderProductRepository = new OrderProductRepository(_context);
            _transactionRepository = new TransactionRepository(_context);
            _policyRepository = new PolicyRepository(_context);
            _productRepository = new ProductRepository(_context);
            _productItemRepository = new ProductItemRepository(_context);
            _returnRequestRepository = new ReturnRequestRepository(_context);
            _userRepository = new UserRepository(_context);
            _userDetailRepository = new UserDetailRepository(_context);
        }

        //[TestCase(new ProductItem() { ProductId = 1, SerialNumber = "SN1", Status = ""})]
        //[TestCase(new Policy() { ProductId = 1)]
        //[Test]
        //public async Task Add_Success_Test()
        //{
        //    User user = new User() { Name = "User", Address = "User Address", Email = "user@gmail.com", Phone = "9876543210", Role = "User", Id = 1};
        //    UserDetail detail = new UserDetail() { UserId = 1, PasswordHashKey = new byte[] { }, Password = new byte[] { }, Status = "Active", Username = "User" };
        //    Product product = new Product() { ProductId = 1, Name = "Test Product", Description = "Test Description", Price = 1000, Stock = 10 };
        //    Policy policy = new Policy() { PolicyId = 1, ProductId = 1, PolicyType = "Replacement", Duration = 7};
        //    ProductItem productitem = new ProductItem() { ProductId = 1, SerialNumber = "SN1", ProductItemId = 1, Status = "Available" }; 
        //    Order order = new Order() { OrderId = 1, OrderDate = new DateTime(), OrderStatus = "Delivered", UserId = 1, TotalAmount = 1200};
        //    OrderProduct orderproduct = new OrderProduct() { ProductId = 1, OrderId = 1, OrderProductId = 1, Price = 1000, SerialNumber = "SN1" };
        //}

        [Test]
        public async Task DeleteTestPass()
        {
            var orderProduct = new OrderProduct { OrderProductId = 1, OrderId = 1, SerialNumber = "ABC123", Price = 100.0M };
            await _orderProductRepository.Add(orderProduct);

            var result = await _orderProductRepository.Delete(orderProduct);

            var deletedEntity = await _orderProductRepository.Get(orderProduct.OrderProductId);
            Assert.IsNull(deletedEntity);
            Assert.That(result, Is.EqualTo(orderProduct));
        }

        [Test]
        public void DeleteExceptionEntityNotFound()
        {
            var orderProduct = new OrderProduct { OrderProductId = 1, OrderId = 1, SerialNumber = "ABC123", Price = 100.0M };

            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await _orderProductRepository.Delete(orderProduct));
        }
    }
}
