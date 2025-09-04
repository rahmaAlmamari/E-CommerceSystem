using E_CommerceSystem.Models;

namespace E_CommerceSystem.Services
{
    public interface IInvoiceService
    {
        byte[] GenerateInvoice(Order order);
    }
}