namespace AutoStarter.Data.Entities;

public class OrderEntity
{
    public int Id { get; init; }
    public int CarModelId { get; init; }
    public CarModelEntity CarModel { get; init; } = null!;
    public int ColorId { get; init; }
    public int TrimId { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal Price { get; init; }
}