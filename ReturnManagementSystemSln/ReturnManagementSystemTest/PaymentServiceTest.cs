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
        private IRepository<int, Payment> _paymentRepository;
        private IRepository<int, RefundTransaction> _refundTransactionRepository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ReturnManagementSystemContext>()
            .UseInMemoryDatabase(databaseName: ("ReturnManagementTestDb_" + Guid.NewGuid()))
                .Options;

            _context = new ReturnManagementSystemContext(options);
            _paymentRepository = new PaymentRepository(_context);
            _refundTransactionRepository = new RefundTransactionRepository(_context);
            _paymentService = new PaymentService(_paymentRepository, _refundTransactionRepository);
        }


        [Test]
        public async Task GetAllPayment_ReturnsPayments_WhenPaymentsExist()
        {
            var payment1 = new Payment { PaymentId = 1, OrderId = 1, PaymentDate = DateTime.UtcNow, TransactionId = "TXN123", Amount = 100 };
            var payment2 = new Payment { PaymentId = 2, OrderId = 2, PaymentDate = DateTime.UtcNow, TransactionId = "TXN124", Amount = 200 };
            await _paymentRepository.Add(payment1);
            await _paymentRepository.Add(payment2);

            var result = await _paymentService.GetAllPayment();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().TransactionId, Is.EqualTo("TXN123"));
        }

        [Test]
        public void GetAllPayment_ThrowsObjectsNotFoundException_WhenNoPaymentsExist()
        {
            Assert.ThrowsAsync<ObjectsNotFoundException>(async () => await _paymentService.GetAllPayment());
        }

        [Test]
        public async Task GetAllPaymentRefund_ReturnsRefundTransactions_WhenRefundTransactionsExist()
        {
            var refundTransaction1 = new RefundTransaction { RefundTransactionId = 1, RequestId = 1, TransactionDate = DateTime.UtcNow, TransactionId = "RTXN123", TransactionAmount = 100 };
            var refundTransaction2 = new RefundTransaction { RefundTransactionId = 2, RequestId = 2, TransactionDate = DateTime.UtcNow, TransactionId = "RTXN124", TransactionAmount = 200 };
            await _refundTransactionRepository.Add(refundTransaction1);
            await _refundTransactionRepository.Add(refundTransaction2);

            var result = await _paymentService.GetAllPaymentRefund();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().TransactionId, Is.EqualTo("RTXN123"));
        }

        [Test]
        public void GetAllPaymentRefund_ThrowsObjectsNotFoundException_WhenNoRefundTransactionsExist()
        {
            Assert.ThrowsAsync<ObjectsNotFoundException>(async () => await _paymentService.GetAllPaymentRefund());
        }

        [Test]
        public async Task GetPayment_ReturnsPayment_WhenPaymentExists()
        {
            var payment = new Payment { PaymentId = 1, OrderId = 1, PaymentDate = DateTime.UtcNow, TransactionId = "TXN123", Amount = 100 };
            await _paymentRepository.Add(payment);

            var result = await _paymentService.GetPayment(1);

            Assert.IsNotNull(result);
            Assert.That(result.TransactionId, Is.EqualTo("TXN123"));
        }

        [Test]
        public void GetPayment_ThrowsObjectNotFoundException_WhenPaymentDoesNotExist()
        {
            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _paymentService.GetPayment(999));
        }

        [Test]
        public async Task GetPaymentRefund_ReturnsRefundTransaction_WhenRefundTransactionExists()
        {
            var refundTransaction = new RefundTransaction { RefundTransactionId = 1, RequestId = 1, TransactionDate = DateTime.UtcNow, TransactionId = "RTXN123", TransactionAmount = 100 };
            await _refundTransactionRepository.Add(refundTransaction);

            var result = await _paymentService.GetPaymentRefund(1);

            Assert.IsNotNull(result);
            Assert.That(result.TransactionId, Is.EqualTo("RTXN123"));
        }

        [Test]
        public void GetPaymentRefund_ThrowsObjectNotFoundException_WhenRefundTransactionDoesNotExist()
        {
            Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _paymentService.GetPaymentRefund(999));
        }

        [Test]
        public async Task ProcessPayment_AddsPaymentSuccessfully()
        {
            var paymentDTO = new PaymentDTO { OrderId = 1, TransactionId = "TXN123", Amount = 100 };

            var result = await _paymentService.ProcessPayment(paymentDTO);

            Assert.IsNotNull(result);
            Assert.That(result.TransactionId, Is.EqualTo("TXN123"));
            var payments = await _paymentRepository.GetAll();
            Assert.That(payments.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task ProcessPaymentRefund_AddsRefundTransactionSuccessfully()
        {
            var paymentDTO = new PaymentDTO { OrderId = 1, TransactionId = "RTXN123", Amount = 100 };

            var result = await _paymentService.ProcessPaymentRefund(paymentDTO);

            Assert.IsNotNull(result);
            Assert.That(result.TransactionId, Is.EqualTo("RTXN123"));
            var refunds = await _refundTransactionRepository.GetAll();
            Assert.That(refunds.Count(), Is.EqualTo(1));
        }
    }
}
