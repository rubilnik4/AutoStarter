namespace AutoStarter.Modules.Sales;

public class SalesModule(IRegionManager regionManager) : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        regionManager.RequestNavigate("MainRegion", nameof(Views.SalesView));
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<Views.SalesView>();
        //containerRegistry.Register<IExcelExportService, ExcelExportService>();
    }
}