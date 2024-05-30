using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.ProductDTOs;
using ReturnManagementSystem.Repositories;
using ReturnManagementSystem.Services;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using ReturnManagementSystem.Exceptions;

namespace ReturnManagementSystemTest
{
    public class ProductServiceTest
    {

        IProductService _productservice;
        IRepository<int, Product> _productRepo;
        IRepository<int, ProductItem> _productitemRepo;
        IRepository<int, Policy> _policyRepo;
        ReturnManagementSystemContext context;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ReturnManagementSystemContext>().UseInMemoryDatabase("DummyDB").Options;
            context = new ReturnManagementSystemContext(options);
            _productRepo = new ProductRepository(context);
            _productitemRepo = new ProductItemRepository(context);
            _policyRepo = new PolicyRepository(context);
            _productservice = new ProductService(_productRepo, _policyRepo, _productitemRepo);
        }

        [Test]
        public async Task AddProductTest()
        {
            ProductDTO productdto = new ProductDTO()
            {
                Name = "Boat TWS 777",
                Price = 1099,
                Description = "TWS 777",
                Policies = new List<PolicyDTO>
                {
                    new PolicyDTO()
                    {
                        PolicyType = "Replacement",
                        Duration = 7
                    },
                    new PolicyDTO()
                    {
                        PolicyType = "Warrenty",
                        Duration = 365
                    }
                },
                ProductItems = new List<ProductItemDTO>
                {
                    new ProductItemDTO()
                    {
                        SerialNumber = "BTWS0001"
                    },
                    new ProductItemDTO()
                    {
                        SerialNumber = "BTWS0002"
                    },
                    new ProductItemDTO()
                    {
                        SerialNumber = "BTWS0003"
                    },
                    new ProductItemDTO()
                    {
                        SerialNumber = "BTWS0004"
                    },
                    new ProductItemDTO()
                    {
                        SerialNumber = "BTWS0005"
                    },
                    new ProductItemDTO()
                    {
                        SerialNumber = "BTWS0006"
                    },
                    new ProductItemDTO()
                    {
                        SerialNumber = "BTWS0007"
                    },
                }
            };

            var product = await _productservice.AddProduct(productdto);

            Assert.IsNotNull(product);
        }

        [Test]
        public async Task GetAllProducts()
        {
            var products = await _productservice.GetAllProducts();

            Assert.That(products, Is.TypeOf<List<Product>>());
        }

        [Test]
        public async Task UpdateProduct()
        {
            ProductDTO productdto = new ProductDTO()
            {
                Name = "Boat TWS 777",
                Price = 1098,
                Description = "TWS 777",
            };
            
            var product = await _productservice.UpdateProduct(1,productdto);

            Assert.That(product.Price, Is.EqualTo(1098));
        }
        [Test]
        public async Task UpdateProductFail()
        {
            ProductDTO productdto = new ProductDTO()
            {
                Name = "Boat TWS 777",
                Price = 1098,
                Description = "TWS 777",
            };

            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _productservice.UpdateProduct(999, productdto));
        }
    }
}