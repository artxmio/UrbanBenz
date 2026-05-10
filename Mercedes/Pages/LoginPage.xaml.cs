using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Mercedes.Data.Data;
using Mercedes.Data.Models;
using Mercedes.Services;

namespace Mercedes.Pages;

public partial class LoginPage : Page
{
    private readonly AppDbContext _context;
    private readonly ISessionService _sessionService;
    private readonly ISettingsService _settingsService;
    private readonly IValidationService _validationService;

    public LoginPage()
    {
        InitializeComponent();
        _context = new AppDbContext();
        _sessionService = SessionService.Instance;
        _settingsService = SettingsService.Instance;
        _validationService = ValidationService.Instance;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService?.CanGoBack == true)
            NavigationService?.GoBack();
    }

    private void RegisterLink_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        NavigationService?.Navigate(new RegistrationPage());
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string email = EmailTextBox?.Text.Trim() ?? string.Empty;
        string password = PasswordBox?.Password ?? string.Empty;

        var emailError = _validationService.ValidateEmail(email);
        if (emailError != null)
        {
            MessageBox.Show(emailError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var passwordError = _validationService.ValidatePassword(password);
        if (passwordError != null)
        {
            MessageBox.Show(passwordError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.IsActive);

            if (user == null)
            {
                MessageBox.Show("Пользователь с таким email не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string passwordHash = HashPassword(password);

            if (user.PasswordHash != passwordHash)
            {
                MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _sessionService.Login(user);

            if (RememberCheckBox?.IsChecked == true)
                _settingsService.SaveRememberedUser(email);
            else
                _settingsService.ClearRememberedUser();

            MessageBox.Show($"Добро пожаловать, {user.FirstName}!", "Успех", 
                MessageBoxButton.OK, MessageBoxImage.Information);

            // Перенаправление на админ-панель для админов
            if (user.Role == Data.Enums.Role.Admin)
                NavigationService?.Navigate(new AdminPage());
            else
                NavigationService?.Navigate(new MainPage());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка", 
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