using System.ComponentModel.DataAnnotations;

namespace E_CommerceSystem.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        // for lazy loading ...
        //protected Category() { }
    }
}
