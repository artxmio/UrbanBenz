using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Mercedes.Data.Data;
using Mercedes.Data.Models;
using Mercedes.Services;

namespace Mercedes.Pages;

public partial class CarDetailsPage : Page
{
    private readonly AppDbContext _context;
    private Car? _car;
    private Guid _carId;

    public CarDetailsPage(Guid carId)
    {
        InitializeComponent();
        _context = new AppDbContext();
        _carId = carId;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        LoadCarDetails();
    }

    private void LoadCarDetails()
    {
        try
        {
            _car = _context.Cars.Include(c => c.Images).FirstOrDefault(c => c.Id == _carId);
            if (_car == null)
            {
                MessageBox.Show("Автомобиль не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService?.GoBack();
                return;
            }

            DataContext = _car;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.GoBack();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show($"Редактирование автомобиля {_car?.Model}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (_car == null) return;

        var result = MessageBox.Show("Вы уверены, что хотите удалить этот автомобиль?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                _context.Cars.Remove(_car);
                _context.SaveChanges();
                MessageBox.Show("Автомобиль удалён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void TestDriveButton_Click(object sender, RoutedEventArgs e)
    {
        if (_car == null) return;

        var currentUser = SessionService.Instance.CurrentUser;
        if (currentUser == null)
        {
            MessageBox.Show("Для записи на тест-драйв необходимо войти в систему", "Требуется авторизация", MessageBoxButton.OK, MessageBoxImage.Warning);
            NavigationService?.Navigate(new LoginPage());
            return;
        }

        if (TestDriveDatePicker.SelectedDate == null)
        {
            MessageBox.Show("Пожалуйста, выберите дату", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var timeStr = (TestDriveTimeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        if (string.IsNullOrEmpty(timeStr))
        {
            MessageBox.Show("Пожалуйста, выберите время", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var timeParts = timeStr.Split(':');
        var hours = int.Parse(timeParts[0]);
        var minutes = int.Parse(timeParts[1]);

        var selectedDate = TestDriveDatePicker.SelectedDate.Value.Date.AddHours(hours).AddMinutes(minutes);
        
        if (selectedDate < DateTime.Now.AddHours(1))
        {
            MessageBox.Show("Пожалуйста, выберите дату и время не ранее чем через час", "Недопустимая дата", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Проверка, что время не занято
        var existingBooking = _context.TestDriveRequests
            .Where(t => t.CarId == _car.Id && t.Status != TestDriveStatus.Cancelled)
            .Where(t => t.RequestedDate.Date == selectedDate.Date)
            .Where(t => Math.Abs((t.RequestedDate - selectedDate).TotalHours) < 1)
            .FirstOrDefault();

        if (existingBooking != null)
        {
            MessageBox.Show("Это время уже занято. Пожалуйста, выберите другое.", "Время занято", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var testDriveRequest = new TestDriveRequest
            {
                CarId = _car.Id,
                UserId = currentUser.Id,
                RequestedDate = selectedDate,
                Notes = $"Тест-драйв автомобиля {_car.Model}",
                Status = TestDriveStatus.Pending
            };

            _context.TestDriveRequests.Add(testDriveRequest);
            await _context.SaveChangesAsync();

            var emailSent = await ServiceInstances.Email.SendTestDriveConfirmationAsync(
                currentUser.Email,
                $"{currentUser.FirstName} {currentUser.LastName}",
                _car.Model,
                selectedDate);

            if (emailSent)
            {
                MessageBox.Show($"Запись на тест-драйв успешно оформлена!\nДата: {selectedDate:dd.MM.yyyy HH:mm}\nПодтверждение отправлено на почту {currentUser.Email}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Запись на тест-драйв успешно оформлена!\nДата: {selectedDate:dd.MM.yyyy HH:mm}\nНе удалось отправить подтверждение на почту", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при записи на тест-драйв: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}