namespace ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs
{
    public class ReturnRequestDTO
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ReturnPolicy { get; set; }
        public string Reason { get; set; }
    }
}
