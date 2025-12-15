namespace JewShop.Shared.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string Type { get; set; } = "customer"; // customer, admin
        public string Status { get; set; } = "active"; // active, locked
        public DateTime CreatedAt { get; set; }
    }
}
