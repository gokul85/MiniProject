using System.ComponentModel.DataAnnotations;

namespace ReturnManagementSystem.Models.DTOs.ProductDTOs
{
    public class UpdateProductItemStatus
    {
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "SerialNumber must be between 1 and 100 characters.")]
        public string SerialNumber { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Status must be between 1 and 50 characters.")]
        public string Status { get; set; }
    }
}
