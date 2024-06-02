using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;
using ReturnManagementSystem.Repositories;

namespace ReturnManagementSystem.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<int, Transaction> _transactionRepository;

        public PaymentService(IRepository<int, Transaction> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactions()
        {
            var transactions = await _transactionRepository.GetAll();
            if (transactions != null)
            {
                return transactions;
            }
            throw new ObjectsNotFoundException("No Transactions Found");
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactions(string transactiontype)
        {
            var transactions = await _transactionRepository.FindAll(t => t.TransactionType == transactiontype);
            if (transactions != null)
            {
                return transactions;
            }
            throw new ObjectsNotFoundException("No Transactions Found");
        }

        public async Task<Transaction> GetTransaction(int transactionId)
        {
            var transaction =  await _transactionRepository.Get(transactionId);
            if(transaction == null)
            {
                throw new ObjectNotFoundException("Transaction Not Found");
            }
            return transaction;
        }

        public async Task<Transaction> ProcessPayment(TransactionDTO transactionDTO)
        {
            Transaction transaction;
            if(transactionDTO.TransactionType == "Payment")
            {
                transaction = new Transaction()
                {
                    OrderId = transactionDTO.OrderId,
                    TransactionDate = transactionDTO.PaymentDate,
                    PaymentGatewayTransactionId = transactionDTO.TransactionId,
                    TransactionAmount = transactionDTO.Amount,
                    TransactionType = transactionDTO.TransactionType,
                };
            }
            else
            {
                transaction = new Transaction()
                {
                    RequestId = transactionDTO.RequestId,
                    TransactionDate = transactionDTO.PaymentDate,
                    PaymentGatewayTransactionId = transactionDTO.TransactionId,
                    TransactionAmount = transactionDTO.Amount,
                    TransactionType = transactionDTO.TransactionType,
                };
            }
            
            return await _transactionRepository.Add(transaction);
        }
    }
}
