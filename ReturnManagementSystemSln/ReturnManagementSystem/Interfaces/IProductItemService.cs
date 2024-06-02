using ReturnManagementSystem.Models.DTOs.ProductDTOs;
using ReturnManagementSystem.Models;

namespace ReturnManagementSystem.Interfaces
{
    public interface IProductItemService
    {
        Task<AddProductItemsResponse> AddProductItem(List<AddProductItemDTO> addpiDTO);
        Task<ProductItem> UpdateProductItemStatus(UpdateProductItemStatus upisDTO);
        Task<ProductItem> UpdateProductItemRefurbished(int productId, string serialNumber);

        Task<IEnumerable<ProductItem>> GetAllProductItems();
        Task<IEnumerable<ProductItem>> GetAllProductItems(string status);
        Task<IEnumerable<ProductItem>> GetAllProductItemsByProductId(int productid);
    }

}
