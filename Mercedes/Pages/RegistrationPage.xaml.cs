using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Mercedes.Data.Data;
using Mercedes.Data.Enums;
using Mercedes.Data.Models;
using Mercedes.Services;

namespace Mercedes.Pages;

public partial class RegistrationPage : Page
{
    private readonly AppDbContext _context;
    private readonly ISessionService _sessionService;
    private readonly IValidationService _validationService;

    public RegistrationPage()
    {
        InitializeComponent();
        _context = new AppDbContext();
        _sessionService = SessionService.Instance;
        _validationService = ValidationService.Instance;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService?.CanGoBack == true)
            NavigationService?.GoBack();
    }

    private void LoginLink_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        NavigationService?.Navigate(new LoginPage());
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        string firstName = FirstNameTextBox?.Text.Trim() ?? string.Empty;
        string lastName = LastNameTextBox?.Text.Trim() ?? string.Empty;
        string email = EmailTextBox?.Text.Trim() ?? string.Empty;
        string phone = PhoneTextBox?.Text.Trim() ?? string.Empty;
        string password = PasswordBox?.Password ?? string.Empty;
        string confirmPassword = ConfirmPasswordBox?.Password ?? string.Empty;

        var firstNameError = _validationService.ValidateName(firstName, "Имя");
        if (firstNameError != null)
        {
            MessageBox.Show(firstNameError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var lastNameError = _validationService.ValidateName(lastName, "Фамилия");
        if (lastNameError != null)
        {
            MessageBox.Show(lastNameError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var emailError = _validationService.ValidateEmail(email);
        if (emailError != null)
        {
            MessageBox.Show(emailError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var phoneError = _validationService.ValidatePhone(phone);
        if (phoneError != null)
        {
            MessageBox.Show(phoneError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var passwordError = _validationService.ValidatePassword(password);
        if (passwordError != null)
        {
            MessageBox.Show(passwordError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (password != confirmPassword)
        {
            MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_context.Users.Any(u => u.Email == email))
        {
            MessageBox.Show("Пользователь с таким email уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = string.IsNullOrEmpty(phone) ? null : phone,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Role = Role.User
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            _sessionService.Login(user);

            MessageBox.Show("Регистрация успешна! Добро пожаловать!", "Успех", 
                MessageBoxButton.OK, MessageBoxImage.Information);

            NavigationService?.Navigate(new MainPage());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}