using Microsoft.AspNetCore.Mvc;
using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;
using ReturnManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace ReturnManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all transactions.
        /// </summary>
        /// <returns>All transactions.</returns>
        [HttpGet("GetAllTransactions")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<Transaction>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetAllTransactions()
        {
            try
            {
                var transactions = await _paymentService.GetAllTransactions();
                return Ok(transactions);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogError(ex, "No transactions found");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving transactions");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves all transactions of a specific type.
        /// </summary>
        /// <param name="transactionType">The type of the transactions to retrieve.</param>
        /// <returns>All transactions of the specified type.</returns>
        [HttpGet("GetTransactionsByType")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<Transaction>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetAllTransactionsByType(string transactionType)
        {
            try
            {
                var transactions = await _paymentService.GetAllTransactions(transactionType);
                return Ok(transactions);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogError(ex, $"No transactions of type {transactionType} found");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving transactions by type");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a transaction by ID.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to retrieve.</param>
        /// <returns>The transaction with the specified ID.</returns>
        [HttpGet("GetTransaction")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Transaction), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Transaction>> GetTransaction(int transactionId)
        {
            try
            {
                var transaction = await _paymentService.GetTransaction(transactionId);
                return Ok(transaction);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogError(ex, "Transaction not found");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the transaction");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
    }
}
