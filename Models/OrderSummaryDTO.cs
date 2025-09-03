namespace E_CommerceSystem.Models
{
    public class OrderSummaryDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // User info
        public string UserName { get; set; }
        public string Email { get; set; }

        // Product details
        public List<OrderProductDetailDTO> Products { get; set; }
    }
}
