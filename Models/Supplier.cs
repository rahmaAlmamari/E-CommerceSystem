using System.ComponentModel.DataAnnotations;

namespace E_CommerceSystem.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }
        [Required, MaxLength(150)]
        public string Name { get; set; }

        [Required, MaxLength(256)]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "Invalid email format.(e.g 'example@gmail.com')")]
        public string ContactEmail { get; set; }
        [Required, MaxLength(8)]
        public string Phone { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        //for lazy loading ...
        //protected Supplier() { }
    }
}
