using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JewShop.Client;
using JewShop.Client.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using JewShop.Client.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// S?a BaseAddress d? tr? d?n Backend API (HTTP)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5289/") });

// ============================================================
// 2. CẤU HÌNH AUTHENTICATION (XÁC THỰC) - QUAN TRỌNG
// ============================================================

// A. Thêm LocalStorage để lưu Token sau khi đăng nhập
builder.Services.AddBlazoredLocalStorage();

// B. Thêm Core Authorization để dùng được thẻ <AuthorizeView> trong HTML
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CouponDataService>();
builder.Services.AddScoped<ProductDataService>();
builder.Services.AddScoped<SupplierDataService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CouponService>();
builder.Services.AddScoped<StatisticsService>();

// C. Đăng ký Custom Provider
// Dòng này báo cho Blazor biết: "Đừng dùng cái check đăng nhập mặc định, hãy dùng cái CustomAuthStateProvider mà tôi vừa viết"
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// D. Đăng ký Service xử lý Logic Đăng nhập/Đăng ký
builder.Services.AddScoped<IAuthService, AuthService>();

// ============================================================
// 3. CÁC SERVICE KHÁC CỦA BẠN
// ============================================================
builder.Services.AddScoped<SupplierDataService>();
builder.Services.AddScoped<IUserService, UserService>();
await builder.Build().RunAsync();