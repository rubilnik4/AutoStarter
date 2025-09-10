using AutoStarter.Models.Domain;
using ClosedXML.Excel;

namespace AutoStarter.Application.Services;

public sealed class ExcelExportService : IExcelExportService
{
     public async Task<string> ExportReport(int year, string? modelName, IReadOnlyList<MonthlyModelSales> monthlySales)
    {
        var path = BuildOutputPath(year, modelName);

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Отчёт");

        WriteHeader(ws);
        WriteRows(ws, monthlySales);
        ApplyNumberFormat(ws);
        HighlightBigCells(ws, threshold: 25_000_000);
        ws.Columns().AdjustToContents();

        await SaveAsync(wb, path);
        return path;
    }

    private static string BuildOutputPath(int year, string? modelName)
    {
        var safeModel = string.IsNullOrWhiteSpace(modelName) ? "" : $"_{modelName}";
        var fileName = $"Отчёт_продажи_{year}{safeModel}.xlsx";
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
    }
    
    private static void WriteHeader(IXLWorksheet ws)
    {
        var headers = new[]
        {
            "Модель","Янв","Фев","Мар","Апр","Май","Июн",
            "Июл","Авг","Сен","Окт","Ноя","Дек"
        };

        for (var i = 0; i < headers.Length; i++)
            ws.Cell(1, i + 1).Value = headers[i];

        ws.Row(1).Style.Font.Bold = true;
    }

    private static void WriteRows(IXLWorksheet ws, IReadOnlyList<MonthlyModelSales> rows)
    {
        var r = 2;
        foreach (var sum in rows)
        {
            ws.Cell(r, 1).Value = sum.ModelName;
           
            ws.Cell(r,  2).Value = sum.M01; ws.Cell(r,  3).Value = sum.M02; ws.Cell(r,  4).Value = sum.M03;
            ws.Cell(r,  5).Value = sum.M04; ws.Cell(r,  6).Value = sum.M05; ws.Cell(r,  7).Value = sum.M06;
            ws.Cell(r,  8).Value = sum.M07; ws.Cell(r,  9).Value = sum.M08; ws.Cell(r, 10).Value = sum.M09;
            ws.Cell(r, 11).Value = sum.M10; ws.Cell(r, 12).Value = sum.M11; ws.Cell(r, 13).Value = sum.M12;

            r++;
        }
    }

    private static void ApplyNumberFormat(IXLWorksheet ws)
    {
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
        if (lastRow <= 1) return;
        
        var data = ws.Range(2, 2, lastRow, 13);
        data.Style.NumberFormat.Format = "#,##0";
        data.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
    }

    private static void HighlightBigCells(IXLWorksheet ws, decimal threshold)
    {
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
        if (lastRow <= 1) return;

        var data = ws.Range(2, 2, lastRow, 13);
        foreach (var cell in data.Cells())
        {
            if (!cell.TryGetValue(out double value)) continue;
            if ((decimal)value <= threshold) continue;

            cell.Style.Fill.BackgroundColor = XLColor.LightPink;
            cell.Style.Font.FontColor = XLColor.DarkRed;
            cell.Style.Font.Bold = true;
        }
    }

    private static async Task SaveAsync(XLWorkbook wb, string path, CancellationToken ct = default)
    {
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        ms.Position = 0;
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllBytesAsync(path, ms.ToArray(), ct);
    }
}