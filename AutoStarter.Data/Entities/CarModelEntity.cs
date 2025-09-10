namespace AutoStarter.Data.Entities;

public class CarModelEntity
{
    public int Id { get; init; } 
    public required string Name { get; init; } 
    public int BrandId { get; init; } 
    public BrandEntity Brand { get; init; } = null!;
}