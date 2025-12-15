using JewShop.Server.Data;
using JewShop.Server.Services.Implementations;
using JewShop.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

// 2. Đăng ký Services

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:7200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5289/") });


builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
// 3. Cấu hình Authentication (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("Jwt:Key").Value!)),
            
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
            
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
            
            ValidateLifetime = true 
        };
    });

// 4. Cấu hình Controller và Blazor View (QUAN TRỌNG)
builder.Services.AddControllersWithViews(); // <-- Sửa: Thêm WithViews
builder.Services.AddRazorPages();           // <-- MỚI: Bắt buộc cho Blazor Hosted

builder.Services.AddOpenApi(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. Cấu hình Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.UseWebAssemblyDebugging(); // <-- MỚI: Để debug được code C# bên Client
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles(); // <-- MỚI: Dòng quan trọng nhất để load file Blazor
app.UseStaticFiles();          // <-- MỚI: Để load ảnh, css, js trong wwwroot
app.UseCors("AllowClient"); 
app.UseAuthentication(); 
app.UseAuthorization();  

app.MapRazorPages(); // <-- MỚI


app.MapControllers();
app.MapFallbackToFile("index.html"); // <-- MỚI: Nếu không tìm thấy API, trả về giao diện Web

app.Run();
