using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.ProductDTOs;

namespace ReturnManagementSystem.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<int,Product> _productRepository;
        private readonly IRepository<int,Policy> _policyRepository;
        private readonly IRepository<int,ProductItem> _productitemRepository;

        public ProductService(IRepository<int,Product> productRepository, IRepository<int, Policy> policyRepository,IRepository<int, ProductItem> productitemRepository)
        {                                                                
            _productRepository = productRepository;
            _policyRepository = policyRepository;
            _productitemRepository = productitemRepository;
        }
        public async Task<Product> AddProduct(ProductDTO productDTO)
        {
            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Stock = 0,
                ProductStatus = "Fresh",
            };
            var aproduct = await _productRepository.Add(product);
            foreach (var p in productDTO.Policies)
            {
                Policy policy = new Policy
                {
                    PolicyType = p.PolicyType,
                    Duration = p.Duration,
                    ProductId = aproduct.ProductId
                };
                var ap = await _policyRepository.Add(policy);
            }
            foreach(var pi in productDTO.ProductItems)
            {
                ProductItem productitem = new ProductItem
                {
                    SerialNumber = pi.SerialNumber,
                    Status = "Available",
                    ProductId = aproduct.ProductId
                };
                var addedpi = await _productitemRepository.Add(productitem);
                if(addedpi != null)
                {
                    aproduct.Stock += 1;
                    await _productRepository.Update(aproduct);
                }
            }
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _productRepository.GetAll();
        }

        public async Task<Product> UpdateProduct(int productId, ProductDTO productDTO)
        {
            var product = await _productRepository.Get(productId);
            if (product == null) throw new ProductNotFoundException("Product not found");

            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Price = productDTO.Price;

            return await _productRepository.Update(product);
        }
    }
}
