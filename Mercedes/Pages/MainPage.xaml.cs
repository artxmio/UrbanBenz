using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using Mercedes.Data;
using Mercedes.Data.Data;
using Mercedes.Data.Enums;
using Mercedes.Data.Models;
using Mercedes.Services;

namespace Mercedes.Pages;

public partial class MainPage : Page
{
    private readonly AppDbContext _context;
    private readonly ISessionService _sessionService;
    private readonly ISettingsService _settingsService;
    private List<Car> _allCars = new();

    public MainPage()
    {
        InitializeComponent();
        _context = new AppDbContext();
        _sessionService = SessionService.Instance;
        _settingsService = SettingsService.Instance;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            UpdateUserInfo();
            LoadFilters();
            await LoadCarsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке страницы: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateUserInfo()
    {
        if (UserNameText != null)
        {
            UserNameText.Text = _sessionService.IsLoggedIn 
                ? _sessionService.CurrentUser!.FirstName 
                : "Войти";
        }
    }

    private void LoadFilters()
    {
        try
        {
            // Загрузка типов
            TypeFilter?.Items.Clear();
            TypeFilter?.Items.Add(new ComboBoxItem { Content = "Все типы", Tag = null });

            foreach (CarType type in Enum.GetValues(typeof(CarType)))
            {
                if (type != CarType.Unknown)
                {
                    TypeFilter?.Items.Add(new ComboBoxItem
                    {
                        Content = GetEnumDescription(type),
                        Tag = type
                    });
                }
            }
            if (TypeFilter != null) TypeFilter.SelectedIndex = 0;

            // Загрузка классов
            ClassFilter?.Items.Clear();
            ClassFilter?.Items.Add(new ComboBoxItem { Content = "Все классы", Tag = null });

            foreach (CarClass carClass in Enum.GetValues(typeof(CarClass)))
            {
                if (carClass != CarClass.Unknown)
                {
                    ClassFilter?.Items.Add(new ComboBoxItem
                    {
                        Content = GetClassDisplayName(carClass),
                        Tag = carClass
                    });
                }
            }
            if (ClassFilter != null) ClassFilter.SelectedIndex = 0;

            // Загрузка двигателей
            EngineFilter?.Items.Clear();
            EngineFilter?.Items.Add(new ComboBoxItem { Content = "Все двигатели", Tag = null });

            foreach (CarEngine engine in Enum.GetValues(typeof(CarEngine)))
            {
                if (engine != CarEngine.Unknown)
                {
                    EngineFilter?.Items.Add(new ComboBoxItem
                    {
                        Content = GetEnumDescription(engine),
                        Tag = engine
                    });
                }
            }
            if (EngineFilter != null) EngineFilter.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке фильтров: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private string GetEnumDescription(Enum value)
    {
        return value switch
        {
            CarType.Sedan => "Седан",
            CarType.Estate => "Универсал",
            CarType.Hatchback => "Хэтчбек",
            CarType.SUV => "Внедорожник",
            CarType.Coupe => "Купе",
            CarType.Cabriolet => "Кабриолет",
            CarType.Roadster => "Родстер",
            CarType.Minivan => "Минивэн",
            CarType.Pickup => "Пикап",
            CarType.Limousine => "Лимузин",
            CarType.Wagon => "Универсал",
            CarType.Crossover => "Кроссовер",
            CarType.Liftback => "Лифтбек",
            CarType.Fastback => "Фастбек",
            CarType.Convertible => "Конвертибл",
            CarType.Van => "Фургон",

            CarEngine.Petrol => "Бензин",
            CarEngine.Diesel => "Дизель",
            CarEngine.Electric => "Электро",
            CarEngine.Hybrid => "Гибрид",
            CarEngine.PlugInHybrid => "Подключаемый гибрид",
            CarEngine.LPG => "Газ (пропан)",
            CarEngine.CNG => "Газ (метан)",
            CarEngine.Rotary => "Роторный",
            CarEngine.Hydrogen => "Водород",

            _ => value.ToString()
        };
    }

    private string GetClassDisplayName(CarClass carClass)
    {
        return carClass switch
        {
            CarClass.A => "A-Class",
            CarClass.B => "B-Class",
            CarClass.C => "C-Class",
            CarClass.CLA => "CLA-Class",
            CarClass.E => "E-Class",
            CarClass.S => "S-Class",
            CarClass.G => "G-Class",
            CarClass.GLA => "GLA-Class",
            CarClass.GLB => "GLB-Class",
            CarClass.GLC => "GLC-Class",
            CarClass.GLE => "GLE-Class",
            CarClass.GLS => "GLS-Class",
            CarClass.AMG_GT => "AMG GT",
            CarClass.AMG_SL => "AMG SL",
            _ => carClass.ToString()
        };
    }

    private async Task LoadCarsAsync()
    {
        try
        {
            _allCars = await _context.Cars
                .Include(c => c.Images)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ApplyFilters();
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ApplyFilters()
    {
        try
        {
            var filteredCars = _allCars.AsEnumerable();

            // Поиск по модели
            if (!string.IsNullOrWhiteSpace(SearchBox?.Text))
            {
                string searchText = SearchBox.Text.ToLower();
                filteredCars = filteredCars.Where(c =>
                    c.Model.ToLower().Contains(searchText) ||
                    (c.Description?.ToLower().Contains(searchText) ?? false));
            }

            // Фильтр по типу
            if (TypeFilter?.SelectedItem is ComboBoxItem typeItem && typeItem.Tag != null)
            {
                var selectedType = (CarType)typeItem.Tag;
                filteredCars = filteredCars.Where(c => c.Type == selectedType);
            }

            // Фильтр по классу
            if (ClassFilter?.SelectedItem is ComboBoxItem classItem && classItem.Tag != null)
            {
                var selectedClass = (CarClass)classItem.Tag;
                filteredCars = filteredCars.Where(c => c.Class == selectedClass);
            }

            // Фильтр по двигателю
            if (EngineFilter?.SelectedItem is ComboBoxItem engineItem && engineItem.Tag != null)
            {
                var selectedEngine = (CarEngine)engineItem.Tag;
                filteredCars = filteredCars.Where(c => c.Engine == selectedEngine);
            }

            var carsList = filteredCars.ToList();
            CarsItemsControl.ItemsSource = carsList;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при применении фильтров: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateStatistics()
    {
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void Filter_Changed(object sender, SelectionChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void ResetFilters_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (SearchBox != null) SearchBox.Text = string.Empty;
            if (TypeFilter != null) TypeFilter.SelectedIndex = 0;
            if (ClassFilter != null) ClassFilter.SelectedIndex = 0;
            if (EngineFilter != null) EngineFilter.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сбросе фильтров: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void EditCar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid carId)
        {
            MessageBox.Show($"Редактирование автомобиля", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private async void DeleteCar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid carId)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить этот автомобиль?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var car = await _context.Cars.FindAsync(carId);
                    if (car != null)
                    {
                        _context.Cars.Remove(car);
                        await _context.SaveChangesAsync();
                        await LoadCarsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private async void AddCar_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Добавление нового автомобиля", "Информация",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        await LoadCarsAsync();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (_sessionService.IsLoggedIn)
        {
            NavigationService?.Navigate(new ProfilePage());
        }
        else
        {
            NavigationService?.Navigate(new LoginPage());
        }
    }

    private void UserInfoBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LoginButton_Click(sender, null!);
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show($"Вы уверены, что хотите выйти?",
            "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _settingsService.ClearRememberedUser();
            _sessionService.Logout();
            UpdateUserInfo();
        }
    }
}