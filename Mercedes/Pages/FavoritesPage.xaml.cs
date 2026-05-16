using System.Windows;
using System.Windows.Controls;
using Mercedes.Data.Data;
using Mercedes.Data.Enums;
using Mercedes.Data.Models;
using Mercedes.Services;
using Microsoft.EntityFrameworkCore;

namespace Mercedes.Pages;

public partial class FavoritesPage : Page
{
    private readonly AppDbContext _context;

    public FavoritesPage()
    {
        InitializeComponent();
        _context = new AppDbContext();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        LoadFavorites();
    }

    private void LoadFavorites()
    {
        try
        {
            var currentUser = SessionService.Instance.CurrentUser;
            if (currentUser == null)
            {
                MessageBox.Show("Для просмотра избранного необходимо войти в систему", "Требуется авторизация", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService?.GoBack();
                return;
            }

            var favorites = _context.Favorites
                .Include(f => f.Car)
                .Where(f => f.UserId == currentUser.Id)
                .OrderByDescending(f => f.CreatedAt)
                .ToList();

            if (favorites.Count == 0)
            {
                EmptyMessage.Visibility = Visibility.Visible;
                FavoritesItemsControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyMessage.Visibility = Visibility.Collapsed;
                FavoritesItemsControl.Visibility = Visibility.Visible;
                FavoritesItemsControl.ItemsSource = favorites;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки избранного: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.GoBack();
    }

    private void CarCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Border border && border.Tag is Guid carId)
        {
            NavigationService?.Navigate(new CarDetailsPage(carId));
        }
    }

    private void RemoveFromFavorites_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid carId)
        {
            var result = MessageBox.Show("Удалить этот автомобиль из избранного?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var currentUser = SessionService.Instance.CurrentUser;
                    if (currentUser == null) return;

                    var favorite = _context.Favorites
                        .FirstOrDefault(f => f.UserId == currentUser.Id && f.CarId == carId);

                    if (favorite != null)
                    {
                        _context.Favorites.Remove(favorite);
                        _context.SaveChanges();
                        LoadFavorites();
                        MessageBox.Show("Автомобиль удалён из избранного", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void BuyCar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid carId)
        {
            try
            {
                var currentUser = SessionService.Instance.CurrentUser;
                if (currentUser == null)
                {
                    MessageBox.Show("Для покупки необходимо войти в систему", "Требуется авторизация", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var car = _context.Cars.FirstOrDefault(c => c.Id == carId);
                if (car == null)
                {
                    MessageBox.Show("Автомобиль не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!car.IsAviable)
                {
                    MessageBox.Show("Этот автомобиль уже продан", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadFavorites();
                    return;
                }

                var result = MessageBox.Show(
                    $"Вы уверены, что хотите приобрести {car.Model} за {car.Price:N0}?\n\nДля подтверждения покупки с вами свяжется менеджер.",
                    "Подтверждение покупки",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Создаём запись о продаже
                    var sale = new Sale
                    {
                        UserId = currentUser.Id,
                        CarId = car.Id,
                        Price = car.Price,
                        Status = SaleStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Sales.Add(sale);

                    // Помечаем автомобиль как проданный
                    car.IsAviable = false;

                    _context.SaveChanges();

                    MessageBox.Show(
                        $"Спасибо за покупку {car.Model}!\n\nМенеджер свяжется с вами в ближайшее время для уточнения деталей.",
                        "Покупка оформлена",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    LoadFavorites();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении покупки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}