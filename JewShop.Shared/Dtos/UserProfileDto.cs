namespace JewShop.Shared.Dtos
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string Type { get; set; } = "customer";
        
        // Danh sách địa chỉ của user này
        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();
    }
}