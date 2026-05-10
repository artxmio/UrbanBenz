namespace Mercedes.Services;

public interface IValidationService
{
    string? ValidateEmail(string email);
    string? ValidateName(string name, string fieldName);
    string? ValidatePhone(string phone);
    string? ValidatePassword(string password);
}