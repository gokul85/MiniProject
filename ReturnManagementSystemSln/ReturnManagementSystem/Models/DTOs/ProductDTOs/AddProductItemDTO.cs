using System.ComponentModel.DataAnnotations;

namespace ReturnManagementSystem.Models.DTOs.ProductDTOs
{
    public class AddProductItemDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "SerialNumber must be between 1 and 100 characters.")]
        public string SerialNumber { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0.")]
        public int ProductId { get; set; }
    }
}
