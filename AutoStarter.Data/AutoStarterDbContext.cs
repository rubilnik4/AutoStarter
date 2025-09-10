using AutoStarter.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoStarter.Data;

public class AutoStarterDbContext(DbContextOptions<AutoStarterDbContext> options) : DbContext(options)
{
    public DbSet<BrandEntity> Brands => Set<BrandEntity>();
    public DbSet<CarModelEntity> CarModels => Set<CarModelEntity>();
    public DbSet<ColorEntity> Colors => Set<ColorEntity>();
    public DbSet<TrimEntity> Trims => Set<TrimEntity>();
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<BrandEntity>().HasIndex(x => x.Name).IsUnique();
        b.Entity<CarModelEntity>().HasIndex(x => new { x.BrandId, x.Name }).IsUnique();
        b.Entity<OrderEntity>().HasIndex(x => x.OrderDate);
        b.Entity<OrderEntity>().HasIndex(x => x.CarModelId);
    }
}