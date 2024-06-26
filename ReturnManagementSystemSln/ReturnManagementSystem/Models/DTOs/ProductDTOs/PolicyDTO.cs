using System.ComponentModel.DataAnnotations;

namespace ReturnManagementSystem.Models.DTOs.ProductDTOs
{
    public class PolicyDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "PolicyType must be between 1 and 100 characters.")]
        public string PolicyType { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0.")]
        public int Duration { get; set; }
    }
}
