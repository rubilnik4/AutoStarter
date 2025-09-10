using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AutoStarter.Data;

public sealed class AutoStarterDesignTimeFactory: IDesignTimeDbContextFactory<AutoStarterDbContext>
{
    public AutoStarterDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AutoStarterDbContext>()
            .UseSqlite("Data Source=dealer.db")
            .Options;

        return new AutoStarterDbContext(options);
    }
}