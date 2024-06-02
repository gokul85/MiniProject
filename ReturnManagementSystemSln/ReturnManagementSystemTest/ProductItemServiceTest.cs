using Microsoft.EntityFrameworkCore;
using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models.DTOs.ProductDTOs;
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
    public class ProductItemServiceTest
    {
        private IProductItemService _productItemService;
        private IRepository<int, ProductItem> _productItemRepository;
        private IRepository<int, Product> _productRepository;
        private ReturnManagementSystemContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ReturnManagementSystemContext>()
                .UseInMemoryDatabase(databaseName: "ProductItemTestDB_" + System.Guid.NewGuid().ToString())
                .Options;
            _context = new ReturnManagementSystemContext(options);

            _productItemRepository = new ProductItemRepository(_context);
            _productRepository = new ProductRepository(_context);

            _productItemService = new ProductItemService(_productItemRepository, _productRepository);
        }

        [Test]
        public async Task AddProductItemTestSuccess()
        {
            var product = new Product
            {
                ProductId = 1,
                Name = "Test Product",
                Stock = 0
            };
            await _productRepository.Add(product);

            var addProductItems = new List<AddProductItemDTO>
            {
                new AddProductItemDTO
                {
                    ProductId = 1,
                    SerialNumber = "SN1"
                },
                new AddProductItemDTO
                {
                    ProductId = 1,
                    SerialNumber = "SN2"
                }
            };

            var result = await _productItemService.AddProductItem(addProductItems);

            Assert.IsNotNull(result);
            Assert.That(result.AddedProductItems.Count, Is.EqualTo(2));
            Assert.That((await _productRepository.Get(1)).Stock, Is.EqualTo(2));
        }

        [Test]
        public async Task AddProductItemTestProductNotFound()
        {
            var addProductItems = new List<AddProductItemDTO>
            {
                new AddProductItemDTO
                {
                    ProductId = 999,
                    SerialNumber = "SN1"
                },
                new AddProductItemDTO
                {
                    ProductId = 999,
                    SerialNumber = "SN2"
                }
            };

            var result = await _productItemService.AddProductItem(addProductItems);

            Assert.That(result.AddedProductItems.Count, Is.EqualTo(0));
            Assert.Contains(999, result.ProductIdsNotFound);
        }

        [Test]
        public async Task AddProductItemTestSerialNumberExists()
        {
            var product = new Product
            {
                ProductId = 1,
                Name = "Test Product",
                Stock = 0
            };
            await _productRepository.Add(product);

            var existingProductItem = new ProductItem
            {
                ProductId = 1,
                SerialNumber = "SN1",
                Status = "Available"
            };
            await _productItemRepository.Add(existingProductItem);

            var addProductItems = new List<AddProductItemDTO>
            {
                new AddProductItemDTO
                {
                    ProductId = 1,
                    SerialNumber = "SN1"
                },
                new AddProductItemDTO
                {
                    ProductId = 1,
                    SerialNumber = "SN1"
                },
                new AddProductItemDTO
                {
                    ProductId = 1,
                    SerialNumber = "SN2"
                }
            };

            var result = await _productItemService.AddProductItem(addProductItems);

            Assert.That(result.AddedProductItems.Count, Is.EqualTo(1));
            Assert.Contains("SN1", result.SerialNumbersExists);
        }

        [Test]
        public async Task GetAllProductItemsTestSuccess()
        {
            var productItem1 = new ProductItem { ProductId = 1, SerialNumber = "SN1", Status = "Available" };
            var productItem2 = new ProductItem { ProductId = 2, SerialNumber = "SN2", Status = "Ordered" };
            await _productItemRepository.Add(productItem1);
            await _productItemRepository.Add(productItem2);

            var result = await _productItemService.GetAllProductItems();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAllProductItemsTestNoItemsFound()
        {
            Assert.ThrowsAsync<ObjectsNotFoundException>(async () => await _productItemService.GetAllProductItems());
        }

        [Test]
        public async Task GetAllProductItemsByStatusTestSuccess()
        {
            var productItem1 = new ProductItem { ProductId = 1, SerialNumber = "SN1", Status = "Available" };
            var productItem2 = new ProductItem { ProductId = 2, SerialNumber = "SN2", Status = "Ordered" };
            await _productItemRepository.Add(productItem1);
            await _productItemRepository.Add(productItem2);

            var result = await _productItemService.GetAllProductItems("Available");

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().SerialNumber, Is.EqualTo("SN1"));
        }

        [Test]
        public void GetAllProductItemsByStatusTestNoItemsFound()
        {
            Assert.ThrowsAsync<ObjectsNotFoundException>(async () => await _productItemService.GetAllProductItems("NonExistentStatus"));
        }

        [Test]
        public async Task GetAllProductItemsByProductIdTestSuccess()
        {
            var productItem1 = new ProductItem { ProductId = 1, SerialNumber = "SN1", Status = "Available" };
            var productItem2 = new ProductItem { ProductId = 2, SerialNumber = "SN2", Status = "Ordered" };
            await _productItemRepository.Add(productItem1);
            await _productItemRepository.Add(productItem2);

            var result = await _productItemService.GetAllProductItemsByProductId(1);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().SerialNumber, Is.EqualTo("SN1"));
        }

        [Test]
        public void GetAllProductItemsByProductIdTestNoItemsFound()
        {
            Assert.ThrowsAsync<ObjectsNotFoundException>(async () => await _productItemService.GetAllProductItemsByProductId(999));
        }

        [Test]
        public async Task UpdateProductItemRefurbishedTestSuccess()
        {
            var product = new Product
            {
                ProductId = 1,
                Name = "Test Product",
                Stock = 0,
                ProductStatus = "Fresh"
            };
            await _productRepository.Add(product);

            var product2 = new Product
            {
                ProductId = 2,
                Name = "Test Product",
                Stock = 0,
                ProductStatus = "Refurbished"
            };
            await _productRepository.Add(product2);

            var productItem = new ProductItem
            {
                ProductId = 1,
                SerialNumber = "SN1",
                Status = "Available"
            };
            await _productItemRepository.Add(productItem);

            var result = await _productItemService.UpdateProductItemRefurbished(2, "SN1");

            Assert.IsNotNull(result);
            Assert.That(result.Status, Is.EqualTo("Refurbished"));
            Assert.That((await _productRepository.Get(2)).Stock, Is.EqualTo(1));
        }

        [TestCase(1, "SN999")]
        [TestCase(999, "SN1")]
        public async Task UpdateProductItemRefurbishedTestProductItemNotFound(int id, string sn)
        {
            var productItem = new ProductItem
            {
                ProductId = 1,
                SerialNumber = "SN1",
                Status = "Available"
            };
            await _productItemRepository.Add(productItem);
            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _productItemService.UpdateProductItemRefurbished(id, sn));
        }

        [TestCase("SN1", "Available")]
        [TestCase("SN2", "Available")]
        [TestCase("SN1", "Replaced")]
        public async Task UpdateProductItemStatusTestSuccess(string sn, string status)
        {
            var product = new Product
            {
                ProductId = 1,
                Name = "Test Product",
                Stock = 0
            };
            await _productRepository.Add(product);

            var productItem = new ProductItem
            {
                ProductId = 1,
                SerialNumber = "SN1",
                Status = "Available"
            };
            await _productItemRepository.Add(productItem);

            var productItem2 = new ProductItem
            {
                ProductId = 1,
                SerialNumber = "SN2",
                Status = "Ordered"
            };
            await _productItemRepository.Add(productItem2);

            var result = await _productItemService.UpdateProductItemStatus(new UpdateProductItemStatus() { SerialNumber = sn, Status = status});

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo(status));
        }

        [Test]
        public void UpdateProductItemStatusTestProductItemNotFound()
        {
            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _productItemService.UpdateProductItemStatus(new UpdateProductItemStatus() { SerialNumber = "SN999", Status = "Available" }));
        }
    }
}
