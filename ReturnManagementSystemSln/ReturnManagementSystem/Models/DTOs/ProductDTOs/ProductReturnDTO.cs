namespace ReturnManagementSystem.Models.DTOs.ProductDTOs
{
    public class ProductReturnDTO
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public string? ProductStatus { get; set; }
        public List<Policy> Policies { get; set; } = new List<Policy>();
    }
}