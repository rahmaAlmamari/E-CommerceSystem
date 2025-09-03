using E_CommerceSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace E_CommerceSystem.Controllers
{
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _adminService;

        public AdminController(IAdminServices adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("bestselling-products")]
        public IActionResult GetBestSellingProducts([FromQuery] int topN)
        {
            var result = _adminService.GetBestsellingproducts(topN);
            return Ok(result);
        }

        [HttpGet("revenue-per-day")]
        public IActionResult GetRevenuePerDay()
        {
            var result = _adminService.GetRevenuePerDay();
            return Ok(result);
        }

        [HttpGet("revenue-per-month")]
        public IActionResult GetRevenuePerMonth()
        {
            var result = _adminService.GetRevenuePerMonth();
            return Ok(result);
        }

        [HttpGet("top-rated-products")]
        public IActionResult GetTopRatedProducts([FromQuery] int topN)
        {
            var result = _adminService.GetTopRatedProducts(topN);
            return Ok(result);
        }

        [HttpGet("most-active-customers")]
       public IActionResult GetMostActiveCustomers([FromQuery] int topN)
        {
            var result = _adminService.GetMostActiveCustomers(topN);
            return Ok(result);
        }
    }
}
