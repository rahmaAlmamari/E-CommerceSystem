namespace E_CommerceSystem.Models
{
    public class OrderProductDetailDTO
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Price per unit
        public decimal TotalPrice { get; set; }
    }
}
