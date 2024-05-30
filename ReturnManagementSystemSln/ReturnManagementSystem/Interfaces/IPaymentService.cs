using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;

namespace ReturnManagementSystem.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> ProcessPayment(PaymentDTO paymentDTO);
        Task<Payment> GetPayment(int paymentId);
        Task<RefundTransaction> ProcessPaymentRefund(PaymentDTO paymentDTO);
        Task<RefundTransaction> GetPaymentRefund(int paymentId);
        Task<IEnumerable<Payment>> GetAllPayment();
        Task<IEnumerable<RefundTransaction>> GetAllPaymentRefund();

    }

}
