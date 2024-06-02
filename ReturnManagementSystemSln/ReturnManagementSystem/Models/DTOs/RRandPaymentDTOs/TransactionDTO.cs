namespace ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs
{
    public class TransactionDTO
    {
        public int OrderId { get; set; }
        public int RequestId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
    }
}
