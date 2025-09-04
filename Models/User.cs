using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_CommerceSystem.Models
{
    public class User
    {
        [Key]
        public int UID { get; set; }

        [Required, MaxLength(100)]
        public string UName { get; set; }

        [Required, MaxLength(256)]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "Invalid email format.(e.g 'example@gmail.com')")]
        public string Email { get; set; }

        [JsonIgnore]
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter," +
            " one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; }

        [Required, MaxLength(8)]
        public string Phone {  get; set; }

        [Required, MaxLength(50)]
        public string Role { get; set; }

        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }

        [JsonIgnore]
        public virtual ICollection<Review> Reviews { get; set; }

        //for lazy loading ...
        //protected User() { }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();


    }
}
