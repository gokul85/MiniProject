using Microsoft.EntityFrameworkCore;
using Moq;
using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.OrderDTOs;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;
using ReturnManagementSystem.Repositories;
using ReturnManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnManagementSystemTest
{
    public class OrderServiceTest
    {
        private IOrderService _orderService;
        private IRepository<int, Order> _orderRepository;
        private IRepository<int, OrderProduct> _orderProductRepository;
        private IRepository<int, ProductItem> _productItemRepository;
        private IRepository<int, Product> _productRepository;
        private Mock<IPaymentService> _mockPaymentService;
        private ReturnManagementSystemContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ReturnManagementSystemContext>()
                .UseInMemoryDatabase("OrderTestDB").Options;
            _context = new ReturnManagementSystemContext(options);

            _orderRepository = new OrderRepository(_context);
            _orderProductRepository = new OrderProductRepository(_context);
            _productItemRepository = new ProductItemRepository(_context);
            _productRepository = new ProductRepository(_context);
            _mockPaymentService = new Mock<IPaymentService>();

            _orderService = new OrderService(_orderRepository, _productItemRepository, _productRepository, _orderProductRepository, _mockPaymentService.Object);
        }

        [Test]
        public async Task CreateOrderTestSuccess()
        {
            var product = new Product
            {
                Name = "Test Product",
                Price = 1000,
                Stock = 10
            };
            product = await _productRepository.Add(product);

            for (int i = 0; i < 10; i++)
            {
                await _productItemRepository.Add(new ProductItem
                {
                    ProductId = product.ProductId,
                    SerialNumber = $"SN{i}",
                    Status = "Available"
                });
            }

            var orderDTO = new OrderDTO
            {
                UserId = 1,
                TotalAmount = 200,
                OrderProducts = new List<OrderProductDTO>
                {
                    new OrderProductDTO
                    {
                        ProductId = product.ProductId,
                        Quantity = 2
                    }
                }
            };


            var result = await _orderService.CreateOrder(orderDTO);

            Assert.IsNotNull(result);
            Assert.That(result.UserId, Is.EqualTo(1));
            Assert.That(result.OrderStatus, Is.EqualTo("Pending"));
        }

        [Test]
        public void CreateOrderTestProductNotFound()
        {
            var orderDTO = new OrderDTO
            {
                UserId = 1,
                TotalAmount = 200,
                OrderProducts = new List<OrderProductDTO>
                {
                    new OrderProductDTO
                    {
                        ProductId = 1,
                        Quantity = 2
                    }
                }
            };

            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _orderService.CreateOrder(orderDTO));
        }

        [Test]
        public async Task CreateOrderTestProductOutOfStock()
        {
            var product = new Product
            {
                ProductId = 1,
                Name = "Test Product",
                Price = 100,
                Stock = 1
            };
            await _productRepository.Add(product);

            await _productItemRepository.Add(new ProductItem
            {
                ProductId = product.ProductId,
                SerialNumber = "SN1",
                Status = "Available"
            });

            var orderDTO = new OrderDTO
            {
                UserId = 1,
                TotalAmount = 200,
                OrderProducts = new List<OrderProductDTO>
                {
                    new OrderProductDTO
                    {
                        ProductId = 1,
                        Quantity = 2
                    }
                }
            };

            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _orderService.CreateOrder(orderDTO));
        }

        [Test]
        public async Task GetAllUserOrdersTestNoOrdersFound()
        {
            Assert.ThrowsAsync<ObjectsNotFoundException>(async () => await _orderService.GetAllUserOrders(2));
        }

        [Test]
        public async Task GetAllUserOrdersTestSuccess()
        {

            var order = new Order
            {
                UserId = 1,
                OrderDate = DateTime.Now,
                TotalAmount = 100,
                OrderStatus = "Pending"
            };
            await _orderRepository.Add(order);

            var result = await _orderService.GetAllUserOrders(1);

            Assert.IsNotNull(result);
            Assert.That(result.Any());
        }

        [Test]
        public async Task UpdateOrderStatusTestSuccess()
        {
            var order = new Order
            {
                UserId = 1,
                OrderDate = DateTime.Now,
                TotalAmount = 100,
                OrderStatus = "Pending"
            };
            await _orderRepository.Add(order);

            var result = await _orderService.UpdateOrderStatus(order.OrderId, "Delivered");

            Assert.IsNotNull(result);
            Assert.That(result.OrderStatus, Is.EqualTo("Delivered"));
        }

        [Test]
        public void UpdateOrderStatusTestOrderNotFound()
        {
            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _orderService.UpdateOrderStatus(999, "Delivered"));
        }
    }
}
