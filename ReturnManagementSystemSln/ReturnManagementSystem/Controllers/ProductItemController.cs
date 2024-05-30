using Microsoft.AspNetCore.Mvc;
using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models.DTOs.ProductDTOs;
using ReturnManagementSystem.Models;

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
        /// Updates the status of a product item.
        /// </summary>
        /// <param name="serialNumber">The serial number of the product item.</param>
        /// <param name="status">The new status.</param>
        /// <returns>The updated product item.</returns>
        [HttpPut("UpdateProductItemStatus/{serialNumber}")]
        [ProducesResponseType(typeof(ProductItem), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductItem>> UpdateProductItemStatus(string serialNumber, [FromBody] string status)
        {
            try
            {
                var productItem = await _productItemService.UpdateProductItemStatus(serialNumber, status);
                return Ok(productItem);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger.LogWarning(ex, "Product item not found with serial number {SerialNumber}", serialNumber);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for product item with serial number {SerialNumber}", serialNumber);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
    }
}
