using System.Windows;
using UrbanBenz.Data.Data;
using UrbanBenz.Data.Seeders;

namespace UrbanBenz;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using var dbContext = new AppDbContext();

        CarSeeder.Seed(dbContext);
    }
}
