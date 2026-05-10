using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Mercedes.Data.Data;
using Mercedes.Data.Models;
using Mercedes.Services;

namespace Mercedes.Pages;

public partial class AdminPage : Page
{
    private readonly AppDbContext _context;
    private readonly ISessionService _sessionService;
    private readonly ISettingsService _settingsService;

    public AdminPage()
    {
        InitializeComponent();
        _context = new AppDbContext();
        _sessionService = SessionService.Instance;
        _settingsService = SettingsService.Instance;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var currentUser = _sessionService.CurrentUser;
        if (currentUser == null || currentUser.Role != Data.Enums.Role.Admin)
        {
            MessageBox.Show("Доступ запрещён", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            NavigationService?.GoBack();
            return;
        }

        AdminNameText.Text = $"{currentUser.FirstName} {currentUser.LastName}";
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            // Загрузка пользователей
            var users = _context.Users.OrderByDescending(u => u.CreatedAt).ToList();
            UsersDataGrid.ItemsSource = users;

            // Загрузка автомобилей
            var cars = _context.Cars.OrderByDescending(c => c.CreatedAt).ToList();
            CarsDataGrid.ItemsSource = cars;

            // Загрузка записей на тест-драйв
            var testDrives = _context.TestDriveRequests
                .Include(t => t.User)
                .Include(t => t.Car)
                .OrderByDescending(t => t.RequestedDate)
                .ToList();
            TestDrivesDataGrid.ItemsSource = testDrives;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new MainPage());
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

    // Пользователи
    private void EditUser_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid userId)
        {
            MessageBox.Show($"Редактирование пользователя {userId}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void DeleteUser_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid userId)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить этого пользователя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var user = _context.Users.Find(userId);
                    if (user != null)
                    {
                        _context.Users.Remove(user);
                        _context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Пользователь удалён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void SaveUsers_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (UsersDataGrid.ItemsSource is List<User> users)
            {
                foreach (var user in users)
                {
                    var existingUser = _context.Users.Find(user.Id);
                    if (existingUser != null)
                    {
                        existingUser.FirstName = user.FirstName;
                        existingUser.LastName = user.LastName;
                        existingUser.Email = user.Email;
                        existingUser.Phone = user.Phone;
                        existingUser.IsActive = user.IsActive;
                    }
                }
                _context.SaveChanges();
                MessageBox.Show("Изменения сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Автомобили
    private void AddCar_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new AddCarPage());
    }

    private void EditCar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid carId)
        {
            NavigationService?.Navigate(new CarDetailsPage(carId));
        }
    }

    private void DeleteCar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid carId)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить этот автомобиль?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var car = _context.Cars.Find(carId);
                    if (car != null)
                    {
                        _context.Cars.Remove(car);
                        _context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Автомобиль удалён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void SaveCars_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CarsDataGrid.ItemsSource is List<Car> cars)
            {
                foreach (var car in cars)
                {
                    var existingCar = _context.Cars.Find(car.Id);
                    if (existingCar != null)
                    {
                        existingCar.Model = car.Model;
                        existingCar.Year = car.Year;
                        existingCar.Price = car.Price;
                        existingCar.VinNumber = car.VinNumber;
                        existingCar.IsAviable = car.IsAviable;
                    }
                }
                _context.SaveChanges();
                MessageBox.Show("Изменения сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Записи на тест-драйв
    private void ConfirmTestDrive_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid requestId)
        {
            try
            {
                var request = _context.TestDriveRequests.Find(requestId);
                if (request != null)
                {
                    request.Status = TestDriveStatus.Confirmed;
                    _context.SaveChanges();
                    LoadData();
                    MessageBox.Show("Запись подтверждена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void CancelTestDrive_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid requestId)
        {
            var result = MessageBox.Show("Вы уверены, что хотите отменить эту запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var request = _context.TestDriveRequests.Find(requestId);
                    if (request != null)
                    {
                        request.Status = TestDriveStatus.Cancelled;
                        _context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Запись отменена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}