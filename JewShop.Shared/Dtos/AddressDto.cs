namespace JewShop.Shared.Dtos
{
    public class AddressDto
    {
        public int AddressId { get; set; }
        public string Label { get; set; } = "Nhà riêng"; 
        public string FullAddress { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Phone { get; set; }
        public bool IsDefault { get; set; }
    }
}