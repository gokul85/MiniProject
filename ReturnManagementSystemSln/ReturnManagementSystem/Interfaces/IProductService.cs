using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.ProductDTOs;

namespace ReturnManagementSystem.Interfaces
{
    public interface IProductService
    {
        public Task<ProductReturnDTO> AddProduct(ProductDTO productDTO);
        Task<IEnumerable<Product>> GetAllProducts();
        Task<ProductReturnDTO> GetProductById(int id);
        Task<ProductReturnDTO> UpdateProduct(int productId, ProductDTO productDto);
    }
}
