using System;
using System.Collections.Generic;

namespace IndependentWork19
{
    // 1. Інтерфейс продукту згідно з варіантом
    public interface IIdGenerator
    {
        string GenerateId();
    }

    // 2. Реалізації генераторів
    public class GuidIdGenerator : IIdGenerator
    {
        public string GenerateId() => Guid.NewGuid().ToString();
    }

    public class SequentialIdGenerator : IIdGenerator
    {
        private int _current = 1;
        public string GenerateId() => (_current++).ToString("D5"); // Наприклад: 00001, 00002
    }

    public class RandomIdGenerator : IIdGenerator
    {
        private Random _rnd = new Random();
        public string GenerateId() => _rnd.Next(100000, 999999).ToString();
    }

    // 3. Абстрактна фабрика
    public abstract class IdGeneratorFactory
    {
        public abstract IIdGenerator CreateGenerator();
    }

    // 4. Конкретні фабрики
    public class GuidFactory : IdGeneratorFactory
    {
        public override IIdGenerator CreateGenerator() => new GuidIdGenerator();
    }

    public class SequentialFactory : IdGeneratorFactory
    {
        public override IIdGenerator CreateGenerator() => new SequentialIdGenerator();
    }

    public class RandomFactory : IdGeneratorFactory
    {
        public override IIdGenerator CreateGenerator() => new RandomIdGenerator();
    }

    // 5. Клас IdService (Singleton)
    public class IdService
    {
        private static IdService? _instance;
        private IIdGenerator? _currentGenerator;

        private IdService() { }

        public static IdService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new IdService();
                return _instance;
            }
        }

        public void SetFactory(IdGeneratorFactory factory)
        {
            _currentGenerator = factory.CreateGenerator();
        }

        public string GetNextId()
        {
            if (_currentGenerator == null)
                throw new Exception("Фабрика не встановлена!");
            
            return _currentGenerator.GenerateId();
        }
    }

    // 6. Демонстрація в Main
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            IdService service = IdService.Instance;

            Console.WriteLine("=== Тестування системи генерації ID ===");

            // Використовуємо GUID
            service.SetFactory(new GuidFactory());
            Console.WriteLine($"\nGUID ID: {service.GetNextId()}");
            Console.WriteLine($"GUID ID: {service.GetNextId()}");

            // Перемикаємось на послідовну генерацію
            service.SetFactory(new SequentialFactory());
            Console.WriteLine($"\nПослідовний ID: {service.GetNextId()}");
            Console.WriteLine($"Послідовний ID: {service.GetNextId()}");

            // Перемикаємось на рандом
            service.SetFactory(new RandomFactory());
            Console.WriteLine($"\nВипадковий ID: {service.GetNextId()}");
            Console.WriteLine($"Випадковий ID: {service.GetNextId()}");
        }
    }
}