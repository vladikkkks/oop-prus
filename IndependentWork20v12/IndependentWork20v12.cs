using System;
using System.Collections.Generic;

namespace IndependentWork20
{
    // --- ПАТЕРН STRATEGY ---

    // Інтерфейс стратегії
    public interface IDataProcessorStrategy
    {
        void Process(string data);
    }

    // Реалізація 1: Цельсій -> Фаренгейт
    public class CelsiusToFahrenheitStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            if (double.TryParse(data, out double celsius))
            {
                double fahrenheit = (celsius * 9 / 5) + 32;
                Console.WriteLine($"[STRATEGY] Конвертація: {celsius}°C = {fahrenheit:F2}°F");
            }
        }
    }

    // Реалізація 2: Фаренгейт -> Цельсій
    public class FahrenheitToCelsiusStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            if (double.TryParse(data, out double fahrenheit))
            {
                double celsius = (fahrenheit - 32) * 5 / 9;
                Console.WriteLine($"[STRATEGY] Конвертація: {fahrenheit}°F = {celsius:F2}°C");
            }
        }
    }

    // Реалізація 3: Швидкість вітру (м/с в км/год)
    public class WindSpeedConverterStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            if (double.TryParse(data, out double ms))
            {
                double kmh = ms * 3.6;
                Console.WriteLine($"[STRATEGY] Вітер: {ms} м/с = {kmh:F2} км/год");
            }
        }
    }

    // Контекст, що використовує стратегію
    public class DataContext
    {
        private IDataProcessorStrategy _strategy;

        public DataContext(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        public void ExecuteProcessing(string data)
        {
            _strategy.Process(data);
        }
    }

    // --- ПАТЕРН OBSERVER (через події C#) ---

    public class DataPublisher
    {
        // Подія, на яку підписуватимуться спостерігачі
        public event Action<string>? DataProcessed;

        public void PublishDataProcessed(string data)
        {
            Console.WriteLine($"\n[PUBLISHER] Надсилання сповіщення про дані: {data}");
            DataProcessed?.Invoke(data);
        }
    }

    // Спостерігач 1: Вивід у консоль
    public class ConsoleOutputObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"[OBSERVER] ConsoleOutput: Отримано нові погодні дані: {data}");
        }
    }

    // Спостерігач 2: "База даних"
    public class WeatherDatabaseObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"[OBSERVER] Database: Дані '{data}' збережено в історію погоди.");
        }
    }

    // --- MAIN ---

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Ініціалізація
            var publisher = new DataPublisher();
            var context = new DataContext(new CelsiusToFahrenheitStrategy());

            // Створення спостерігачів
            var consoleObs = new ConsoleOutputObserver();
            var dbObs = new WeatherDatabaseObserver();

            // Підписка на події
            publisher.DataProcessed += consoleObs.OnDataProcessed;
            publisher.DataProcessed += dbObs.OnDataProcessed;

            Console.WriteLine("=== Система обробки погоди (Strategy + Observer) ===\n");

            // Етап 1: Конвертація температури
            Console.WriteLine("--- Крок 1: Цельсій в Фаренгейт ---");
            context.ExecuteProcessing("25");
            publisher.PublishDataProcessed("25°C оброблено");

            // Етап 2: Зміна стратегії на вітер
            Console.WriteLine("\n--- Крок 2: Швидкість вітру ---");
            context.SetStrategy(new WindSpeedConverterStrategy());
            context.ExecuteProcessing("10");
            publisher.PublishDataProcessed("10 м/с оброблено");

            // Етап 3: Зміна стратегії на Фаренгейт -> Цельсій
            Console.WriteLine("\n--- Крок 3: Фаренгейт в Цельсій ---");
            context.SetStrategy(new FahrenheitToCelsiusStrategy());
            context.ExecuteProcessing("77");
            publisher.PublishDataProcessed("77°F оброблено");
        }
    }
}