using E_CommerceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderSummaryController : ControllerBase
    {
        private readonly IOrderSummaryService _orderSummaryService;

        public OrderSummaryController(IOrderSummaryService orderSummaryService)
        {
            _orderSummaryService = orderSummaryService;
        }
        [HttpGet("GetOrderSummary/{orderId}")]
        public IActionResult GetOrderSummary(int orderId)
        {
            try
            {
                var summary = _orderSummaryService.GetOrderSummary(orderId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
