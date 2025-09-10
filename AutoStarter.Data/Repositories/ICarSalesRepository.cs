using AutoStarter.Models.Domain;

namespace AutoStarter.Data.Repositories;

public interface ICarSalesRepository
{
    Task<IReadOnlyList<CarModel>> GetAllModels();
    Task<IReadOnlyList<MonthlySales>> GetMonthlySums(int year, int? modelId);
}