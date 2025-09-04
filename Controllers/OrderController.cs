using AutoMapper;
using E_CommerceSystem.Models;
using E_CommerceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace E_CommerceSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IInvoiceService _invoiceService;
        public OrderController(IOrderService orderService, IMapper mapper, IInvoiceService invoiceService)
        {
            _orderService = orderService;
            _mapper = mapper;
            _invoiceService = invoiceService;

        }

        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder([FromBody] List<OrderItemDTO> items)
        {
            try
            {
                if (items == null || !items.Any())
                {
                    return BadRequest("Order items cannot be empty.");
                }

                // Retrieve the Authorization header from the request
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Decode the token to check user role
                var userId = GetUserIdFromToken(token);

                // Extract user ID 
                int uid = int.Parse(userId);

                _orderService.PlaceOrder(items, uid);

                return Ok("Order placed successfully.");
            }
            catch (Exception ex)
            {
                // Return a generic error response
                return StatusCode(500, $"An error occurred while placing order. {(ex.Message)}");
            }
        }

        [HttpGet("GetAllOrders")]
        public IActionResult GetAllOrders()
        {
            try
            {
                // Retrieve the Authorization header from the request
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Decode the token to check user role
                var userId = GetUserIdFromToken(token);

                // Extract user ID 
                int uid = int.Parse(userId);

                // Get all order products for the user
                var orderProducts = _orderService.GetAllOrders(uid);

                // map entities to DTOs instead of returning raw OrderProducts
                var orderItems = _mapper.Map<IEnumerable<OrderItemDTO>>(orderProducts);

                return Ok(orderItems);
            }
            catch (Exception ex)
            {
                // Return a generic error response
                return StatusCode(500, $"An error occurred while retrieving products. {(ex.Message)}");
            }
        }

        [HttpGet("GetOrderById/{OrderId}")]
        public IActionResult GetOrderById(int OrderId)
        {
            try
            {
                // Retrieve the Authorization header from the request
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Decode the token to check user role
                var userId = GetUserIdFromToken(token);

                // Extract user ID 
                int uid = int.Parse(userId);

                // Service already returns OrdersOutputOTD, no manual mapping needed here
                return Ok(_orderService.GetOrderById(OrderId, uid));
            }
            catch (Exception ex)
            {
                // Return a generic error response
                return StatusCode(500, $"An error occurred while retrieving products. {(ex.Message)}");
            }
        }

        // Method to decode token to get user id
        private string? GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);

                // Extract the 'sub' claim
                var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");

                return (subClaim?.Value); // Return both values as a tuple
            }

            throw new UnauthorizedAccessException("Invalid or unreadable token.");
        }


        [HttpGet("GetInvoice/{OrderId}")]
        public IActionResult GetInvoice(int OrderId)
        {
            try
            {
                // Get user id from token
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = GetUserIdFromToken(token);
                int uid = int.Parse(userId);

                // Get order
                var order = _orderService.GetOrderByUserId(uid).FirstOrDefault(o => o.OID == OrderId);
                if (order == null)
                    return NotFound($"Order with ID {OrderId} not found for this user.");

                // Generate invoice PDF
                var pdfBytes = _invoiceService.GenerateInvoice(order);

                // Return file as PDF download
                return File(pdfBytes, "application/pdf", $"Invoice_Order_{OrderId}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while generating invoice. {ex.Message}");
            }

        }
    }
}


