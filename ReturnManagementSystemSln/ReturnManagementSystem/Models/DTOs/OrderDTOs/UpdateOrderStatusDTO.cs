using System.ComponentModel.DataAnnotations;

namespace ReturnManagementSystem.Models.DTOs.OrderDTOs
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "OrderId must be greater than 0.")]
        public int OrderId { get; set; }

        [Required]
        public string OrderStatus { get; set; }
    }
}
