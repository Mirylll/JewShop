namespace JewShop.Shared.Dtos;

public class DashboardStatsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TodayRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockVariants { get; set; }
    public List<MonthlyRevenuePointDto> MonthlyRevenue { get; set; } = new();
    public List<TopProductStatDto> TopProducts { get; set; } = new();
    public List<CategoryRevenueSliceDto> CategoryRevenueDistribution { get; set; } = new();
}

public class MonthlyRevenuePointDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Revenue { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class TopProductStatDto
{
    public string ProductName { get; set; } = string.Empty;
    public int UnitsSold { get; set; }
    public decimal Revenue { get; set; }
}

public class CategoryRevenueSliceDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Percentage { get; set; }
}
