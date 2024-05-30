using ReturnManagementSystem.Models.DTOs.ProductDTOs;
using ReturnManagementSystem.Models;

namespace ReturnManagementSystem.Interfaces
{
    public interface IProductItemService
    {
        Task<AddProductItemsResponse> AddProductItem(List<AddProductItemDTO> addpiDTO);
        Task<ProductItem> UpdateProductItemStatus(string serialNumber, string status);
        Task<ProductItem> UpdateProductItemRefurbished(int productId, string serialNumber);
    }

}
