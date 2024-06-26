using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models.DTOs.OrderDTOs;
using ReturnManagementSystem.Models;
using System.Security.Claims;

namespace ReturnManagementSystem.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="orderDTO">The order details.</param>
        /// <returns>The created order.</returns>
        [HttpPost("CreateOrder")]
        [Authorize]
        [ProducesResponseType(typeof(OrderReturnDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderReturnDTO>> CreateOrder([FromBody] OrderDTO orderDTO)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue("uid"));
                orderDTO.UserId = userId;
                var order = await _orderService.CreateOrder(orderDTO);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        /// <summary>
        /// Gets all orders.
        /// </summary>
        /// <returns>A list of orders.</returns>
        [HttpGet("GetAllOrders")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrders();
                return Ok(orders);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogWarning(ex, "Orders not found");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Gets all orders Count.
        /// </summary>
        /// <returns>Orders Count.</returns>
        [HttpGet("GetAllOrdersCount")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GetAllOrdersCount()
        {
            try
            {
                var orders = await _orderService.GetAllOrders();
                return Ok(orders.Count());
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogWarning(ex, "Orders not found");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Gets all orders for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A list of orders.</returns>
        [HttpGet("GetAllUserOrders")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllUserOrders()
        {
            int userId = int.Parse(User.FindFirstValue("uid"));
            try
            {
                var orders = await _orderService.GetAllUserOrders(userId);
                return Ok(orders);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogWarning(ex, "Orders not found for user with ID {UserId}", userId);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for user with ID {UserId}", userId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Gets a specific order by ID.
        /// </summary>
        /// <param name="orderId">The order ID.</param>
        /// <returns>The order details.</returns>
        [HttpGet("GetOrder")]
        [ProducesResponseType(typeof(OrderReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderReturnDTO>> GetOrder(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrder(orderId);
                return Ok(order);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found with ID {OrderId}", orderId);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order with ID {OrderId}", orderId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="orderId">The order ID.</param>
        /// <param name="orderStatus">The new status.</param>
        /// <returns>The updated order.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateOrderStatus")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Order>> UpdateOrderStatus(UpdateOrderStatusDTO uos)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatus(uos);
                return Ok(order);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found with ID {OrderId}", uos.OrderId);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for order with ID {OrderId}", uos.OrderId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
    }
}
