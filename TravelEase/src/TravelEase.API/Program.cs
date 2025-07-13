using System.Text;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

// Add controllers
builder.Services.AddControllers();

// Load JWT config
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new Exception("Missing JWT key."));

// Configure JWT authentication
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

// Register EF Core DbContext
builder.Services.AddDbContext<TravelEaseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================
// Cloudinary Configuration
// ==========================
builder.Services.AddSingleton(sp =>
{
    var config = builder.Configuration.GetSection("Cloudinary");
    var account = new Account(
        config["CloudName"],
        config["ApiKey"],
        config["ApiSecret"]);
    return new Cloudinary(account);
});

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAdminHotelService, AdminHotelService>();
builder.Services.AddScoped<IAdminRoomService, AdminRoomService>();
builder.Services.AddScoped<IAdminCityService, AdminCityService>();
builder.Services.AddScoped<IImageUploader, CloudinaryImageService>();
builder.Services.AddScoped<ICloudinaryWrapper, CloudinaryWrapper>();
builder.Services.AddScoped<CloudinaryImageService>();
builder.Services.AddScoped<IStripeSessionService, StripeSessionService>();

// Repositories
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger
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
            new string[] {}
        }
    });
});

var app = builder.Build();

// Stripe config (optional if you're using it)
var stripeKey = builder.Configuration["Stripe:SecretKey"];
if (!string.IsNullOrEmpty(stripeKey))
    Stripe.StripeConfiguration.ApiKey = stripeKey;

// Enable middleware
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
