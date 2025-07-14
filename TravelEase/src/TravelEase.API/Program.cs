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

var builder = WebApplication.CreateBuilder(args);

// ✅ Set QuestPDF license (Community Edition)
QuestPDF.Settings.License = LicenseType.Community;

// Controllers
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
// Database Configuration
// ==========================
builder.Services.AddDbContext<TravelEaseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================
// Cloudinary Configuration
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
// Email Configuration (Mailtrap Ready)
// ==========================
builder.Services.AddScoped<ISmtpClientWrapper>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var host = config["Email:SmtpHost"];
    var port = int.Parse(config["Email:SmtpPort"]);
    var user = config["Email:Username"];
    var pass = config["Email:Password"];
    return new SmtpClientWrapper(host, port, user, pass);
});

// ✅ Add the factory for ISmtpClientWrapper (fixes the crash)
builder.Services.AddScoped<Func<ISmtpClientWrapper>>(sp => () =>
{
    return sp.GetRequiredService<ISmtpClientWrapper>();
});

builder.Services.AddScoped<IEmailService, EmailService>();

// ==========================
// Service Registrations
// ==========================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAdminHotelService, AdminHotelService>();
builder.Services.AddScoped<IAdminRoomService, AdminRoomService>();
builder.Services.AddScoped<IAdminCityService, AdminCityService>();
builder.Services.AddScoped<IImageUploader, CloudinaryImageService>();
builder.Services.AddScoped<ICloudinaryWrapper, CloudinaryWrapper>();
builder.Services.AddScoped<CloudinaryImageService>();
builder.Services.AddScoped<IStripeSessionService, StripeSessionService>();

// ==========================
// Repository Registrations
// ==========================
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// ==========================
// CORS Configuration
// ==========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ==========================
// Swagger Configuration
// ==========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TravelEase API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
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
            Array.Empty<string>()
        }
    });
});

// ==========================
// Build App
// ==========================
var app = builder.Build();

// Stripe
var stripeKey = builder.Configuration["Stripe:SecretKey"];
if (!string.IsNullOrEmpty(stripeKey))
    Stripe.StripeConfiguration.ApiKey = stripeKey;

// Middleware
if (app.Environment.IsDevelopment())
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
