using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JewShop.Client;
using JewShop.Client.Services;
using JewShop.Client.Providers;             // <-- MỚI: Để nhận diện CustomAuthStateProvider
using Blazored.LocalStorage;                // <-- MỚI: Để dùng LocalStorage
using Microsoft.AspNetCore.Components.Authorization; // <-- MỚI: Để dùng AuthenticationStateProvider

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ============================================================
// 1. CẤU HÌNH HTTP CLIENT
// ============================================================
// Dùng BaseAddress của Host (Server) là chuẩn nhất cho mô hình Hosted.
// Nó sẽ tự động lấy http://localhost:5289 khi chạy local.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// ============================================================
// 2. CẤU HÌNH AUTHENTICATION (XÁC THỰC) - QUAN TRỌNG
// ============================================================

// A. Thêm LocalStorage để lưu Token sau khi đăng nhập
builder.Services.AddBlazoredLocalStorage();

// B. Thêm Core Authorization để dùng được thẻ <AuthorizeView> trong HTML
builder.Services.AddAuthorizationCore();

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