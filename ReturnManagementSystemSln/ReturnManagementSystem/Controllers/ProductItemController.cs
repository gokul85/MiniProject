using Microsoft.AspNetCore.Mvc;
using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models.DTOs.ProductDTOs;
using ReturnManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace ReturnManagementSystem.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ProductItemController : ControllerBase
    {
        private readonly IProductItemService _productItemService;
        private readonly ILogger<ProductItemController> _logger;

        public ProductItemController(IProductItemService productItemService, ILogger<ProductItemController> logger)
        {
            _productItemService = productItemService;
            _logger = logger;
        }

        /// <summary>
        /// Adds new product items.
        /// </summary>
        /// <param name="addProductItemDTOs">The product items to add.</param>
        /// <returns>The response containing added product items and errors.</returns>
        [HttpPost("AddProductItems")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AddProductItemsResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddProductItemsResponse>> AddProductItems([FromBody] List<AddProductItemDTO> addProductItemDTOs)
        {
            try
            {
                var response = await _productItemService.AddProductItem(addProductItemDTOs);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product items");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves all product items.
        /// </summary>
        /// <returns>A list of product items.</returns>
        [HttpGet("GetAllProductItems")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<ProductItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<ProductItem>>> GetAllProductItems()
        {
            try
            {
                var productItems = await _productItemService.GetAllProductItems();
                return Ok(productItems);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogWarning(ex, "No product items found.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all product items failed.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves product items by status.
        /// </summary>
        /// <param name="status">The status of the product items.</param>
        /// <returns>A list of product items with the specified status.</returns>
        [HttpGet("GetProductItemsByStatus")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<ProductItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<ProductItem>>> GetProductItemsByStatus(string status)
        {
            try
            {
                var productItems = await _productItemService.GetAllProductItems(status);
                return Ok(productItems);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogWarning(ex, "No product items found with status: {Status}", status);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get product items by status failed.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves product items by product ID.
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <returns>A list of product items with the specified product ID.</returns>
        [HttpGet("GetProductItemsByProductId")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<ProductItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<ProductItem>>> GetProductItemsByProductId(int productId)
        {
            try
            {
                var productItems = await _productItemService.GetAllProductItemsByProductId(productId);
                return Ok(productItems);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogWarning(ex, "No product items found with product ID: {ProductId}", productId);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get product items by product ID failed.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Updates the status of a product item.
        /// </summary>
        /// <param name="serialNumber">The serial number of the product item.</param>
        /// <param name="status">The new status.</param>
        /// <returns>The updated product item.</returns>
        [HttpPut("UpdateProductItemStatus")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductItem), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductItem>> UpdateProductItemStatus(UpdateProductItemStatus upis)
        {
            try
            {
                var productItem = await _productItemService.UpdateProductItemStatus(upis);
                return Ok(productItem);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogWarning(ex, "Product item not found with serial number {SerialNumber}", upis.SerialNumber);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for product item with serial number {SerialNumber}", upis.SerialNumber);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Updates a product item to refurbished status.
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <param name="serialNumber">The serial number of the product item.</param>
        /// <returns>The updated product item.</returns>
        [HttpPut("UpdateProductItemRefurbished")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductItem), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductItem>> UpdateProductItemRefurbished(int productId, string serialNumber)
        {
            try
            {
                var productItem = await _productItemService.UpdateProductItemRefurbished(productId, serialNumber);
                return Ok(productItem);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product item to refurbished status with serial number {SerialNumber}", serialNumber);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
    }
}
