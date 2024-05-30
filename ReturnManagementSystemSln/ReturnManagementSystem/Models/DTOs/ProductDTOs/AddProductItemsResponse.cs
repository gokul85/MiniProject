namespace ReturnManagementSystem.Models.DTOs.ProductDTOs
{
    public class AddProductItemsResponse
    {
        public List<ProductItem> AddedProductItems { get; set; } = new List<ProductItem>();
        public List<string> SerialNumbersExists { get; set; } = new List<string>(); 
        public List<int> ProductIdsNotFound { get; set; } = new List<int>();
    }
}
