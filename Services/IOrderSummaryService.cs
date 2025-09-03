using E_CommerceSystem.Models;

namespace E_CommerceSystem.Services
{
    public interface IOrderSummaryService
    {
        OrderSummaryDTO GetOrderSummary(int orderId);
    }
}