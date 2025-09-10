namespace AutoStarter.Models.Domain;

public sealed record MonthlySales(int CarModelId, string CarModelName, int Month, decimal Sum);