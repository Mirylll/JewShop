using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewShop.Server.Models
{
    [Table("Sessions")]
    public class Session
    {
        [Key]
        [Column("session_id")]
        public int SessionId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("refresh_token_hash")]
        public string RefreshTokenHash { get; set; } = string.Empty;

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("revoked")]
        public byte Revoked { get; set; } = 0;
    }
}