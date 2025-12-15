using JewShop.Server.Data;
using JewShop.Server.Services.Implementations;
using JewShop.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseMySql(
builder.Configuration.GetConnectionString("DefaultConnection"),
ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
)
);

builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IAuthService, AuthService>(); // Registered AuthService
builder.Services.AddScoped<IUserService, UserService>(); // Registered UserService
builder.Services.AddScoped<IImportService, ImportService>(); // Registered ImportService
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

// Cấu hình Authentication (JWT)
builder.Services.AddAuthentication(options =>
{
options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
options.TokenValidationParameters = new TokenValidationParameters
{
ValidateIssuerSigningKey = true,
IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
builder.Configuration.GetSection("Jwt:Key").Value!)),
ValidateIssuer = false,
ValidateAudience = false,
RoleClaimType = ClaimTypes.Role // IMPORTANT: Map role claim for [Authorize(Roles = "admin")]
};
});

// CORS cho phép Client gọi API - Đã cập nhật thêm port 7194
builder.Services.AddCors(options =>
{
options.AddPolicy("AllowBlazorClient", policy =>
{
policy.WithOrigins(
"http://localhost:5049", 
"https://localhost:7049",
"https://localhost:7194"  // Thêm port HTTPS của Client
)
.AllowAnyMethod()
.AllowAnyHeader();
});
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorClient");

app.UseAuthentication(); // Must be before Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();