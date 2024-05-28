namespace ReturnManagementSystem.Models.DTOs.ProductDTOs
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public ICollection<PolicyDTO> Policies { get; set; }
        public ICollection<ProductItemDTO> ProductItems { get; set; }
    }
}
