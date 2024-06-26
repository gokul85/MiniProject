using System.ComponentModel.DataAnnotations;

namespace ReturnManagementSystem.Models.DTOs.OrderDTOs
{
    public class OrderProductDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0.")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }
}
