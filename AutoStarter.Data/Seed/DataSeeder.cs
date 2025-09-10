using AutoStarter.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoStarter.Data.Seed;

public static class DataSeeder
{
    public static async Task Seed(AutoStarterDbContext db, int targetOrders = 1400, int? randomSeed = 42)
    {
        if (await db.Orders.AsNoTracking().AnyAsync()) return;

        var brands = await SeedBrands(db);
        var models = await SeedModels(db, brands);
        var lookups = await SeedLookups(db);
        await SeedOrders(db, models, lookups.colors, lookups.trims, targetOrders, randomSeed);
    }

    private static async Task<List<BrandEntity>> SeedBrands(AutoStarterDbContext db)
    {
        var brands = new[]
        {
            new BrandEntity { Name = "Audi" },
            new BrandEntity { Name = "BMW" },
            new BrandEntity { Name = "Mercedes" },
            new BrandEntity { Name = "Toyota" },
            new BrandEntity { Name = "Volkswagen" }
        }.ToList();

        await db.Brands.AddRangeAsync(brands);
        await db.SaveChangesAsync();
        return brands;
    }

    private static async Task<List<CarModelEntity>> SeedModels(AutoStarterDbContext db, List<BrandEntity> brands)
    {
        var models = new List<CarModelEntity>();

        Add("Audi", "A3", "A4", "A6", "Q3", "Q5");
        Add("BMW", "1", "3", "5", "X2", "X3");
        Add("Mercedes", "A200", "C200", "E200", "GLA", "GLC");
        Add("Toyota", "Corolla", "Camry", "RAV4", "Highlander");
        Add("Volkswagen", "Golf", "Passat", "Tiguan", "Touareg");

        await db.CarModels.AddRangeAsync(models);
        await db.SaveChangesAsync();
        return models;

        void Add(string brand, params string[] names)
        {
            var b = brands.First(x => x.Name == brand);
            models.AddRange(names.Select(n => new CarModelEntity { Name = n, BrandId = b.Id }));
        }
    }

    private static async Task<(List<ColorEntity> colors, List<TrimEntity> trims)> SeedLookups(AutoStarterDbContext db)
    {
        var colors = new[] { "Белый", "Чёрный", "Серый", "Синий", "Красный" }
            .Select(c => new ColorEntity { Name = c }).ToList();

        var trims = new[] { "Base", "Style", "Sport", "Luxury" }
            .Select(t => new TrimEntity { Name = t }).ToList();

        await db.Colors.AddRangeAsync(colors);
        await db.Trims.AddRangeAsync(trims);
        await db.SaveChangesAsync();

        return (colors, trims);
    }
   
    private static async Task SeedOrders(
        AutoStarterDbContext db, List<CarModelEntity> models, List<ColorEntity> colors,
        List<TrimEntity> trims, int targetOrders, int? randomSeed)
    {
        var rnd = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
        var end = DateTime.Today;
        var start = end.AddYears(-5);
       
        const int batchSize = 500;
        var left = targetOrders;

        while (left > 0)
        {
            var take = Math.Min(batchSize, left);
            var batch = new List<OrderEntity>(take);

            for (var i = 0; i < take; i++)
            {
                var model = models[rnd.Next(models.Count)];
                var brandName = 
                    await db.Brands.AsNoTracking()
                        .Where(b => b.Id == model.BrandId)
                        .Select(b => b.Name)
                        .FirstAsync();

                var date = RandomDateWeighted(rnd, start, end, MonthWeight);
                var (min, max) = PriceRange(brandName);

                var basePrice = rnd.Next(min, max);
                var coef = 1m + (decimal)rnd.NextDouble() * 0.4m;
                var price = Math.Round(basePrice * coef, 0);

                batch.Add(new OrderEntity
                {
                    CarModelId = model.Id,
                    ColorId = colors[rnd.Next(colors.Count)].Id,
                    TrimId = trims[rnd.Next(trims.Count)].Id,
                    OrderDate = date,
                    Price = price
                });
            }

            await db.Orders.AddRangeAsync(batch);
            await db.SaveChangesAsync();

            left -= take;
        }
    }
   
    private static decimal MonthWeight(int m) => m switch
    {
        1 => 0.7m,  2 => 0.9m,  3 => 1.1m,  4 => 1.15m,
        5 => 1.1m,  6 => 1.0m,  7 => 0.85m, 8 => 0.9m,
        9 => 1.2m, 10 => 1.25m, 11 => 1.1m, 12 => 1.0m,
        _ => 1.0m
    };

    private static (int min, int max) PriceRange(string brand) => brand switch
    {
        "Toyota" or "Volkswagen" => (1_800_000, 4_000_000),
        "Audi" or "BMW"          => (3_500_000, 7_500_000),
        "Mercedes"               => (3_800_000, 8_500_000),
        _                        => (2_000_000, 5_000_000),
    };

    private static DateTime RandomDateWeighted(Random rnd, DateTime start, DateTime end, Func<int, decimal> monthWeight)
    {
        var year = rnd.Next(start.Year, end.Year + 1);

        var months = Enumerable.Range(1, 12)
            .Where(m =>
            {
                var first = new DateTime(year, m, 1);
                var last  = first.AddMonths(1).AddDays(-1);
                return last >= start && first <= end;
            })
            .Select(m => (m, w: (double)monthWeight(m)))
            .ToList();

        var sumW = months.Sum(x => x.w);
        var roll = rnd.NextDouble() * sumW;
        var month = months.First(x => (roll -= x.w) <= 0).m;

        var day = rnd.Next(1, DateTime.DaysInMonth(year, month) + 1);
        var dt  = new DateTime(year, month, day);

        if (dt < start) dt = start;
        if (dt > end)   dt = end;

        return dt.AddHours(rnd.Next(0, 24)).AddMinutes(rnd.Next(0, 60));
    }
}