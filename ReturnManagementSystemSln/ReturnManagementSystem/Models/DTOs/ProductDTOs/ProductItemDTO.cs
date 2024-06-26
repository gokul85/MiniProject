using System.ComponentModel.DataAnnotations;

namespace ReturnManagementSystem.Models.DTOs.ProductDTOs
{
    public class ProductItemDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "SerialNumber must be between 1 and 100 characters.")]
        public string SerialNumber { get; set; }
    }
}
