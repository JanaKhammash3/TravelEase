using System.Text;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using TravelEase.TravelEase.Application.Features.Auth;
using TravelEase.TravelEase.Application.Features.Booking;
using TravelEase.TravelEase.Application.Features.City;
using TravelEase.TravelEase.Application.Features.Hotel;
using TravelEase.TravelEase.Application.Features.Review;
using TravelEase.TravelEase.Application.Features.Room;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Application.Interfaces.Admin;
using TravelEase.TravelEase.Infrastructure.Data;
using TravelEase.TravelEase.Infrastructure.Repositories;
using TravelEase.TravelEase.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using TravelEase.TravelEase.Infrastructure.Services.Admin;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
});

// Logging environment
Console.WriteLine($"ENV: {builder.Environment.EnvironmentName}");

// Config loading (supports Docker)
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Set QuestPDF license (Community Edition)
QuestPDF.Settings.License = LicenseType.Community;

// ==========================
// Add Services
// ==========================
builder.Services.AddControllers();

// ==========================
// JWT Configuration
// ==========================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new Exception("Missing JWT key."));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// ==========================
// Database
// ==========================
builder.Services.AddDbContext<TravelEaseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================
// Cloudinary
// ==========================
builder.Services.AddSingleton(sp =>
{
    var config = builder.Configuration.GetSection("Cloudinary");
    var account = new Account(
        config["CloudName"] ?? throw new Exception("Missing Cloudinary:CloudName"),
        config["ApiKey"] ?? throw new Exception("Missing Cloudinary:ApiKey"),
        config["ApiSecret"] ?? throw new Exception("Missing Cloudinary:ApiSecret")
    );
    return new Cloudinary(account);
});

// ==========================
// Mailtrap Email Service
// ==========================
builder.Services.AddScoped<ISmtpClientWrapper>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new SmtpClientWrapper(
        config["Email:SmtpHost"],
        int.Parse(config["Email:SmtpPort"]),
        config["Email:Username"],
        config["Email:Password"]);
});

builder.Services.AddScoped<Func<ISmtpClientWrapper>>(sp => () =>
{
    return sp.GetRequiredService<ISmtpClientWrapper>();
});

builder.Services.AddScoped<IEmailService, EmailService>();

// ==========================
// App Services
// ==========================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAdminHotelService, AdminHotelService>();
builder.Services.AddScoped<IAdminRoomService, AdminRoomService>();
builder.Services.AddScoped<IAdminCityService, AdminCityService>();
builder.Services.AddScoped<IImageUploader, CloudinaryImageService>();
builder.Services.AddScoped<ICloudinaryWrapper, CloudinaryWrapper>();
builder.Services.AddScoped<CloudinaryImageService>();
builder.Services.AddScoped<IStripeSessionService, StripeSessionService>();

// ==========================
// Repositories
// ==========================
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// ==========================
// Swagger
// ==========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TravelEase API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Bearer (Authorization: Bearer {token})",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ==========================
// CORS
// ==========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// ==========================
// App Build
// ==========================
var app = builder.Build();

// Stripe Setup
var stripeKey = builder.Configuration["Stripe:SecretKey"];
if (!string.IsNullOrEmpty(stripeKey))
    Stripe.StripeConfiguration.ApiKey = stripeKey;

// ==========================
// Middleware Pipeline
// ==========================
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.Run();
