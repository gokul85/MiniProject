using System.ComponentModel.DataAnnotations;

namespace ReturnManagementSystem.Models.DTOs.ProductDTOs
{
    public class ProductDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 500 characters.")]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        public string Status { get; set; }

        public ICollection<PolicyDTO> Policies { get; set; }

        public ICollection<ProductItemDTO> ProductItems { get; set; }
    }
}
