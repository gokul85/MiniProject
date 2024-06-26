using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs;
using System.Security.Claims;

namespace ReturnManagementSystem.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates the user and returns a token if successful.
        /// </summary>
        /// <param name="userLoginDTO">The user login data transfer object.</param>
        /// <returns>A token if login is successful.</returns>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginReturnDTO>> Login([FromBody] UserLoginDTO userLoginDTO)
        {
            try
            {
                var result = await _userService.Login(userLoginDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for user: {Username}", userLoginDTO.Username);
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userDTO">The user registration data transfer object.</param>
        /// <returns>A message indicating the result of the registration.</returns>
        [HttpPost("Register")]
        [ProducesResponseType(typeof(RegisterReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegisterReturnDTO>> Register([FromBody] RegisterUserDTO userDTO)
        {
            try
            {
                RegisterReturnDTO result = await _userService.Register(userDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for user: {Username}", userDTO.Username);
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        /// <summary>
        /// Updates the status of a user. Only accessible by Admins.
        /// </summary>
        /// <param name="userstatusDTO">The user status update data transfer object.</param>
        /// <returns>A message indicating the result of the update.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateStatus")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> UpdateStatus(UserUpdateStatusDTO userstatusDTO)
        {
            try
            {
                string result = await _userService.UpdateUserStatus(userstatusDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update status failed for user: {UserId}", userstatusDTO.UserId);
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves all users. Only accessible by Admins.
        /// </summary>
        /// <returns>A list of all users.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                var result = await _userService.GetAllUsers();
                return Ok(result);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogError(ex, "GetAllUsers failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves all users count
        /// </summary>
        /// <returns>Users Count</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsersCount")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GetAllUsersCount()
        {
            try
            {
                var result = await _userService.GetAllUsers();
                return Ok(result.Count());
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogError(ex, "GetAllUsers failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        /// <summary>
        /// Updates the role of a user. Only accessible by Admins.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="role">The new role of the user.</param>
        /// <returns>A message indicating the result of the update.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateUserRole")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> UpdateUserRole(int userId, string role)
        {
            try
            {
                string result = await _userService.UpdateUserRole(userId, role);
                return Ok(result);
            }
            catch (NoUserFoundException ex)
            {
                _logger.LogError(ex, "UpdateUserRole failed for userId: {UserId}", userId);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        [Authorize]
        [HttpGet("VerifyRole")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> VerifyRole()
        {
            try
            {
                var result = User.FindFirstValue(ClaimTypes.Role);
                return Ok(result);
            }
            catch (NoUserFoundException ex)
            {
                return NotFound(new ErrorModel(400, ex.Message));
            }
        }
    }
}
