using System.ComponentModel.DataAnnotations;

namespace ReturnManagementSystem.Models.DTOs.OrderDTOs
{
    public class OrderDTO
    {
        public int UserId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Order must contain at least one product.")]
        public List<OrderProductDTO> OrderProducts { get; set; }
    }
}
