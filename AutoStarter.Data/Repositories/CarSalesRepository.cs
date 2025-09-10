using AutoStarter.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AutoStarter.Data.Repositories;

public sealed class CarSalesRepository(AutoStarterDbContext db) : ICarSalesRepository
{
    public async Task<IReadOnlyList<CarModel>> GetAllModels() =>
        await db.CarModels
            .AsNoTracking()
            .Include(m => m.Brand)
            .OrderBy(m => m.Brand.Name).ThenBy(m => m.Name)
            .Select(m => new CarModel(m.Id, new Brand(m.Brand.Id, m.Brand.Name), m.Name))
            .ToListAsync();

    public async Task<IReadOnlyList<MonthlySales>> GetMonthlySums(int year, int? modelId)
    {
        var cars = db.CarModels.AsNoTracking()
            .Include(m => m.Brand)
            .Where(m => modelId == null || m.Id == modelId)
            .Select(m => new { m.Id, Display = m.Brand.Name + " " + m.Name });
        
        var sums = db.Orders.AsNoTracking()
            .Where(o => o.OrderDate.Year == year)
            .GroupBy(o => new { o.CarModelId, o.OrderDate.Month })
            .Select(g => new { g.Key.CarModelId, g.Key.Month, Sum = g.Sum(x => x.Price) });

        var result =
            from sum in sums
            join car in cars on sum.CarModelId equals car.Id
            orderby car.Display, sum.Month
            select new MonthlySales(sum.CarModelId, car.Display, sum.Month, sum.Sum);

        return await result.ToListAsync();
    }
}