using salhoubapi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using salhoubapi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));  // Ensure you have the connection string in your appsettings.json

// Add Identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

// Add custom authentication service
builder.Services.AddScoped<IAuthService, AuthService>();

// Add controllers and JWT configuration
builder.Services.AddControllers();
builder.Services.AddAuthentication(options =>{options.DefaultAuthenticateScheme = "JwtBearer";options.DefaultChallengeScheme = "JwtBearer";}).AddJwtBearer("JwtBearer", options =>{options.TokenValidationParameters = new TokenValidationParameters{        ValidateIssuer = true,ValidateAudience = true,ValidateLifetime = true,ValidateIssuerSigningKey = true,ValidIssuer = builder.Configuration["Jwt:Issuer"],ValidAudience = builder.Configuration["Jwt:Audience"],IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))};});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
