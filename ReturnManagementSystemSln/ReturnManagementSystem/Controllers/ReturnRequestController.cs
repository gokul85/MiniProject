using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;
using ReturnManagementSystem.Models;

namespace ReturnManagementSystem.Controllers
{
    /// <summary>
    /// Controller for managing return requests.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnRequestController : ControllerBase
    {
        private readonly IReturnRequestService _returnRequestService;
        private readonly ILogger<ReturnRequestController> _logger;

        public ReturnRequestController(IReturnRequestService returnRequestService, ILogger<ReturnRequestController> logger)
        {
            _returnRequestService = returnRequestService;
            _logger = logger;
        }

        /// <summary>
        /// Opens a new return request.
        /// </summary>
        /// <param name="returnRequestDTO">Data required to open a return request.</param>
        /// <returns>The created return request.</returns>
        [HttpPost("OpenReturnRequest")]
        [ProducesResponseType(typeof(ReturnRequest), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReturnRequest>> OpenReturnRequest(ReturnRequestDTO returnRequestDTO)
        {
            try
            {
                var returnRequest = await _returnRequestService.OpenReturnRequest(returnRequestDTO);
                return Ok(returnRequest);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogError(ex, "Object not found while opening return request");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (InvalidReturnRequest ex)
            {
                _logger.LogError(ex, "Invalid return request");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while opening return request");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Performs technical review on a return request.
        /// </summary>
        /// <param name="requestId">The ID of the return request.</param>
        /// <param name="process">The process to be performed.</param>
        /// <param name="feedback">Feedback regarding the return request.</param>
        /// <returns>The updated return request after technical review.</returns>
        [HttpPost("TechnicalReview/{requestId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ReturnRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReturnRequest>> TechnicalReview(int requestId, string process, string feedback)
        {
            try
            {
                var returnRequest = await _returnRequestService.TechnicalReview(requestId, process, feedback);
                return Ok(returnRequest);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogError(ex, "Object not found while performing technical review");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (InvalidDataException ex)
            {
                _logger.LogError(ex, "Invalid data while performing technical review");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while performing technical review");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Updates user serial number for a return request.
        /// </summary>
        /// <param name="requestId">The ID of the return request.</param>
        /// <param name="serialNumber">The new serial number to be updated.</param>
        /// <returns>The updated return request after updating the serial number.</returns>
        [HttpPut("UpdateUserSerialNumber/{requestId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ReturnRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReturnRequest>> UpdateUserSerialNumber(int requestId, [FromBody] string serialNumber)
        {
            try
            {
                var returnRequest = await _returnRequestService.UpdateUserSerialNumber(requestId, serialNumber);
                return Ok(returnRequest);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogError(ex, "Object not found while updating user serial number");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (InvalidSerialNumber ex)
            {
                _logger.LogError(ex, "Invalid serial number while updating user serial number");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user serial number");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Closes a return request.
        /// </summary>
        /// <param name="requestId">The ID of the return request.</param>
        /// <param name="userId">The ID of the user closing the return request.</param>
        /// <param name="feedback">Feedback regarding the return request closure.</param>
        /// <returns>The closed return request.</returns>
        [HttpPut("CloseReturnRequest/{requestId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ReturnRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReturnRequest>> CloseReturnRequest(int requestId, int userId, [FromBody] string feedback)
        {
            try
            {
                var returnRequest = await _returnRequestService.CloseReturnRequest(requestId, userId, feedback);
                return Ok(returnRequest);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogError(ex, "Object not found while closing return request");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while closing return request");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
    }
}
