using System;

namespace lab21
{
    public interface IPassStrategy
    {
        decimal CalculatePrice(int hours, bool extraServices);
    }

    public class MorningPassStrategy : IPassStrategy
    {
        public decimal CalculatePrice(int hours, bool extraServices)
        {
            decimal baseRate = 50m;
            decimal cost = hours * baseRate * 0.8m;
            if (extraServices) cost += 30m;
            return cost;
        }
    }

    public class DayPassStrategy : IPassStrategy
    {
        public decimal CalculatePrice(int hours, bool extraServices)
        {
            decimal baseRate = 50m;
            decimal cost = hours * baseRate * 1.0m;
            if (extraServices) cost += 50m;
            return cost;
        }
    }

    public class FullPassStrategy : IPassStrategy
    {
        public decimal CalculatePrice(int hours, bool extraServices)
        {
            decimal baseRate = 50m;
            decimal cost = hours * baseRate * 1.5m;
            if (extraServices) cost += 100m;
            return cost;
        }
    }

    public class WeekendPassStrategy : IPassStrategy
    {
        public decimal CalculatePrice(int hours, bool extraServices)
        {
            decimal baseRate = 50m;
            decimal cost = hours * baseRate * 2.0m;
            if (extraServices) cost += 150m;
            return cost;
        }
    }

    public static class PassStrategyFactory
    {
        public static IPassStrategy CreateStrategy(string passType)
        {
            return passType.ToLower() switch
            {
                "morning" => new MorningPassStrategy(),
                "day" => new DayPassStrategy(),
                "full" => new FullPassStrategy(),
                "weekend" => new WeekendPassStrategy(),
                _ => throw new ArgumentException("Невідомий тип абонемента")
            };
        }
    }

    public class GymService
    {
        public decimal GetTotalCost(int hours, bool extraServices, IPassStrategy strategy)
        {
            if (strategy == null) throw new ArgumentNullException(nameof(strategy));
            return strategy.CalculatePrice(hours, extraServices);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var gymService = new GymService();

            try
            {
                Console.WriteLine("Доступні абонементи: Morning, Day, Full, Weekend");
                Console.Write("Оберіть тип: ");
                string type = Console.ReadLine();

                Console.Write("Кількість годин: ");
                int hours = int.Parse(Console.ReadLine());

                Console.Write("Додаткові послуги (сауна/басейн) - так/ні: ");
                bool extra = Console.ReadLine().ToLower() == "так";

                IPassStrategy strategy = PassStrategyFactory.CreateStrategy(type);
                decimal price = gymService.GetTotalCost(hours, extra, strategy);

                Console.WriteLine($"\nВартість абонемента ({type}): {price:F2} грн.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПомилка: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}