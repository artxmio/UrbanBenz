using System.Windows;
using System.Windows.Controls;
using Mercedes.Data.Data;
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
}