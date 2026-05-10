namespace Mercedes.Services;

public static class ServiceInstances
{
    public static ISessionService Session { get; } = SessionService.Instance;
    public static ISettingsService Settings { get; } = SettingsService.Instance;
    public static IValidationService Validation { get; } = ValidationService.Instance;
    public static IEmailService Email { get; } = new EmailService();
}