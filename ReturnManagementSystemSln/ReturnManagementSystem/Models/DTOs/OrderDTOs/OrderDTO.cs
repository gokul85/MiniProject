namespace ReturnManagementSystem.Models.DTOs.OrderDTOs
{
    public class OrderDTO
    {
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderProductDTO> OrderProducts { get; set; }
    }
}
