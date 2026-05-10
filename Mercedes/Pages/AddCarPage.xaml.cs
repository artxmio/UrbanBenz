using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Mercedes.Data.Data;
using Mercedes.Data.Enums;
using Mercedes.Data.Models;
using System.IO;

namespace Mercedes.Pages;

public partial class AddCarPage : Page
{
    private readonly AppDbContext _context;
    private string? _selectedImagePath;

    public AddCarPage()
    {
        InitializeComponent();
        _context = new AppDbContext();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        LoadComboBoxes();
    }

    private void LoadComboBoxes()
    {
        // Типы кузова
        TypeComboBox.Items.Clear();
        foreach (CarType type in Enum.GetValues(typeof(CarType)))
        {
            if (type != CarType.Unknown)
            {
                TypeComboBox.Items.Add(new ComboBoxItem
                {
                    Content = GetTypeDescription(type),
                    Tag = type
                });
            }
        }
        TypeComboBox.SelectedIndex = 0;

        // Классы
        ClassComboBox.Items.Clear();
        foreach (CarClass carClass in Enum.GetValues(typeof(CarClass)))
        {
            if (carClass != CarClass.Unknown)
            {
                ClassComboBox.Items.Add(new ComboBoxItem
                {
                    Content = GetClassDisplayName(carClass),
                    Tag = carClass
                });
            }
        }
        ClassComboBox.SelectedIndex = 0;

        // Двигатели
        EngineComboBox.Items.Clear();
        foreach (CarEngine engine in Enum.GetValues(typeof(CarEngine)))
        {
            if (engine != CarEngine.Unknown)
            {
                EngineComboBox.Items.Add(new ComboBoxItem
                {
                    Content = GetEngineDescription(engine),
                    Tag = engine
                });
            }
        }
        EngineComboBox.SelectedIndex = 0;
    }

    private string GetTypeDescription(CarType type)
    {
        return type switch
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
            _ => type.ToString()
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

    private string GetEngineDescription(CarEngine engine)
    {
        return engine switch
        {
            CarEngine.Petrol => "Бензин",
            CarEngine.Diesel => "Дизель",
            CarEngine.Electric => "Электро",
            CarEngine.Hybrid => "Гибрид",
            CarEngine.PlugInHybrid => "Подключаемый гибрид",
            CarEngine.LPG => "Газ (пропан)",
            CarEngine.CNG => "Газ (метан)",
            CarEngine.Rotary => "Роторный",
            CarEngine.Hydrogen => "Водород",
            _ => engine.ToString()
        };
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.GoBack();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.GoBack();
    }

    private void SelectImage_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
            Title = "Выберите изображение автомобиля"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            _selectedImagePath = openFileDialog.FileName;
            ImagePathText.Text = System.IO.Path.GetFileName(_selectedImagePath);
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(ModelTextBox.Text))
        {
            MessageBox.Show("Введите модель автомобиля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(YearTextBox.Text, out int year) || year < 1900 || year > 2025)
        {
            MessageBox.Show("Введите корректный год выпуска", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(ColorTextBox.Text))
        {
            MessageBox.Show("Введите цвет автомобиля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
        {
            MessageBox.Show("Введите корректную цену", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(VinTextBox.Text) || VinTextBox.Text.Length != 17)
        {
            MessageBox.Show("Введите корректный VIN номер (17 символов)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var car = new Car
            {
                Id = Guid.NewGuid(),
                Model = ModelTextBox.Text,
                Type = (CarType)(TypeComboBox.SelectedItem as ComboBoxItem)!.Tag!,
                Class = (CarClass)(ClassComboBox.SelectedItem as ComboBoxItem)!.Tag!,
                Engine = (CarEngine)(EngineComboBox.SelectedItem as ComboBoxItem)!.Tag!,
                Year = year,
                Color = ColorTextBox.Text,
                Price = price,
                VinNumber = VinTextBox.Text.ToUpper(),
                IsAviable = IsAviableCheckBox.IsChecked ?? true,
                Description = DescriptionTextBox.Text,
                CreatedAt = DateTime.UtcNow
            };

            _context.Cars.Add(car);
            _context.SaveChanges();

            // Сохранение изображения
            if (!string.IsNullOrEmpty(_selectedImagePath) && System.IO.File.Exists(_selectedImagePath))
            {
                var imagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Cars");
                Directory.CreateDirectory(imagesFolder);

                var fileName = $"{car.Id}{System.IO.Path.GetExtension(_selectedImagePath)}";
                var destinationPath = System.IO.Path.Combine(imagesFolder, fileName);

                System.IO.File.Copy(_selectedImagePath, destinationPath, true);

                var carImage = new CarImage
                {
                    Id = Guid.NewGuid(),
                    CarId = car.Id,
                    Path = $"/Images/Cars/{fileName}",
                    IsMain = true
                };

                _context.CarImages.Add(carImage);
                _context.SaveChanges();
            }

            MessageBox.Show("Автомобиль успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService?.GoBack();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}