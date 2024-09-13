using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;
using ReturnManagementSystem.Models;
using System.Security.Claims;

namespace ReturnManagementSystem.Controllers
{
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
        [Authorize]
        [ProducesResponseType(typeof(ReturnRequest), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReturnRequest>> OpenReturnRequest([FromBody] ReturnRequestDTO returnRequestDTO)
        {
            try
            {
                int userid = int.Parse(User.FindFirstValue("uid"));
                returnRequestDTO.UserId = userid;
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
        [HttpPost("TechnicalReview")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ReturnRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReturnRequest>> TechnicalReview(TechnicalReviewDTO technicalReviewDTO)
        {
            try
            {
                var returnRequest = await _returnRequestService.TechnicalReview(technicalReviewDTO);
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
        [HttpPut("UpdateUserSerialNumber")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ReturnRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReturnRequest>> UpdateUserSerialNumber(UpdateRequestSerialNumberDTO ursn)
        {
            try
            {
                var returnRequest = await _returnRequestService.UpdateUserSerialNumber(ursn);
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
        /// <param name="closerequestDTO">Close Request DTO</param>
        /// <returns>The closed return request.</returns>
        [HttpPut("CloseReturnRequest")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ReturnRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReturnRequest>> CloseReturnRequest(CloseRequestDTO closerequestDTO)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue("uid"));
                var returnRequest = await _returnRequestService.CloseReturnRequest( userId, closerequestDTO);
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

        /// <summary>
        /// Gets all active return requests.
        /// </summary>
        /// <returns>A list of active return requests.</returns>
        [HttpGet("GetAllReturnRequests")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<ReturnRequest>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ReturnRequest>>> GetAllReturnRequests()
        {
            try
            {
                var returnRequests = await _returnRequestService.GetAllReturnRequests();
                return Ok(returnRequests);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogError(ex, "No return requests found");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving return requests");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Gets all active return requests Count
        /// </summary>
        /// <returns>return requests count.</returns>
        [HttpGet("GetAllReturnRequestsCount")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GetAllReturnRequestsCount()
        {
            try
            {
                var returnRequests = await _returnRequestService.GetAllReturnRequests();
                return Ok(returnRequests.Count());
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogError(ex, "No return requests found");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving return requests");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Gets all return requests for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of return requests for the user.</returns>
        [HttpGet("GetAllUserReturnRequests")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(IEnumerable<ReturnRequest>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ReturnRequest>>> GetAllUserReturnRequests()
        {
            try
            {
                int userid = int.Parse(User.FindFirstValue("uid"));
                var returnRequests = await _returnRequestService.GetAllUserReturnRequests(userid);
                return Ok(returnRequests);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogError(ex, "No return requests found for user");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving return requests for user");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Gets a specific return request by ID.
        /// </summary>
        /// <param name="requestId">The ID of the return request.</param>
        /// <returns>The return request with the specified ID.</returns>
        [HttpGet("GetReturnRequest")]
        [Authorize(Roles = "User,Admin")]
        [ProducesResponseType(typeof(ReturnRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReturnRequest>> GetReturnRequest(int requestId)
        {
            try
            {
                var returnRequest = await _returnRequestService.GetReturnRequest(requestId);
                return Ok(returnRequest);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogError(ex, "Return request not found");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the return request");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
    }
}
