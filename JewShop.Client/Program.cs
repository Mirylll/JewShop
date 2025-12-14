using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JewShop.Client;
using JewShop.Client.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("http://localhost:5289/") });

builder.Services.AddScoped<CouponDataService>();
builder.Services.AddScoped<ProductDataService>();
builder.Services.AddScoped<SupplierDataService>();
builder.Services.AddScoped<DashboardDataService>();

await builder.Build().RunAsync();
