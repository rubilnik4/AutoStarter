using AutoStarter.Application;
using AutoStarter.Application.Services;
using AutoStarter.Modules.Sales.ViewModels;
using AutoStarter.Modules.Sales.Views;

namespace AutoStarter.Modules.Sales;

[ModuleDependency(nameof(ApplicationModule))]
public class SalesModule(IRegionManager regionManager) : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        regionManager.RequestNavigate("MainRegion", nameof(SalesView));
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<SalesView, SalesViewModel>();
        containerRegistry.Register<IExcelExportService, ExcelExportService>();
    }
}