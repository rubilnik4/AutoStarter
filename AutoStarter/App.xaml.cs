
using System.Windows;
using AutoStarter.Application;
using AutoStarter.Data;
using AutoStarter.Modules.Sales;
using Microsoft.Extensions.Configuration;

namespace AutoStarter;

public partial class App : PrismApplication
{
    protected override Window CreateShell() => Container.Resolve<MainWindow>();

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance(BuildConfiguration());
    }

    
    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<DataModule>();
        moduleCatalog.AddModule<ApplicationModule>();
        moduleCatalog.AddModule<SalesModule>();
    }
    
    private static IConfiguration BuildConfiguration()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();
    }
}