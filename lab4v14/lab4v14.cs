using System;
using System.Text; // Потрібно для Console.OutputEncoding

namespace Lab4
{
    // --- 1. Інтерфейс (Контракт) ---
    // Визначає, що *повинен* вміти робити будь-який транспортний засіб (ТЗ).
    public interface IVehicle
    {
        // Властивість (тільки для читання)
        // Повертає середнє споживання пального у л/100км
        double ConsumptionPer100Km { get; }

        // Метод для запуску двигуна
        void StartEngine();

        // Метод для імітації поїздки
        void Drive(double distance);
    }

    // --- 2. Клас для Композиції ---
    // Клас, який представляє "частину" (двигун).
    // Автомобіль "має" (has-a) двигун.
    public class Engine
    {
        public string EngineType { get; private set; }

        public Engine(string engineType)
        {
            EngineType = engineType;
            Console.WriteLine($"[Engine] Створено двигун типу: {EngineType}");
        }

        public void Start()
        {
            Console.WriteLine($"[Engine] Двигун {EngineType} запущено. Врррум!");
        }
    }

    // --- 3. Абстрактний клас (Базова реалізація) ---
    // Надає спільну базу для всіх ТЗ.
    public abstract class AbstractVehicle : IVehicle
    {
        // Спільна властивість для всіх наслідників
        public double ConsumptionPer100Km { get; protected set; }

        // --- Демонстрація КОМПОЗИЦІЇ ---
        // Двигун є невід'ємною частиною транспортного засобу.
        // Життєвий цикл _engine контролюється цим класом.
        protected readonly Engine _engine;

        // Конструктор базового класу
        public AbstractVehicle(double baseConsumption, Engine engine)
        {
            ConsumptionPer100Km = baseConsumption;
            // Двигун створюється або передається і присвоюється ТУТ.
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        // Реалізація методу інтерфейсу (спільна для всіх)
        public void StartEngine()
        {
            _engine.Start();
        }

        // Абстрактний метод - змушує *кожен* клас-наслідник
        // надати свою власну реалізацію цього методу.
        public abstract void Drive(double distance);
    }

    // --- 4. Перша конкретна реалізація (Вантажівка) ---
    public class Truck : AbstractVehicle
    {
        public double CargoWeight { get; set; }

        // Вантажівка сама створює свій двигун (Композиція).
        public Truck(double baseConsumption, double cargoWeight)
            : base(baseConsumption, new Engine("Diesel V8")) // Створення "частини"
        {
            CargoWeight = cargoWeight;
            // Коригуємо споживання пального залежно від вантажу
            AdjustConsumptionForCargo();
        }

        // Внутрішня логіка, специфічна для вантажівки
        private void AdjustConsumptionForCargo()
        {
            // Приклад: +1л/100км за кожну тонну вантажу
            ConsumptionPer100Km += (CargoWeight / 1000.0) * 1.0;
        }

        // Реалізація абстрактного методу
        public override void Drive(double distance)
        {
            Console.WriteLine($"[Truck] Вантажівка їде {distance} км з вантажем {CargoWeight} кг.");
        }
    }

    // --- 5. Друга конкретна реалізація (Автобус) ---
    public class Bus : AbstractVehicle
    {
        public int PassengerCount { get; set; }

        // Автобус також створює свій власний двигун (Композиція)
        public Bus(double baseConsumption, int passengers)
            : base(baseConsumption, new Engine("Diesel V6")) // Створення "частини"
        {
            PassengerCount = passengers;
        }

        // Реалізація абстрактного методу
        public override void Drive(double distance)
        {
            Console.WriteLine($"[Bus] Автобус їде {distance} км з {PassengerCount} пасажирами.");
        }
    }

    // --- 6. Клас для Агрегації та Обчислень ---
    public class TripCalculator
    {
        // --- Демонстрація АГРЕГАЦІЇ ---
        // Калькулятор *використовує* IVehicle, але не володіє ним.
        private readonly IVehicle _vehicle;

        public TripCalculator(IVehicle vehicle)
        {
            _vehicle = vehicle;
        }

        // Обчислення 1: Витрати пального (грошей) на поїздку
        public double CalculateTripCost(double distance, double fuelPrice)
        {
            // Отримуємо середнє споживання з ТЗ (це Обчислення 2)
            double consumption = _vehicle.ConsumptionPer100Km;
            
            double totalFuelNeeded = (distance / 100.0) * consumption;
            double totalCost = totalFuelNeeded * fuelPrice;
            
            return totalCost;
        }
    }

    // --- 7. Точка входу (Демонстрація) ---
    class Program
    {
        static void Main(string[] args)
        {
            // Встановлюємо кодування для коректного відображення гривень '₴'
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("--- Демонстрація Лабораторної роботи №4 ---");

            // --- 1. Створення об'єктів (Композиція в дії) ---
            Console.WriteLine("\n--- Створення Вантажівки (Truck) ---");
            IVehicle myTruck = new Truck(baseConsumption: 30.0, cargoWeight: 5000); // 5 тонн

            Console.WriteLine("\n--- Створення Автобуса (Bus) ---");
            IVehicle myBus = new Bus(baseConsumption: 20.0, passengers: 40);

            // --- 2. Використання методів (Поліморфізм) ---
            Console.WriteLine("\n--- Дії Вантажівки ---");
            myTruck.StartEngine();
            myTruck.Drive(150);
            Console.WriteLine($"Середнє споживання вантажівки (з вантажем): {myTruck.ConsumptionPer100Km:F2} л/100км");

            Console.WriteLine("\n--- Дії Автобуса ---");
            myBus.StartEngine();
            myBus.Drive(150);
            Console.WriteLine($"Середнє споживання автобуса: {myBus.ConsumptionPer100Km:F2} л/100км");

            // --- 3. Демонстрація Агрегації та Обчислень ---
            Console.WriteLine("\n--- Розрахунок поїздок (Агрегація) ---");
            double distanceToTravel = 250; // км
            double priceOfDiesel = 55.50; // грн

            // Створюємо калькулятор і передаємо йому Вантажівку (Агрегація)
            TripCalculator truckCalculator = new TripCalculator(myTruck);
            double truckTripCost = truckCalculator.CalculateTripCost(distanceToTravel, priceOfDiesel);
            Console.WriteLine($"Вартість поїздки {distanceToTravel} км для Вантажівки: {truckTripCost:C2}"); // :C2 форматує як гроші

            // Створюємо інший калькулятор і передаємо йому Автобус (Агрегація)
            TripCalculator busCalculator = new TripCalculator(myBus);
            double busTripCost = busCalculator.CalculateTripCost(distanceToTravel, priceOfDiesel);
            Console.WriteLine($"Вартість поїздки {distanceToTravel} км для Автобуса: {busTripCost:C2}");
        }
    }
}