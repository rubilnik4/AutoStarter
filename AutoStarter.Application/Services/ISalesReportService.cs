using AutoStarter.Models.Domain;

namespace AutoStarter.Application.Services;

public interface ISalesReportService
{
    Task<IReadOnlyList<CarModel>> GetAllModels();
    Task<IReadOnlyList<MonthlyModelSales>> GetMonthlyModelSums(int year, int? modelId);
}