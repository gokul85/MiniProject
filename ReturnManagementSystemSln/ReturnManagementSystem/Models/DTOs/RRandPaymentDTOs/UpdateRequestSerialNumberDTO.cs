using System.ComponentModel.DataAnnotations;

namespace ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs
{
    public class UpdateRequestSerialNumberDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "RquestId must be greater than 0.")]
        public int RequestId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "SerialNumber must be between 1 and 100 characters.")]
        public string SerialNumber { get; set; }
    }
}
