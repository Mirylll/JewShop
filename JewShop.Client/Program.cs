using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JewShop.Client;
using JewShop.Client.Services;
using Blazored.LocalStorage;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// S?a BaseAddress d? tr? d?n Backend API (HTTP)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5289/") });

builder.Services.AddScoped<SupplierDataService>();
builder.Services.AddScoped<ProductDataService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CouponService>();

await builder.Build().RunAsync();
