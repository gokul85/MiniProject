using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.ProductDTOs;

namespace ReturnManagementSystem.Interfaces
{
    public interface IProductService
    {
        public Task<Product> AddProduct(ProductDTO productDTO);
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product> UpdateProduct(int productId, ProductDTO productDto);
    }
}
