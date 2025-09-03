
    using E_CommerceSystem.Models;
using E_CommerceSystem.Repositories;
namespace E_CommerceSystem.Services
{
    public class AdminServices
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
    }
}
