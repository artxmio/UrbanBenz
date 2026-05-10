using System.Text.RegularExpressions;

namespace Mercedes.Services;

public class ValidationService : IValidationService
{
    private static readonly ValidationService _instance = new();
    public static ValidationService Instance => _instance;

    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled);

    private static readonly Regex NameRegex = new(
        @"^[a-zA-Zа-яА-ЯёЁ\s'-]{1,100}$",
        RegexOptions.Compiled);

    private static readonly Regex PhoneRegex = new(
        @"^\+?[0-9\s\-()]{7,20}$",
        RegexOptions.Compiled);

    private static readonly Regex PasswordRegex = new(
        @"^.{6,}$",
        RegexOptions.Compiled);

    public string? ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Email обязателен";
        
        if (!EmailRegex.IsMatch(email))
            return "Некорректный формат email";
        
        return null;
    }

    public string? ValidateName(string name, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(name))
            return $"{fieldName} обязательно";
        
        if (!NameRegex.IsMatch(name))
            return $"Некорректный формат {fieldName.ToLower()}";
        
        return null;
    }

    public string? ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return null;
        
        if (!PhoneRegex.IsMatch(phone))
            return "Некорректный формат телефона";
        
        return null;
    }

    public string? ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return "Пароль обязателен";
        
        if (!PasswordRegex.IsMatch(password))
            return "Пароль должен содержать минимум 6 символов";
        
        return null;
    }
}