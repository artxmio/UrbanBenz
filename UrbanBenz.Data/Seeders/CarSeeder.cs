using UrbanBenz.Data.Data;
using UrbanBenz.Data.Enums;
using UrbanBenz.Data.Models;

namespace UrbanBenz.Data.Seeders;

public class CarSeeder()
{
    public static void Seed(AppDbContext context)
    {
        try
        {
            if (context.Cars.Any())
            {
                return;
            }

            var cars = GetSampleCars();

            context.Cars.AddRange(cars);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при заполнении базы данных: {ex.Message}", ex);
        }
    }

    private static List<Car> GetSampleCars()
    {
        var random = new Random();

        var cars = new List<Car>
        {
            new() {
                Id = Guid.NewGuid(),
                Model = "S 500 4MATIC",
                Type = CarType.Sedan,
                Class = CarClass.S,
                Engine = CarEngine.Petrol,
                Year = 2024,
                Color = "Черный",
                Price = 12500000m,
                VinNumber = $"W1K2231161A{random.Next(100000, 999999)}",
                IsAviable = true,
                Description = "Флагманский седан с гибридной установкой, кожа Nappa, массажные кресла",
                CreatedAt = DateTime.Now
            },

            new() {
                Id = Guid.NewGuid(),
                Model = "G 63 AMG",
                Type = CarType.SUV,
                Class = CarClass.G,
                Engine = CarEngine.Petrol,
                Year = 2024,
                Color = "Серебристый",
                Price = 18500000m,
                VinNumber = $"W1K4631161A{random.Next(100000, 999999)}",
                IsAviable = true,
                Description = "Легендарный внедорожник, V8 битурбо, спортивный пакет AMG",
                CreatedAt = DateTime.Now
            },

            new() {
                Id = Guid.NewGuid(),
                Model = "E 300 e",
                Type = CarType.Sedan,
                Class = CarClass.E,
                Engine = CarEngine.PlugInHybrid,
                Year = 2023,
                Color = "Синий",
                Price = 7500000m,
                VinNumber = $"W1K2141161A{random.Next(100000, 999999)}",
                IsAviable = true,
                Description = "Бизнес-класс с подзаряжаемым гибридом, панорамная крыша",
                CreatedAt = DateTime.Now
            },

            new() {
                Id = Guid.NewGuid(),
                Model = "GLC 300 4MATIC",
                Type = CarType.Crossover,
                Class = CarClass.GLA,
                Engine = CarEngine.Petrol,
                Year = 2024,
                Color = "Белый",
                Price = 6800000m,
                VinNumber = $"W1K2541161A{random.Next(100000, 999999)}",
                IsAviable = true,
                Description = "Компактный кроссовер, адаптивная подвеска, цифровая приборная панель",
                CreatedAt = DateTime.Now
            },

            new() {
                Id = Guid.NewGuid(),
                Model = "A 200",
                Type = CarType.Hatchback,
                Class = CarClass.A,
                Engine = CarEngine.Petrol,
                Year = 2023,
                Color = "Красный",
                Price = 3500000m,
                VinNumber = $"W1K1771161A{random.Next(100000, 999999)}",
                IsAviable = false,
                Description = "Компактный городской автомобиль, пакет AMG Line",
                CreatedAt = DateTime.Now
            },

            new() {
                Id = Guid.NewGuid(),
                Model = "CLS 350 d",
                Type = CarType.Coupe,
                Class = CarClass.C,
                Engine = CarEngine.Diesel,
                Year = 2023,
                Color = "Серый",
                Price = 8200000m,
                VinNumber = $"W1K2571161A{random.Next(100000, 999999)}",
                IsAviable = true,
                Description = "Четырехдверное купе, дизайн AMG, кожаный салон",
                CreatedAt = DateTime.Now
            },

            new() {
                Id = Guid.NewGuid(),
                Model = "EQC 400",
                Type = CarType.SUV,
                Class = CarClass.E,
                Engine = CarEngine.Electric,
                Year = 2024,
                Color = "Синий",
                Price = 7200000m,
                VinNumber = $"W1K2931161A{random.Next(100000, 999999)}",
                IsAviable = true,
                Description = "Электрический внедорожник, запас хода 450 км",
                CreatedAt = DateTime.Now
            },
            new() {
                Id = Guid.NewGuid(),
                Model = "V 300 d",
                Type = CarType.Minivan,
                Class = CarClass.G,
                Engine = CarEngine.Diesel,
                Year = 2023,
                Color = "Черный",
                Price = 8900000m,
                VinNumber = $"W1K4471161A{random.Next(100000, 999999)}",
                IsAviable = true,
                Description = "Минивэн бизнес-класса, 7 мест, трансформер",
                CreatedAt = DateTime.Now
            },

            new() {
                Id = Guid.NewGuid(),
                Model = "SL 55 AMG",
                Type = CarType.Roadster,
                Class = CarClass.S,
                Engine = CarEngine.Petrol,
                Year = 2024,
                Color = "Белый",
                Price = 14200000m,
                VinNumber = $"W1K2321161A{random.Next(100000, 999999)}",
                IsAviable = false,
                Description = "Спортивный родстер, мягкая крыша, 4.0 V8",
                CreatedAt = DateTime.Now
            }
        };

        return cars;
    }
}
