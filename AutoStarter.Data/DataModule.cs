using AutoStarter.Configuration;
using AutoStarter.Configuration.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AutoStarter.Data;

public class DataModule : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<AutoStarterDbContext>(provider =>
        {
            var config = provider.Resolve<IConfiguration>();
            var settings = ConfigLoader.Load<DbSettings>(config, "Database");
            var connectionString = settings.ConnectionString;

            var builder = new DbContextOptionsBuilder<AutoStarterDbContext>();
            builder.UseSqlite(connectionString);

            return new AutoStarterDbContext(builder.Options);
        });
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        using var scope = containerProvider.CreateScope();
        var ctx = scope.Resolve<AutoStarterDbContext>();
        ctx.Database.Migrate();
        //Seed.DataSeeder.SeedIfEmpty(ctx);
    }
}