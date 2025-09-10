using AutoStarter.Application.Services;
using AutoStarter.Data;

namespace AutoStarter.Application;

[ModuleDependency(nameof(DataModule))]
public class ApplicationModule : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<ISalesReportService, SalesReportService>();
        containerRegistry.Register<IExcelExportService, ExcelExportService>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    { }
}