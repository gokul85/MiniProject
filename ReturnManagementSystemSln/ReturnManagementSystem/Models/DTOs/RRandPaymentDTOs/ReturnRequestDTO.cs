using System.ComponentModel.DataAnnotations;

namespace ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs
{
    public class ReturnRequestDTO
    {
        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "OrderId must be greater than 0.")]
        public int OrderId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0.")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "ReturnPolicy must be between 1 and 100 characters.")]
        public string ReturnPolicy { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Reason must be between 1 and 500 characters.")]
        public string Reason { get; set; }
    }
}
