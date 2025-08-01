﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Domain.Enums;

namespace TravelEase.TravelEase.Infrastructure.Data;

public class TravelEaseDbContext : DbContext
{
    public TravelEaseDbContext(DbContextOptions<TravelEaseDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<HotelView> HotelViews { get; set; }
    public DbSet<HotelImage> HotelImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Hotel>()
            .HasMany(h => h.Images)
            .WithOne(i => i.Hotel)
            .HasForeignKey(i => i.HotelId)
            .OnDelete(DeleteBehavior.Cascade);
        // Relationships
        modelBuilder.Entity<City>()
            .HasMany(c => c.Hotels)
            .WithOne(h => h.City)
            .HasForeignKey(h => h.CityId);

        modelBuilder.Entity<Hotel>()
            .HasMany(h => h.Rooms)
            .WithOne(r => r.Hotel)
            .HasForeignKey(r => r.HotelId);

        modelBuilder.Entity<Hotel>()
            .HasMany(h => h.Reviews)
            .WithOne(r => r.Hotel)
            .HasForeignKey(r => r.HotelId);

        modelBuilder.Entity<Room>()
            .HasMany(r => r.Bookings)
            .WithOne(b => b.Room)
            .HasForeignKey(b => b.RoomId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Discount>()
            .HasOne(d => d.Hotel)
            .WithMany()
            .HasForeignKey(d => d.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Discount>()
            .HasOne(d => d.Room)
            .WithMany()
            .HasForeignKey(d => d.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // Enum conversion
        modelBuilder.Entity<Booking>()
            .Property(b => b.PaymentMethod)
            .HasConversion(new EnumToStringConverter<PaymentMethod>());

        // Decimal and string field constraints
        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Room>()
            .Property(r => r.PricePerNight)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Booking>()
            .Property(b => b.PaymentStatus)
            .HasMaxLength(20);
    }

}