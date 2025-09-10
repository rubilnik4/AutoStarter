namespace AutoStarter.Models.Domain;

public sealed record MonthlyModelSales(
    string ModelName,
    decimal M01, decimal M02, decimal M03, decimal M04, decimal M05, decimal M06,
    decimal M07, decimal M08, decimal M09, decimal M10, decimal M11, decimal M12);