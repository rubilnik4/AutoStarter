using System.Collections.ObjectModel;
using AutoStarter.Application.Services;
using AutoStarter.Models.Domain;

namespace AutoStarter.Modules.Sales.ViewModels;

public sealed class SalesViewModel : BindableBase, INavigationAware
{
    public SalesViewModel(ISalesReportService reportService, IExcelExportService excel)
    {
        _reportService = reportService;
        _excel = excel;
       
        RefreshCommand = new AsyncDelegateCommand(Load, () => !IsBusy).ObservesProperty(() => IsBusy);
        ExportCommand = new AsyncDelegateCommand(Export, () => !IsBusy).ObservesProperty(() => IsBusy);
    }
    
    private readonly ISalesReportService _reportService;
    private readonly IExcelExportService _excel;
    
    public ObservableCollection<int> Years { get; } = new(BuildYears(6));
    public ObservableCollection<CarModel> CarModels { get; } = [];
    public ObservableCollection<MonthlyModelSales> MonthlyModelSales { get; } = [];

    public AsyncDelegateCommand RefreshCommand { get; }
    public AsyncDelegateCommand ExportCommand  { get; }

    private int _selectedYear =  DateTime.Now.Year;
    public int SelectedYear{ get => _selectedYear; set => SetProperty(ref _selectedYear, value);}

    private CarModel? _selectedCarModel;
    public CarModel? SelectedCarModel{ get => _selectedCarModel; set => SetProperty(ref _selectedCarModel, value); }
    
    private bool _isBusy;
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public async void OnNavigatedTo(NavigationContext context)
    {
        try
        {
            IsBusy = true;
            CarModels.Clear();
            var models = await _reportService.GetAllModels();
            CarModels.AddRange(models);
            await Load();
        }
        finally { IsBusy = false; }
    }
    public bool IsNavigationTarget(NavigationContext context) => true;
    public void OnNavigatedFrom(NavigationContext context) { }

    private async Task Load()
    {
        try
        {
            IsBusy = true;
            MonthlyModelSales.Clear();
            var monthlyModels = await _reportService.GetMonthlyModelSums(SelectedYear, SelectedCarModel?.Id);
            MonthlyModelSales.AddRange(monthlyModels);
        }
        finally { IsBusy = false; }
    }

    private async Task Export()
    {
        try
        {
            IsBusy = true;
            var path = await _excel.ExportReport(SelectedYear, SelectedCarModel?.Name, MonthlyModelSales.ToList());
            System.Windows.MessageBox.Show($"Экспортировано: {path}");
        }
        finally { IsBusy = false; }
    }

    private static List<int> BuildYears(int count) =>
        Enumerable.Range(0, count)
            .Select(offset => DateTime.Now.Year - offset)
            .ToList();
}