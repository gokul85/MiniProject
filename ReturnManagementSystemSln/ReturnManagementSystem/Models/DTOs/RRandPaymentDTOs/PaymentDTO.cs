namespace ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs
{
    public class PaymentDTO
    {
        public int OrderId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
    }
}
