using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_CommerceSystem.Models
{
    public enum OrderStatus
    {
        Pending,
        Paid,
        Shipped,
        Delivered,
        Cancelled
    }
    public class Order
    {
        [Key] 
        public int OID { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        [ForeignKey("user")]
        public int UID { get; set; }
        public virtual User user { get; set; }

        [JsonIgnore]
        public virtual ICollection <OrderProducts> OrderProducts { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]//Optional: to store enum as string in JSON
        public OrderStatus Status { get; set; }=OrderStatus.Pending;
        // for lazy loading ...
        //protected Order() { }
    }
}
