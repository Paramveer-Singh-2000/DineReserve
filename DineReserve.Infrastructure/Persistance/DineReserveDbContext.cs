using DineReserve.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DineReserve.Infrastructure.Persistance;

public class DineReserveDbContext(DbContextOptions<DineReserveDbContext> options)  : DbContext(options)
{
    internal DbSet<Restaurant>  Restaurants { get; set; }
    internal DbSet<Dish> Dishes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Restaurant>().OwnsOne(r => r.Address);
        modelBuilder.Entity<Restaurant>().HasMany(r => r.Dishes).WithOne().HasForeignKey(r => r.RestaurantId);
    }
}