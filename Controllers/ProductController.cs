using AutoMapper;
using E_CommerceSystem.Models;
using E_CommerceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using E_CommerceSystem.Auth;

namespace E_CommerceSystem.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IConfiguration configuration, IMapper mapper)
        {
            _productService = productService;
            _configuration = configuration;
            _mapper = mapper;
        }

        [Authorize(Policy = "AdminOrManager")]
        [HttpPost("AddProduct")]
        public IActionResult AddNewProduct(ProductDTO productInput, int sid, int cid)
        {
            //try
            //{
                // Only allow Admin users to add products
                //if (!User.IsInRole("admin"))
                //{
                //    // You are authenticated but not allowed
                //    return Forbid(); // better than BadRequest for authorization
                //}

                // Check if input data is null
                if (productInput == null)
                {
                    // return BadRequest("Product data is required.");
                    throw new AppException("Product data is required.", 400, "VALIDATION_ERROR");
                }

                // AutoMapper ProductDTO -> Product ...
                var product = _mapper.Map<Product>(productInput);
                product.OverallRating = 0; // keep your initial rating behavior
                product.SupplierId = sid;
                product.CategoryId = cid;

                // Add the new product to the database/service layer
                _productService.AddProduct(product);

                return Ok(product);
            //}
            //catch (Exception ex)
            //{
            //    // Return a generic error response
            //    // return StatusCode(500, $"An error occurred while adding the product: {ex.Message}");
            //    throw; // let the ErrorHandlingMiddleware return a consistent 500 response
            //}
        }

        [Authorize(Policy = "AdminOrManager")]
        [HttpPut("UpdateProduct/{productId}")]
        public IActionResult UpdateProduct(int productId, ProductDTO productInput)
        {
            //try
            //{
            //    // Only allow Admin users to add products
            //    if (!User.IsInRole("admin"))
            //    {
            //        return Forbid();
            //    }

                if (productInput == null)
                    // return BadRequest("Product data is required.");
                    throw new AppException("Product data is required.", 400, "VALIDATION_ERROR");

                var product = _productService.GetProductById(productId);

                // AutoMapper replacement for manual mapping ...
                _mapper.Map(productInput, product);

                _productService.UpdateProduct(product);

                return Ok(product);
            //}
            //catch (Exception ex)
            //{
            //    // Return a generic error response
            //    // return StatusCode(500, $"An error occurred while updte product. {(ex.Message)}");
            //    throw; // centralized middleware will handle 500
            //}
        }

        [AllowAnonymous]
        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts(
        [FromQuery] string? name,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            //try
            //{
                // Validate pagination parameters
                if (pageNumber < 1 || pageSize < 1)
                {
                    // return BadRequest("PageNumber and PageSize must be greater than 0.");
                    throw new AppException("PageNumber and PageSize must be greater than 0.", 400, "VALIDATION_ERROR");
                }

                // Call the service to get the paged and filtered products
                var products = _productService.GetAllProducts(pageNumber, pageSize, name, minPrice, maxPrice);

                if (products == null || !products.Any())
                {
                    // return NotFound("No products found matching the given criteria.");
                    throw new AppException("No products found matching the given criteria.", 404, "NOT_FOUND");
                }

                return Ok(products);
            //}
            //catch (Exception ex)
            //{
            //    // Return a generic error response
            //    // return StatusCode(500, $"An error occurred while retrieving products. {ex.Message}");
            //    throw; // centralized middleware will handle 500
            //}
        }

        [AllowAnonymous]
        [HttpGet("GetProductByID/{ProductId}")]
        public IActionResult GetProductById(int ProductId)
        {
            //try
            //{
                var product = _productService.GetProductById(ProductId);
                if (product == null)
                    // return NotFound("No product found.");
                    throw new AppException("No product found.", 404, "NOT_FOUND");

                return Ok(product);
            //}
            //catch (Exception ex)
            //{
            //    // Return a generic error response
            //    // return StatusCode(500, $"An error occurred while retrieving product. {(ex.Message)}");
            //    throw; // centralized middleware will handle 500
            //}
        }
        private string? GetUserRoleFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);

                // Extract the 'role' claim
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == "unique_name");


                return roleClaim?.Value; // Return the role or null if not found
            }

            throw new UnauthorizedAccessException("Invalid or unreadable token.");
        }

        [HttpPost("UploadProductImage/{productId}")]
        public IActionResult UploadProductImage(int productId, IFormFile imageFile)
        {
            //try
            //{
                if (imageFile == null || imageFile.Length == 0)

                    // return BadRequest("No file uploaded.");
                    throw new AppException("No file uploaded.", 400, "VALIDATION_ERROR");
                //call servervice method to save image
                var imageUrl = _productService.SaveProductImage(productId, imageFile);

                return Ok(new { Message = "Image uploaded successfully.", ImageUrl = imageUrl });
            //}
            //catch (Exception ex)
            //{
            //    // return StatusCode(500, $"An error occurred while uploading the image. {ex.Message}");
            //    throw; // centralized middleware will handle 500
            //}
        }
    }
}
