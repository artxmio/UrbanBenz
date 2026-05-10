using System.Windows;
using System.Windows.Controls;
using Mercedes.Data.Data;
using Mercedes.Data.Models;
using Mercedes.Services;

namespace Mercedes.Pages;

public partial class ProfilePage : Page
{
    private bool _isEditing = false;
    private readonly ISessionService _sessionService;
    private readonly ISettingsService _settingsService;
    private readonly IValidationService _validationService;

    public ProfilePage()
    {
        InitializeComponent();
        _sessionService = SessionService.Instance;
        _settingsService = SettingsService.Instance;
        _validationService = ValidationService.Instance;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (!_sessionService.IsLoggedIn)
        {
            MessageBox.Show("Пожалуйста, войдите в аккаунт", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            NavigationService?.Navigate(new LoginPage());
            return;
        }

        LoadUserInfo();
    }

    private void LoadUserInfo()
    {
        var user = _sessionService.CurrentUser;
        if (user != null)
        {
            FirstNameTextBox.Text = user.FirstName;
            LastNameTextBox.Text = user.LastName;
            EmailTextBox.Text = user.Email;
            PhoneTextBox.Text = user.Phone ?? "Не указан";
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new MainPage());
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        _isEditing = true;
        
        FirstNameTextBox.IsReadOnly = false;
        LastNameTextBox.IsReadOnly = false;
        PhoneTextBox.IsReadOnly = false;
        
        FirstNameTextBox.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00A6FF"));
        LastNameTextBox.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00A6FF"));
        PhoneTextBox.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00A6FF"));
        
        EditButton.Visibility = Visibility.Collapsed;
        SaveButton.Visibility = Visibility.Visible;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        string firstName = FirstNameTextBox.Text.Trim();
        string lastName = LastNameTextBox.Text.Trim();
        string phone = PhoneTextBox.Text.Trim();

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

        var phoneError = _validationService.ValidatePhone(phone);
        if (phoneError != null)
        {
            MessageBox.Show(phoneError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var context = new AppDbContext();
            var user = context.Users.Find(_sessionService.CurrentUser!.Id);
            
            if (user != null)
            {
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Phone = string.IsNullOrEmpty(phone) ? null : phone;
                
                context.SaveChanges();
                
                _sessionService.Login(user);
                
                _isEditing = false;
                
                FirstNameTextBox.IsReadOnly = true;
                LastNameTextBox.IsReadOnly = true;
                PhoneTextBox.IsReadOnly = true;
                
                FirstNameTextBox.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E0E0E0"));
                LastNameTextBox.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E0E0E0"));
                PhoneTextBox.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E0E0E0"));
                
                EditButton.Visibility = Visibility.Visible;
                SaveButton.Visibility = Visibility.Collapsed;
                
                MessageBox.Show("Профиль успешно обновлён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _settingsService.ClearRememberedUser();
            _sessionService.Logout();
            NavigationService?.Navigate(new MainPage());
        }
    }
}