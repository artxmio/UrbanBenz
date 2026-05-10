using Mercedes.Services;
using System;
using System.Windows.Markup;

namespace Mercedes.Extensions;

public abstract class ServiceExtension<T> : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return GetInstance();
    }

    protected abstract T GetInstance();
}

[MarkupExtensionReturnType(typeof(ISessionService))]
public class SessionServiceExtension : ServiceExtension<ISessionService>
{
    protected override ISessionService GetInstance()
    {
        return SessionService.Instance;
    }
}

[MarkupExtensionReturnType(typeof(ISettingsService))]
public class SettingsServiceExtension : ServiceExtension<ISettingsService>
{
    protected override ISettingsService GetInstance()
    {
        return SettingsService.Instance;
    }
}

[MarkupExtensionReturnType(typeof(IValidationService))]
public class ValidationServiceExtension : ServiceExtension<IValidationService>
{
    protected override IValidationService GetInstance()
    {
        return ValidationService.Instance;
    }
}