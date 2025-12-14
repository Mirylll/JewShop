using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewShop.Server.Models
{
    [Table("PasswordResets")] // Map với bảng PasswordResets trong DB
    public class PasswordReset
    {
        [Key]
        [Column("reset_id")]
        public int ResetId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("token")]
        public string Token { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        [Column("used")]
        public byte Used { get; set; } = 0; // 0: chưa dùng, 1: đã dùng
    }
}