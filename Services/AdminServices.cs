
    using E_CommerceSystem.Models;
using E_CommerceSystem.Repositories;
namespace E_CommerceSystem.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly IOrderProductsRepo _orderProductsRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly IProductRepo _productRepo;
        private readonly IUserRepo _userRepo;
        private readonly IReviewRepo _reviewRepo;

        public AdminServices(IOrderProductsRepo orderProductsRepo, IOrderRepo orderRepo, IProductRepo productRepo, IUserRepo userRepo, IReviewRepo reviewRepo)
        {
            _orderProductsRepo = orderProductsRepo;
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
            _reviewRepo = reviewRepo;
        }

        public IEnumerable<object> GetBestsellingproducts(int topN)
        {
            var bestselling = _orderProductsRepo.GetAllOrders()
                .GroupBy(op => op.PID)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = _productRepo.GetProductById(g.Key)?.ProductName,
                    TotalSales = g.Count(),
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(topN)
                .ToList();
            return bestselling;
        }

        public IEnumerable<object> GetRevenuePerDay()
        {
            var revenue = _orderRepo.GetAllOrders()
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                })
                .ToList();
            return revenue;
        }
        public IEnumerable<object> GetRevenuePerMonth()
        {
            var revenue = _orderProductsRepo.GetAllOrders()
                .GroupBy(op => new { op.Order.OrderDate.Year, op.Order.OrderDate.Month })
                .Select(global => new
                {
                    Year = global.Key.Year,
                    Month = global.Key.Month,
                    TotalRevenue = global.Sum(op => op.Quantity * op.product.Price)
                })
                .ToList();
            return revenue;
        }
        public IEnumerable<object> GetTopRatedProducts(int topN)
        {
            var topRated = _reviewRepo.GetAllReviews()
                .GroupBy(r => r.PID)
                .Select(g => new
                {
                    ProductId = g.Key,
                    _productRepo.GetProductById(g.Key)?.ProductName,
                    AverageRating = g.Average(r => r.Rating),

                })
                .OrderByDescending(x => x.AverageRating)
                .Take(topN)
                .ToList();
            return topRated;
        }

        public IEnumerable<object> GetMostActiveCustomers(int topN)
        {
            var activeUsers = _orderRepo.GetAllOrders()
                .GroupBy(o => o.UID)
                .Select(g => new
                {
                    UserId = g.Key,
                    UserName = _userRepo.GetUserById(g.Key)?.UName,
                    OrdersCount = g.Count()
                })
                .OrderByDescending(x => x.OrdersCount)
                .Take(topN)
                .ToList();

            return activeUsers;
        }
    }
}
