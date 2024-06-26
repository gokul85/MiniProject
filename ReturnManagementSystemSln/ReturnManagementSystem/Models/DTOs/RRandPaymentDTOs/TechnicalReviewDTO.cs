using System.ComponentModel.DataAnnotations;

namespace ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs
{
    public class TechnicalReviewDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "requestId must be greater than 0.")]
        public int requestId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "process must be between 1 and 50 characters.")]
        public string process { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "feedback must be between 1 and 500 characters.")]
        public string feedback { get; set; }
    }
}
