using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;

namespace ReturnManagementSystem.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<Transaction>> GetAllTransactions();
        Task<IEnumerable<Transaction>> GetAllTransactions(string transactiontype);
        Task<Transaction> GetTransaction(int transactionId);
        Task<Transaction> ProcessPayment(TransactionDTO transactionDTO);

    }

}
