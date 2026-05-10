namespace Mercedes.Services;

public interface ISettingsService
{
    void SaveRememberedUser(string email);
    string? GetRememberedEmail();
    void ClearRememberedUser();
}