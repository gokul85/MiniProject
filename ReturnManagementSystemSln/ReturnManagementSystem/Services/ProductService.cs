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
        public async Task<ProductReturnDTO> AddProduct(ProductDTO productDTO)
        {
            var prodcheck = await _productRepository.FindAll(p=>p.Name == productDTO.Name && p.ProductStatus == productDTO.Status);
            if (prodcheck != null)
            {
                throw new InvalidDataException("Product Already Found Exception");
            }
            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Stock = 0,
                ProductStatus = productDTO.Status,
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
                try
                {
                    ProductItem productitem = new ProductItem
                    {
                        SerialNumber = pi.SerialNumber,
                        Status = "Available",
                        ProductId = aproduct.ProductId
                    };
                    var addedpi = await _productitemRepository.Add(productitem);
                    if (addedpi != null)
                    {
                        aproduct.Stock += 1;
                        await _productRepository.Update(aproduct);
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return MapProductReturnDTO(product);
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var products =  await _productRepository.GetAllWithIncludes(p=>p.Policies, p=>p.ProductItems);
            if (products == null || products.Count() == 0)
                throw new ObjectsNotFoundException("Products not found");
            return products;
        }

        public async Task<ProductReturnDTO> GetProductById(int id)
        {
            var product = await _productRepository.FindAllWithIncludes(p=>p.ProductId == id, p => p.Policies, p => p.ProductItems);
            if (product != null) 
                return MapProductReturnDTO(product.First());
            throw new ObjectNotFoundException("Product not found");
        }

        private ProductReturnDTO MapProductReturnDTO(Product product)
        {
            ProductReturnDTO pr = new ProductReturnDTO()
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ProductStatus = product.ProductStatus,
                Policies = product.Policies.ToList()
            };
            return pr;
        }

        public async Task<ProductReturnDTO> UpdateProduct(int productId, ProductDTO productDTO)
        {
            var product = await _productRepository.Get(productId);
            if (product == null) throw new ObjectNotFoundException("Product not found");

            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Price = productDTO.Price;

            var updateproduct = await _productRepository.Update(product);
            return MapProductReturnDTO(updateproduct);
        }
    }
}
