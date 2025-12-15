namespace JewShop.Shared.Dtos
{
    public class ImportDto
    {
        public int Id { get; set; }
        public string ImportCode { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public DateTime ImportDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "pending";
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ImportItemDto> ImportItems { get; set; } = new();
    }

    public class ImportItemDto
    {
        public int Id { get; set; }
        public int VariantId { get; set; }
        public string? ProductName { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CreateImportDto
    {
        public int SupplierId { get; set; }
        public string? Note { get; set; }
        public List<CreateImportItemDto> Items { get; set; } = new();
    }

    public class CreateImportItemDto
    {
        public int VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
