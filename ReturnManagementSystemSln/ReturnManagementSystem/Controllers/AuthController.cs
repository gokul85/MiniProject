using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs;

namespace ReturnManagementSystem.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
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
    }
}
