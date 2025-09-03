using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceSystem.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        //[ForeignKey("User")]
        public int UserId { get; set; }

        [Required, MaxLength(200)]
        public string Token { get; set; } = string.Empty; // Store as random opaque string
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }

        [MaxLength(45)]
        public string CreatedByIp { get; set; } = string.Empty;

        public DateTime? Revoked { get; set; }

        [MaxLength(45)]
        public string? RevokedByIp { get; set; }

        [MaxLength(200)]
        public string? ReplacedByToken { get; set; }// for rotation

        [NotMapped]
        public bool IsActive => Revoked == null && !IsExpired;
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= Expires;

        // Navigation
        public virtual User User { get; set; } = default!;
    }

}
