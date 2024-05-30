using Microsoft.AspNetCore.Mvc;
using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;
using ReturnManagementSystem.Models;

namespace ReturnManagementSystem.Controllers
{
    /// <summary>
    /// Controller for managing payments.
    /// </summary>
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
        /// Retrieves all payments.
        /// </summary>
        /// <returns>All payments.</returns>
        [HttpGet("AllPayments")]
        [ProducesResponseType(typeof(IEnumerable<Payment>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayments()
        {
            try
            {
                var payments = await _paymentService.GetAllPayment();
                return Ok(payments);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogError(ex, "No payments found");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving payments");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves all refund payments.
        /// </summary>
        /// <returns>All refund payments.</returns>
        [HttpGet("AllRefundPayments")]
        [ProducesResponseType(typeof(IEnumerable<RefundTransaction>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RefundTransaction>>> GetAllRefundPayments()
        {
            try
            {
                var refundTransactions = await _paymentService.GetAllPaymentRefund();
                return Ok(refundTransactions);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogError(ex, "No refund payments found");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving refund payments");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a payment by ID.
        /// </summary>
        /// <param name="paymentId">The ID of the payment to retrieve.</param>
        /// <returns>The payment with the specified ID.</returns>
        [HttpGet("Payment/{paymentId}")]
        [ProducesResponseType(typeof(Payment), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Payment>> GetPayment(int paymentId)
        {
            try
            {
                var payment = await _paymentService.GetPayment(paymentId);
                return Ok(payment);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogError(ex, "Payment not found");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving payment");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Processes a payment.
        /// </summary>
        /// <param name="paymentDTO">Data required to process a payment.</param>
        /// <returns>The processed payment.</returns>
        [HttpPost("ProcessPayment")]
        [ProducesResponseType(typeof(Payment), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Payment>> ProcessPayment(PaymentDTO paymentDTO)
        {
            try
            {
                var payment = await _paymentService.ProcessPayment(paymentDTO);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing payment");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Processes a payment refund.
        /// </summary>
        /// <param name="paymentDTO">Data required to process a payment refund.</param>
        /// <returns>The processed payment refund.</returns>
        [HttpPost("ProcessPaymentRefund")]
        [ProducesResponseType(typeof(RefundTransaction), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RefundTransaction>> ProcessPaymentRefund(PaymentDTO paymentDTO)
        {
            try
            {
                var refundTransaction = await _paymentService.ProcessPaymentRefund(paymentDTO);
                return Ok(refundTransaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing payment refund");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a payment refund by ID.
        /// </summary>
        /// <param name="paymentId">The ID of the payment refund to retrieve.</param>
        /// <returns>The payment refund with the specified ID.</returns>
        [HttpGet("PaymentRefund/{paymentId}")]
        [ProducesResponseType(typeof(RefundTransaction), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RefundTransaction>> GetRefundPayment(int paymentId)
        {
            try
            {
                var refundPayment = await _paymentService.GetPaymentRefund(paymentId);
                return Ok(refundPayment);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogError(ex, "Refund payment not found");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving refund payment");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
    }
}
