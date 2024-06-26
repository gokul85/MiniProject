using System.ComponentModel.DataAnnotations;

namespace ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs
{
    public class CloseRequestDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "RequestId must be greater than 0.")]
        public int RequestId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Feedback must be between 1 and 500 characters.")]
        public string Feedback { get; set; }
    }
}
