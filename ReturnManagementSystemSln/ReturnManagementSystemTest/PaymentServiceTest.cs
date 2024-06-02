using Microsoft.EntityFrameworkCore;
using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReturnManagementSystem.Repositories;

namespace ReturnManagementSystemTest
{
    public class PaymentServiceTest
    {
        private PaymentService _paymentService;
        private ReturnManagementSystemContext _context;
        private IRepository<int, Transaction> _transactionRepository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ReturnManagementSystemContext>()
                .UseInMemoryDatabase(databaseName: ("ReturnManagementTestDb_" + Guid.NewGuid()))
                .Options;

            _context = new ReturnManagementSystemContext(options);
            _transactionRepository = new TransactionRepository(_context);
            _paymentService = new PaymentService(_transactionRepository);
        }

        [Test]
        public async Task GetAllTransactions_ReturnsTransactions_WhenTransactionsExist()
        {
            var transaction1 = new Transaction { TransactionId = 1, OrderId = 1, TransactionDate = DateTime.UtcNow, PaymentGatewayTransactionId = "TXN123", TransactionAmount = 100, TransactionType = "Payment" };
            var transaction2 = new Transaction { TransactionId = 2, OrderId = 2, TransactionDate = DateTime.UtcNow, PaymentGatewayTransactionId = "TXN124", TransactionAmount = 200, TransactionType = "Payment" };
            await _transactionRepository.Add(transaction1);
            await _transactionRepository.Add(transaction2);

            var result = await _paymentService.GetAllTransactions();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().PaymentGatewayTransactionId, Is.EqualTo("TXN123"));
        }

        [Test]
        public void GetAllTransactions_ThrowsObjectsNotFoundException_WhenNoTransactionsExist()
        {
            Assert.ThrowsAsync<ObjectsNotFoundException>(async () => await _paymentService.GetAllTransactions());
        }

        [Test]
        public async Task GetAllTransactionsByType_ReturnsTransactions_WhenTransactionsExist()
        {
            var transaction1 = new Transaction { TransactionId = 1, OrderId = 1, TransactionDate = DateTime.UtcNow, PaymentGatewayTransactionId = "TXN123", TransactionAmount = 100, TransactionType = "Payment" };
            var transaction2 = new Transaction { TransactionId = 2, OrderId = 2, TransactionDate = DateTime.UtcNow, PaymentGatewayTransactionId = "TXN124", TransactionAmount = 200, TransactionType = "Refund" };
            await _transactionRepository.Add(transaction1);
            await _transactionRepository.Add(transaction2);

            var result = await _paymentService.GetAllTransactions("Payment");

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().PaymentGatewayTransactionId, Is.EqualTo("TXN123"));
        }

        [Test]
        public void GetAllTransactionsByType_ThrowsObjectsNotFoundException_WhenNoTransactionsExist()
        {
            Assert.ThrowsAsync<ObjectsNotFoundException>(async () => await _paymentService.GetAllTransactions("NonExistentType"));
        }

        [Test]
        public async Task GetTransaction_ReturnsTransaction_WhenTransactionExists()
        {
            var transaction = new Transaction { TransactionId = 1, OrderId = 1, TransactionDate = DateTime.UtcNow, PaymentGatewayTransactionId = "TXN123", TransactionAmount = 100, TransactionType = "Payment" };
            await _transactionRepository.Add(transaction);

            var result = await _paymentService.GetTransaction(1);

            Assert.IsNotNull(result);
            Assert.That(result.PaymentGatewayTransactionId, Is.EqualTo("TXN123"));
        }

        [Test]
        public void GetTransaction_ThrowsObjectNotFoundException_WhenTransactionDoesNotExist()
        {
            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _paymentService.GetTransaction(999));
        }

        [Test]
        public async Task ProcessPayment_AddsTransactionSuccessfully()
        {
            var transactionDTO = new TransactionDTO { OrderId = 1, TransactionId = "TXN123", Amount = 100, TransactionType = "Payment", PaymentDate = DateTime.UtcNow };

            var result = await _paymentService.ProcessPayment(transactionDTO);

            Assert.IsNotNull(result);
            Assert.That(result.PaymentGatewayTransactionId, Is.EqualTo("TXN123"));
            var transactions = await _transactionRepository.GetAll();
            Assert.That(transactions.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task ProcessRefund_AddsRefundTransactionSuccessfully()
        {
            var transactionDTO = new TransactionDTO { RequestId = 1, TransactionId = "RTXN123", Amount = 100, TransactionType = "Refund", PaymentDate = DateTime.UtcNow };

            var result = await _paymentService.ProcessPayment(transactionDTO);

            Assert.IsNotNull(result);
            Assert.That(result.PaymentGatewayTransactionId, Is.EqualTo("RTXN123"));
            var transactions = await _transactionRepository.GetAll();
            Assert.That(transactions.Count(), Is.EqualTo(1));
        }
    }
}
