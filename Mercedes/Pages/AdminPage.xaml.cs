using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Mercedes.Data.Data;
using Mercedes.Data.Models;
using Mercedes.Services;
using Mercedes.Data.Enums;
using ClosedXML.Excel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

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

            // Загрузка продаж
            var sales = _context.Sales
                .Include(s => s.User)
                .Include(s => s.Car)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();
            SalesDataGrid.ItemsSource = sales;

            // Загрузка статистики
            LoadStatistics();
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

    // Продажи
    private void CompleteSale_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid saleId)
        {
            try
            {
                var sale = _context.Sales.Find(saleId);
                if (sale != null)
                {
                    sale.Status = SaleStatus.Completed;
                    sale.CompletedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    LoadData();
                    MessageBox.Show("Продажа завершена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void CancelSale_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Guid saleId)
        {
            var result = MessageBox.Show("Вы уверены, что хотите отменить эту продажу? Автомобиль снова станет доступным.", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var sale = _context.Sales.Find(saleId);
                    if (sale != null)
                    {
                        // Возвращаем автомобиль в наличие
                        var car = _context.Cars.Find(sale.CarId);
                        if (car != null)
                        {
                            car.IsAviable = true;
                        }

                        sale.Status = SaleStatus.Cancelled;
                        _context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Продажа отменена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    // Экспорт в Excel
    private void ExportUsers_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var users = _context.Users.OrderByDescending(u => u.CreatedAt).ToList();
            
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Пользователи");
            
            // Заголовки
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Имя";
            worksheet.Cell(1, 3).Value = "Фамилия";
            worksheet.Cell(1, 4).Value = "Email";
            worksheet.Cell(1, 5).Value = "Телефон";
            worksheet.Cell(1, 6).Value = "Роль";
            worksheet.Cell(1, 7).Value = "Активен";
            worksheet.Cell(1, 8).Value = "Дата регистрации";
            
            var headerRange = worksheet.Range(1, 1, 1, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1A1A1A");
            headerRange.Style.Font.FontColor = XLColor.White;
            
            // Данные
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                worksheet.Cell(i + 2, 1).Value = user.Id.ToString();
                worksheet.Cell(i + 2, 2).Value = user.FirstName;
                worksheet.Cell(i + 2, 3).Value = user.LastName;
                worksheet.Cell(i + 2, 4).Value = user.Email;
                worksheet.Cell(i + 2, 5).Value = user.Phone ?? "";
                worksheet.Cell(i + 2, 6).Value = user.Role.ToString();
                worksheet.Cell(i + 2, 7).Value = user.IsActive ? "Да" : "Нет";
                worksheet.Cell(i + 2, 8).Value = user.CreatedAt.ToString("dd.MM.yyyy HH:mm");
            }
            
            worksheet.Columns().AdjustToContents();
            
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"users_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };
            
            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Данные успешно выгружены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка выгрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportCars_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var cars = _context.Cars.OrderByDescending(c => c.CreatedAt).ToList();
            
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Автомобили");
            
            // Заголовки
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Модель";
            worksheet.Cell(1, 3).Value = "Тип кузова";
            worksheet.Cell(1, 4).Value = "Класс";
            worksheet.Cell(1, 5).Value = "Двигатель";
            worksheet.Cell(1, 6).Value = "Год";
            worksheet.Cell(1, 7).Value = "Цвет";
            worksheet.Cell(1, 8).Value = "Цена";
            worksheet.Cell(1, 9).Value = "VIN";
            worksheet.Cell(1, 10).Value = "В наличии";
            
            var headerRange = worksheet.Range(1, 1, 1, 10);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1A1A1A");
            headerRange.Style.Font.FontColor = XLColor.White;
            
            // Данные
            for (int i = 0; i < cars.Count; i++)
            {
                var car = cars[i];
                worksheet.Cell(i + 2, 1).Value = car.Id.ToString();
                worksheet.Cell(i + 2, 2).Value = car.Model;
                worksheet.Cell(i + 2, 3).Value = car.Type.ToString();
                worksheet.Cell(i + 2, 4).Value = car.Class.ToString();
                worksheet.Cell(i + 2, 5).Value = car.Engine.ToString();
                worksheet.Cell(i + 2, 6).Value = car.Year;
                worksheet.Cell(i + 2, 7).Value = car.Color;
                worksheet.Cell(i + 2, 8).Value = car.Price;
                worksheet.Cell(i + 2, 9).Value = car.VinNumber;
                worksheet.Cell(i + 2, 10).Value = car.IsAviable ? "В наличии" : "Нет в наличии";
            }
            
            worksheet.Columns().AdjustToContents();
            
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"cars_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };
            
            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Данные успешно выгружены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка выгрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportTestDrives_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var testDrives = _context.TestDriveRequests
                .Include(t => t.User)
                .Include(t => t.Car)
                .OrderByDescending(t => t.RequestedDate)
                .ToList();
            
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Тест-драйвы");
            
            // Заголовки
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Клиент";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Телефон";
            worksheet.Cell(1, 5).Value = "Автомобиль";
            worksheet.Cell(1, 6).Value = "Дата";
            worksheet.Cell(1, 7).Value = "Статус";
            
            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1A1A1A");
            headerRange.Style.Font.FontColor = XLColor.White;
            
            // Данные
            for (int i = 0; i < testDrives.Count; i++)
            {
                var td = testDrives[i];
                worksheet.Cell(i + 2, 1).Value = td.Id.ToString();
                worksheet.Cell(i + 2, 2).Value = $"{td.User?.FirstName} {td.User?.LastName}";
                worksheet.Cell(i + 2, 3).Value = td.User?.Email ?? "";
                worksheet.Cell(i + 2, 4).Value = td.User?.Phone ?? "";
                worksheet.Cell(i + 2, 5).Value = td.Car?.Model ?? "";
                worksheet.Cell(i + 2, 6).Value = td.RequestedDate.ToString("dd.MM.yyyy HH:mm");
                worksheet.Cell(i + 2, 7).Value = td.Status.ToString();
            }
            
            worksheet.Columns().AdjustToContents();
            
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"testdrives_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };
            
            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Данные успешно выгружены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка выгрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportSales_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var sales = _context.Sales
                .Include(s => s.User)
                .Include(s => s.Car)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();
            
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Продажи");
            
            // Заголовки
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Клиент";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Автомобиль";
            worksheet.Cell(1, 5).Value = "Цена";
            worksheet.Cell(1, 6).Value = "Дата";
            worksheet.Cell(1, 7).Value = "Статус";
            
            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1A1A1A");
            headerRange.Style.Font.FontColor = XLColor.White;
            
            // Данные
            for (int i = 0; i < sales.Count; i++)
            {
                var sale = sales[i];
                worksheet.Cell(i + 2, 1).Value = sale.Id.ToString();
                worksheet.Cell(i + 2, 2).Value = $"{sale.User?.FirstName} {sale.User?.LastName}";
                worksheet.Cell(i + 2, 3).Value = sale.User?.Email ?? "";
                worksheet.Cell(i + 2, 4).Value = sale.Car?.Model ?? "";
                worksheet.Cell(i + 2, 5).Value = sale.Price;
                worksheet.Cell(i + 2, 6).Value = sale.CreatedAt.ToString("dd.MM.yyyy HH:mm");
                worksheet.Cell(i + 2, 7).Value = sale.Status.ToString();
            }
            
            worksheet.Columns().AdjustToContents();
            
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"sales_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };
            
            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Данные успешно выгружены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка выгрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Статистика
    private void LoadStatistics()
    {
        try
        {
            // Общая статистика
            var totalUsers = _context.Users.Count();
            var totalCars = _context.Cars.Count();
            var soldCars = _context.Cars.Count(c => !c.IsAviable);
            var availableCars = _context.Cars.Count(c => c.IsAviable);
            var totalTestDrives = _context.TestDriveRequests.Count();
            var totalRevenue = _context.Sales.Where(s => s.Status == SaleStatus.Completed).Sum(s => s.Price);

            TotalUsersText.Text = totalUsers.ToString();
            TotalCarsText.Text = totalCars.ToString();
            SoldCarsText.Text = soldCars.ToString();
            AvailableCarsText.Text = availableCars.ToString();
            TotalTestDrivesText.Text = totalTestDrives.ToString();
            TotalRevenueText.Text = $"{totalRevenue:N0} BYN";

            // Продажи по месяцам - линейный график
            var salesByMonth = _context.Sales
                .Where(s => s.Status == SaleStatus.Completed)
                .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Count = g.Count()
                })
                .ToList();

            var salesPlotModel = new PlotModel { Title = "Продажи по месяцам" };
            salesPlotModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                ItemsSource = salesByMonth.Select(x => x.Month).ToList()
            });
            salesPlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0
            });
            
            var lineSeries = new LineSeries
            {
                Color = OxyColors.DeepSkyBlue,
                StrokeThickness = 2,
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerFill = OxyColors.DeepSkyBlue
            };
            
            for (int i = 0; i < salesByMonth.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i, salesByMonth[i].Count));
            }
            
            salesPlotModel.Series.Add(lineSeries);
            SalesByMonthPlot.Model = salesPlotModel;

            // Автомобили по типу кузова - круговая диаграмма
            var carsByType = _context.Cars
                .GroupBy(c => c.Type)
                .Select(g => new
                {
                    Type = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();

            var carsTypePlotModel = new PlotModel { Title = "Автомобили по типу кузова" };
            
            var pieSeries = new PieSeries
            {
                StrokeThickness = 2,
                InsideLabelPosition = 0.5,
                AngleSpan = 360,
                StartAngle = 0
            };
            
            foreach (var item in carsByType)
            {
                pieSeries.Slices.Add(new PieSlice(item.Type, item.Count)
                {
                    Fill = OxyColor.FromRgb(
                        (byte)(item.Count * 50 % 256),
                        (byte)(item.Count * 100 % 256),
                        (byte)(item.Count * 150 % 256))
                });
            }
            
            carsTypePlotModel.Series.Add(pieSeries);
            CarsByTypePlot.Model = carsTypePlotModel;

            // Статус тест-драйвов - круговая диаграмма
            var testDrivesByStatus = _context.TestDriveRequests
                .GroupBy(t => t.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();

            var testDrivesPlotModel = new PlotModel { Title = "Статус тест-драйвов" };
            
            var pieSeries2 = new PieSeries
            {
                StrokeThickness = 2,
                InsideLabelPosition = 0.5,
                AngleSpan = 360,
                StartAngle = 0
            };

            foreach (var item in testDrivesByStatus)
            {
                pieSeries2.Slices.Add(new PieSlice(item.Status, item.Count)
                {
                    Fill = OxyColor.FromRgb(
                        (byte)(item.Count * 80 % 256),
                        (byte)(item.Count * 120 % 256),
                        (byte)(item.Count * 160 % 256))
                });
            }
            
            testDrivesPlotModel.Series.Add(pieSeries2);
            TestDrivesStatusPlot.Model = testDrivesPlotModel;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки статистики: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}