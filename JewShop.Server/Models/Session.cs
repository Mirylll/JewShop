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

        [Column("device_id")]
        public string? DeviceId { get; set; }

        [Column("refresh_token_hash")]
        public string RefreshTokenHash { get; set; } = string.Empty;

        // --- BỔ SUNG CÁC TRƯỜNG BỊ THIẾU ---
        [Column("ip_address")]
        public string? IpAddress { get; set; }

        [Column("user_agent")]
        public string? UserAgent { get; set; }
        // -----------------------------------

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        [Column("revoked")]
        public byte Revoked { get; set; } = 0; // 0: False, 1: True

        // (Tùy chọn) Navigation property để Join bảng Users nếu cần
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}