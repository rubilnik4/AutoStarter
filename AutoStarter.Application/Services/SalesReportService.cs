using AutoStarter.Data.Repositories;
using AutoStarter.Models.Domain;

namespace AutoStarter.Application.Services;

public sealed class SalesReportService(ICarSalesRepository repository): ISalesReportService
{
    public Task<IReadOnlyList<CarModel>> GetAllModels() =>
        repository.GetAllModels();

    public async Task<IReadOnlyList<MonthlyModelSales>> GetMonthlyModelSums(int year, int? modelId)
    {
        var monthlySums = await repository.GetMonthlySums(year, modelId);
       
        var result = monthlySums
            .GroupBy(sum => new { sum.CarModelId, sum.CarModelName })
            .Select(g => MapToMonthlyModelSales(g.Key.CarModelName, g))
            .OrderBy(r => r.ModelName)
            .ToList();

        return result;
    }
    
    private static MonthlyModelSales MapToMonthlyModelSales(string modelName, IEnumerable<MonthlySales> salesByModel)
    {
        var monthTotals = new decimal[13];
        foreach (var item in salesByModel)
            monthTotals[item.Month] = item.Sum;

        return new MonthlyModelSales(
            modelName,
            monthTotals[1], monthTotals[2], monthTotals[3], monthTotals[4],
            monthTotals[5], monthTotals[6], monthTotals[7], monthTotals[8],
            monthTotals[9], monthTotals[10], monthTotals[11], monthTotals[12]
        );
    }
}