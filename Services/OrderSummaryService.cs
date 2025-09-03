using AutoMapper;
using E_CommerceSystem.Models;
using E_CommerceSystem.Repositories;
namespace E_CommerceSystem.Services
{
    public class OrderSummaryService : IOrderSummaryService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IUserService _userService;
        private readonly IOrderProductsService _orderProductsService;
        private readonly IProductService _productService;
        public OrderSummaryService(
         IOrderRepo orderRepo,
        IUserService userService,
        IOrderProductsService orderProductsService,
        IProductService productService)
        {
            _orderRepo = orderRepo;
            _userService = userService;
            _orderProductsService = orderProductsService;
            _productService = productService;
        }

        public OrderSummaryDTO GetOrderSummary(int orderId)
        {
            var order = _orderRepo.GetOrderById(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            var user = _userService.GetUserById(order.UID);

            var orderProducts = _orderProductsService.GetOrdersByOrderId(orderId);

            var productDetails = new List<OrderProductDetailDTO>();

            foreach (var op in orderProducts)
            {
                var product = _productService.GetProductById(op.PID);
                if (product != null)
                {
                    productDetails.Add(new OrderProductDetailDTO
                    {

                        ProductName = product.ProductName,
                        Quantity = op.Quantity,
                        Price = product.Price,
                        TotalPrice = op.Quantity * product.Price
                    });
                }
            }
            return new OrderSummaryDTO
            {
                OrderId = order.OID,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                UserName = user.UName,
                Email = user.Email,
                Products = productDetails
            };
        }
    }
}

