using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;
using ReturnManagementSystem.Repositories;

namespace ReturnManagementSystem.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<int, Payment> _paymentRepository;
        private readonly IRepository<int, RefundTransaction> _refundTransactionRepository;

        public PaymentService(IRepository<int, Payment> paymentRepository, IRepository<int, RefundTransaction> refundTransactionRepository)
        {
            _paymentRepository = paymentRepository;
            _refundTransactionRepository = refundTransactionRepository;
        }

        public async Task<IEnumerable<Payment>> GetAllPayment()
        {
            var payments = await _paymentRepository.GetAll();
            if (payments != null)
            {
                return payments;
            }
            throw new ObjectsNotFoundException("No Payments Found");
        }

        public async Task<IEnumerable<RefundTransaction>> GetAllPaymentRefund()
        {
            var refundTransactions = await _refundTransactionRepository.GetAll();
            if (refundTransactions != null)
            {
                return refundTransactions;
            }
            throw new ObjectsNotFoundException("No Refund Payments Found");
        }

        public async Task<Payment> GetPayment(int paymentId)
        {
            var payment =  await _paymentRepository.Get(paymentId);
            if(payment == null)
            {
                throw new ObjectNotFoundException("Payment Not Found");
            }
            return payment;
        }

        public async Task<RefundTransaction> GetPaymentRefund(int paymentId)
        {
            var refundpayment = await _refundTransactionRepository.Get(paymentId);
            if(refundpayment == null)
            {
                throw new ObjectNotFoundException("Refund Payments Not Found");
            }
            return refundpayment;
        }

        public async Task<Payment> ProcessPayment(PaymentDTO paymentDTO)
        {
            var payment = new Payment
            {
                OrderId = paymentDTO.OrderId,
                PaymentDate = DateTime.UtcNow,
                TransactionId = paymentDTO.TransactionId,
                Amount = paymentDTO.Amount
            };

            return await _paymentRepository.Add(payment);
        }

        public async Task<RefundTransaction> ProcessPaymentRefund(PaymentDTO paymentDTO)
        {
            var refundtras = new RefundTransaction() { 
                RequestId = paymentDTO.OrderId, 
                TransactionAmount = paymentDTO.Amount, 
                TransactionId = paymentDTO.TransactionId, 
                TransactionDate = DateTime.Now 
            };
            return await _refundTransactionRepository.Add(refundtras);
        }
    }
}
