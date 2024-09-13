using Microsoft.AspNetCore.Mvc;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models.DTOs.ProductDTOs;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace ReturnManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new product.
        /// </summary>
        /// <param name="productDTO">The product data transfer object.</param>
        /// <returns>The added product.</returns>
        [HttpPost("AddProduct")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductReturnDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductReturnDTO>> AddProduct([FromBody] ProductDTO productDTO)
        {
            try
            {
                productDTO.Status = "Fresh";
                var product = await _productService.AddProduct(productDTO);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add product failed for product: {ProductName}", productDTO.Name);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A list of products.</returns>
        [HttpGet("GetAllProducts")]
        [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                return Ok(products);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogWarning(ex, "No products found.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all products failed.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves all products count.
        /// </summary>
        /// <returns>Products Count.</returns>
        [HttpGet("GetAllProductsCount")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GetAllProductsCount()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                return Ok(products.Count());
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogWarning(ex, "No products found.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all products failed.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The product if found.</returns>
        [HttpGet("GetProductById")]
        [ProducesResponseType(typeof(ProductReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductReturnDTO>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                return Ok(product);
            }
            catch (ObjectsNotFoundException ex)
            {
                _logger.LogWarning(ex, "No product found.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get product by ID failed for ID: {ProductId}", id);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <param name="productDTO">The updated product data transfer object.</param>
        /// <returns>The updated product.</returns>
        [HttpPut("UpdateProduct")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductReturnDTO>> UpdateProduct(int id, [FromBody] ProductDTO productDTO)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProduct(id, productDTO);
                return Ok(updatedProduct);
            }
            catch (ObjectNotFoundException)
            {
                _logger.LogWarning("Update product failed. Product not found: {ProductId}", id);
                return NotFound(new ErrorModel(404, "Product not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update product failed for product ID: {ProductId}", id);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }
    }
}
