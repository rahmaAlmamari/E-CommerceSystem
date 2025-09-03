
namespace E_CommerceSystem.Services
{
    public interface IAdminServices
    {
        IEnumerable<object> GetBestsellingproducts(int topN);
        IEnumerable<object> GetMostActiveCustomers(int topN);
        IEnumerable<object> GetRevenuePerDay();
        IEnumerable<object> GetRevenuePerMonth();
        IEnumerable<object> GetTopRatedProducts(int topN);
    }
}