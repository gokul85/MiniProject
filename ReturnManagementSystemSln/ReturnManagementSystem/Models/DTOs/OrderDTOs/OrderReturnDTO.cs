namespace ReturnManagementSystem.Models.DTOs.OrderDTOs
{
    public class OrderReturnDTO
    {
        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? OrderStatus { get; set; }
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
