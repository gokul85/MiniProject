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

        IProductService _productService;
        IRepository<int, Product> _productRepo;
        IRepository<int, ProductItem> _productitemRepo;
        IRepository<int, Policy> _policyRepo;
        ReturnManagementSystemContext context;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ReturnManagementSystemContext>().UseInMemoryDatabase("ProductServiceTestDB_"+Guid.NewGuid()).Options;
            context = new ReturnManagementSystemContext(options);
            _productRepo = new ProductRepository(context);
            _productitemRepo = new ProductItemRepository(context);
            _policyRepo = new PolicyRepository(context);
            _productService = new ProductService(_productRepo, _policyRepo, _productitemRepo);
        }

        [Test]
        public async Task AddProductTest_Success()
        {
            var productDto = new ProductDTO()
            {
                Name = "Boat TWS 777",
                Price = 1099,
                Description = "TWS 777",
                Policies = new List<PolicyDTO>
                {
                    new PolicyDTO { PolicyType = "Replacement", Duration = 7 },
                    new PolicyDTO { PolicyType = "Warranty", Duration = 365 }
                },
                ProductItems = new List<ProductItemDTO>
                {
                    new ProductItemDTO { SerialNumber = "BTWS0001" },
                    new ProductItemDTO { SerialNumber = "BTWS0002" }
                }
            };

            var product = await _productService.AddProduct(productDto);

            Assert.IsNotNull(product);
            Assert.AreEqual("Boat TWS 777", product.Name);
            Assert.AreEqual(2, product.Stock);
            Assert.That(product.Policies.Count, Is.EqualTo(2));
            Assert.That(product.Policies.First().PolicyId, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllProductsTest_Success()
        {
            var product1 = new Product { Name = "Product 1", Price = 100, Description = "Description 1", Stock = 10, ProductStatus = "Fresh"};
            var product2 = new Product { Name = "Product 2", Price = 200, Description = "Description 2", Stock = 20, ProductStatus = "Fresh" };
            await _productRepo.Add(product1);
            await _productRepo.Add(product2);

            var products = await _productService.GetAllProducts();

            Assert.IsNotNull(products);
            Assert.That(products.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAllProductsTest_NoProductsFound()
        {
            Assert.ThrowsAsync<ObjectsNotFoundException>(async () => await _productService.GetAllProducts());
        }

        [Test]
        public async Task GetProductByIdTest_Success()
        {
            var product = new Product { Name = "Product 1", Price = 100, Description = "Description 1", Stock = 10, ProductStatus = "Fresh" };
            var addedProduct = await _productRepo.Add(product);

            var retrievedProduct = await _productService.GetProductById(addedProduct.ProductId);

            Assert.IsNotNull(retrievedProduct);
            Assert.AreEqual("Product 1", retrievedProduct.Name);
        }

        [Test]
        public void GetProductByIdTest_ProductNotFound()
        {
            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _productService.GetProductById(999));
        }

        [Test]
        public async Task UpdateProductTest_Success()
        {
            var product = new Product { Name = "Product 1", Price = 100, Description = "Description 1", Stock = 10, ProductStatus = "Fresh" };
            var addedProduct = await _productRepo.Add(product);

            var productDto = new ProductDTO { Name = "Updated Product", Price = 150, Description = "Updated Description" };
            var updatedProduct = await _productService.UpdateProduct(addedProduct.ProductId, productDto);

            Assert.IsNotNull(updatedProduct);
            Assert.AreEqual("Updated Product", updatedProduct.Name);
            Assert.AreEqual(150, updatedProduct.Price);
        }

        [Test]
        public void UpdateProductTest_ProductNotFound()
        {
            var productDto = new ProductDTO { Name = "Non-existent Product", Price = 150, Description = "Description" };
            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _productService.UpdateProduct(999, productDto));
        }
    }
}