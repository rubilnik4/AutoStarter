using AutoStarter.Models.Domain;

namespace AutoStarter.Application.Services;

public interface IExcelExportService
{
    Task<string> ExportReport(int year, string? modelName, IReadOnlyList<MonthlyModelSales> monthlySales);
}