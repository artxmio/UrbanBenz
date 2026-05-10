using System.Windows;
using Mercedes.Data.Data;
using Mercedes.Data.Seeders;
using Mercedes.Services;

namespace Mercedes;

public partial class App : Application
{
    private readonly ISessionService _sessionService;
    private readonly ISettingsService _settingsService;

    public App()
    {
        _sessionService = SessionService.Instance;
        _settingsService = SettingsService.Instance;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using var dbContext = new AppDbContext();

        CarSeeder.Seed(dbContext);

        var rememberedEmail = _settingsService.GetRememberedEmail();
        if (!string.IsNullOrEmpty(rememberedEmail))
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Email == rememberedEmail && u.IsActive);
            if (user != null)
            {
                _sessionService.Login(user);
            }
        }
    }
}