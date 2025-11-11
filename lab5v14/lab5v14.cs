// Підключаємо необхідні простори імен
using System;
using System.Collections.Generic;
using System.Linq; // Обов'язково для роботи LINQ!
using Spectre.Console; // Підключаємо для красивого виводу таблиць (опціональна вимога)

// Створюємо стандартний клас Program, оскільки мій .NET SDK
// вимагає явного методу Main (вирішує помилку CS5001).
internal class Program 
{
    // Головний метод, звідки починається виконання програми.
    private static void Main(string[] args)
    {
        // ===================================================================================
        // ЧАСТИНА 1: ДЕМОНСТРАЦІЯ РОБОТИ
        // ===================================================================================

        // 1. Створення узагальненого репозиторію (Вимога "Generics")
        // Створюємо екземпляр Repository<T>, вказавши, що 'T' - це наш клас 'Sale'.
        // Тепер 'salesRepository' може зберігати ТІЛЬКИ об'єкти типу Sale,
        // що гарантує нам безпечність типів (Type Safety).
        IRepository<Sale> salesRepository = new Repository<Sale>();

        // 2. Створення головного об'єкта (Вимога "Агрегація")
        // Створюємо нашу "АЗС" і передаємо їй в конструктор репозиторій.
        // Це приклад агрегації (або слабкої композиції), бо FuelPump "має"
        // посилання на репозиторій і використовує його для роботи.
        FuelPump pump1 = new FuelPump(salesRepository);

        AnsiConsole.MarkupLine("[green]=== Демонстрація: Додаємо коректні дані ===[/]");

        // 3. Демонстрація "Happy Path" (Успішний сценарій)
        // Використовуємо try-catch, хоча тут не очікуємо помилок,
        // щоб показати загальну структуру роботи.
        try
        {
            // Ці дані пройдуть валідацію в конструкторі Sale
            pump1.MakeSale("A-95", 30.5, 55.40m);
            pump1.MakeSale("Diesel", 50.0, 52.10m);
            pump1.MakeSale("A-95", 20.0, 55.60m);
            pump1.MakeSale("LPG", 40.2, 28.90m);
            pump1.MakeSale("Diesel", 15.0, 52.15m);
        }
        catch (InvalidSaleException ex)
        {
            // Цей блок не повинен виконатися
            AnsiConsole.MarkupLine($"[red]Помилка (неочікувана): {ex.Message}[/]");
        }

        // 4. Демонстрація обробки винятків (Вимога "Exception Handling")
        AnsiConsole.MarkupLine("\n[red]=== Демонстрація: Обробка некоректних даних (від'ємні літри) ===[/]");
        
        // Блок try-catch для валідації вхідних даних.
        try
        {
            // Свідомо намагаємося створити продаж з НЕкоректними даними (-10.0 літрів).
            // Очікуємо, що конструктор 'Sale' "кине" наш власний виняток.
            pump1.MakeSale("A-92", -10.0, 50.00m);
        }
        catch (InvalidSaleException ex) // "Ловимо" *тільки* наш кастомний виняток
        {
            // Обробляємо помилку - виводимо повідомлення, програма не "падає".
            AnsiConsole.MarkupLine($"[yellow]ПІЙМАНО ОЧІКУВАНИЙ ВИНЯТОК: {ex.Message}[/]");
        }

        // Ще одна демонстрація валідації (нульова ціна)
        AnsiConsole.MarkupLine("\n[red]=== Демонстрація: Обробка некоректних даних (нульова ціна) ===[/]");
        try
        {
            pump1.MakeSale("A-98", 20.0, 0.0m);
        }
        catch (InvalidSaleException ex)
        {
            AnsiConsole.MarkupLine($"[yellow]ПІЙМАНО ОЧІКУВАНИЙ ВИНЯТОК: {ex.Message}[/]");
        }
        Console.WriteLine();

        // 5. Демонстрація обчислень з колекціями (Вимога "LINQ")
        // Викликаємо метод, який інкапсулює (ховає) в собі
        // всю логіку роботи з LINQ (GroupBy, Sum, Average).
        pump1.PrintSalesSummaryByBrand();
    }
}


// ===================================================================================
// ЧАСТИНА 2: ВЛАСНИЙ КЛАС ВИНЯТКУ (Вимога лаби)
// ===================================================================================

/// <summary>
/// Це наш власний, користувацький клас винятку.
/// Ми його створили (вимога лаби), щоб семантично відділити
/// помилки, пов'язані з бізнес-логікою продажів, від інших системних помилок.
/// </summary>
public class InvalidSaleException : Exception // Обов'язково наслідуємо від System.Exception
{
    // Просто передаємо повідомлення про помилку "батьківському" класу Exception
    public InvalidSaleException(string message) : base(message) { }
}

// ===================================================================================
// ЧАСТИНА 3: МОДЕЛІ ПРЕДМЕТНОЇ ОБЛАСТІ (Сутності)
// ===================================================================================

/// <summary>
/// Модель (сутність), що описує один факт продажу.
/// </summary>
public class Sale
{
    // Властивості з 'get' дозволяють тільки читати дані (імутабельність)
    public string Brand { get; }
    public double Liters { get; }
    public decimal PricePerLiter { get; }
    
    // Розрахункова властивість (вимога лаби - обчислення)
    public decimal TotalAmount => (decimal)Liters * PricePerLiter;

    /// <summary>
    /// Конструктор класу Sale, який виконує ВАЛІДАЦІЮ вхідних даних.
    /// Це єдине місце, де можна створити об'єкт Sale, тому ми
    /// гарантуємо, що в системі не може існувати "неправильний" продаж.
    /// </summary>
    public Sale(string brand, double liters, decimal pricePerLiter)
    {
        // Вимога "Контроль вхідних даних"
        if (liters <= 0)
        {
            // Якщо дані невалідні - "кидаємо" наш власний виняток.
            throw new InvalidSaleException($"Кількість літрів ({liters}) має бути додатним числом.");
        }
        if (pricePerLiter <= 0)
        {
            throw new InvalidSaleException($"Ціна за літр ({pricePerLiter}) має бути додатним числом.");
        }
        if (string.IsNullOrWhiteSpace(brand))
        {
            throw new InvalidSaleException("Марка пального (brand) не може бути порожньою.");
        }

        // Якщо всі перевірки пройшли, ініціалізуємо властивості
        Brand = brand;
        Liters = liters;
        PricePerLiter = pricePerLiter;
    }

    // Перевизначаємо ToString() для зручного логування
    public override string ToString()
    {
        return $"{Brand}: {Liters:F2} л @ {PricePerLiter:C2} = {TotalAmount:C2}";
    }
}

/// <summary>
/// Клас, що моделює АЗС. Це наш "агрегатор".
/// Вимога лаби: "Композиція або агрегація".
/// FuelPump "агрегує" посилання на IRepository<Sale>.
/// </summary>
public class FuelPump
{
    // Приватне поле, що зберігає посилання на репозиторій.
    // Ми використовуємо тип ІНТЕРФЕЙСУ (IRepository), а не класу (Repository),
    // для слабкого зв'язування (принцип Dependency Inversion).
    private readonly IRepository<Sale> _salesRepository;

    // Конструктор, що приймає залежність (Dependency Injection)
    public FuelPump(IRepository<Sale> salesRepository)
    {
        _salesRepository = salesRepository;
    }

    /// <summary>
    /// Метод для реєстрації нового продажу.
    /// </summary>
    public void MakeSale(string brand, double liters, decimal pricePerLiter)
    {
        // Делегуємо створення та валідацію конструктору Sale.
        // Якщо конструктор 'Sale' "кине" виняток, цей метод також зупиниться
        // і передасть виняток "нагору" (де його "зловить" try-catch у Main).
        var sale = new Sale(brand, liters, pricePerLiter);
        
        // Якщо винятку не було, додаємо об'єкт у репозиторій
        _salesRepository.Add(sale);
        
        // Використовуємо AnsiConsole для логування
        AnsiConsole.MarkupLine($"[grey]LOG: Додано продаж: {sale}[/]");
    }

    /// <summary>
    /// Метод, що демонструє обчислення з колекціями (Вимога "LINQ").
    /// </summary>
    public void PrintSalesSummaryByBrand()
    {
        // Отримуємо *всі* дані з репозиторію
        var allSales = _salesRepository.All();

        // Перевірка, чи є взагалі дані для аналізу
        if (!allSales.Any()) // .Any() - це теж метод LINQ
        {
            AnsiConsole.MarkupLine("[yellow]Звіт: Продажів ще не було.[/]");
            return;
        }

        // ==========================================================
        // ОСНОВНА ЛОГІКА LINQ (Вимога лаби)
        // ==========================================================
        var summary = allSales
            // 1. (LINQ) Групуємо всі продажі за ключем (sale.Brand).
            // Результат: колекція груп (напр., група 'A-95', група 'Diesel')
            .GroupBy(sale => sale.Brand) 
            
            // 2. (LINQ) "Проектуємо" (перетворюємо) кожну групу
            // на новий анонімний об'єкт зі статистикою.
            .Select(group => new
            {
                Brand = group.Key, // Ключ групи (назва пального)
                
                // 3. (LINQ) Сумуємо літри *всередині* кожної групи
                TotalLiters = group.Sum(sale => sale.Liters), 
                
                // 4. (LINQ) Сумуємо загальний дохід *всередині* кожної групи
                TotalRevenue = group.Sum(sale => sale.TotalAmount), 
                
                // 5. (LINQ) Обчислюємо середню ціну (вимога лаби)
                // (Загальний дохід / Загальна к-ть літрів)
                AveragePricePerLiter = group.Sum(sale => sale.TotalAmount) / (decimal)group.Sum(sale => sale.Liters)
            })
            // 6. (LINQ) Сортуємо результат за брендом для красивого виводу
            .OrderBy(s => s.Brand);
        // ==========================================================

        AnsiConsole.MarkupLine("\n[cyan]=== Звіт по марках пального (LINQ GroupBy/Sum/Average) ===[/]");
        
        // Використовуємо Spectre.Console для виводу гарної таблиці
        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("Марка");
        table.AddColumn(new TableColumn("Продано, л").RightAligned());
        table.AddColumn(new TableColumn("Дохід, ₴").RightAligned());
        table.AddColumn(new TableColumn("Сер. Ціна, ₴/л").RightAligned());

        // Наповнюємо таблицю даними з 'summary'
        foreach (var item in summary)
        {
            table.AddRow(
                item.Brand,
                item.TotalLiters.ToString("F2"), // F2 - 2 знаки після коми
                item.TotalRevenue.ToString("C2"), // C2 - формат валюти
                item.AveragePricePerLiter.ToString("C2")
            );
        }
        
        // Малюємо таблицю в консолі
        AnsiConsole.Write(table);
    }
}


// ===================================================================================
// ЧАСТИНА 4: УЗАГАЛЬНЕНИЙ КОМПОНЕНТ (GENERICS) (Вимога лаби)
// ===================================================================================

/// <summary>
/// Узагальнений (Generic) інтерфейс репозиторію.
/// 'T' - це "заповнювач" (placeholder), який буде замінено
/// на конкретний тип (у нашому випадку - 'Sale') при використанні.
/// Це дозволяє нам перевикористовувати цей інтерфейс для 'Sale', 'User', 'Product' тощо.
/// </summary>
public interface IRepository<T> // 'T' - це generic-параметр
{
    void Add(T item); // Метод приймає будь-який об'єкт типу 'T'
    bool Remove(Predicate<T> match);
    IEnumerable<T> Where(Func<T, bool> predicate);
    T? FirstOrDefault(Func<T, bool> predicate);
    IReadOnlyList<T> All(); // Метод повертає список об'єктів 'T'
}

/// <summary>
/// Узагальнена (Generic) РЕАЛІЗАЦІЯ інтерфейсу.
/// Цей клас реалізує логіку зберігання для *будь-якого* типу 'T'.
/// Він працює на основі узагальненої колекції List<T>.
/// </summary>
public class Repository<T> : IRepository<T> // 'T' тут також є generic-параметром
{
    // Використовуємо узагальнену колекцію List<T> (вимога лаби - колекції)
    // Вона теж є generic-класом.
    private readonly List<T> _data = new List<T>();

    public void Add(T item) => _data.Add(item);

    public bool Remove(Predicate<T> match) => _data.RemoveAll(match) > 0;

    // Використовуємо LINQ (.Where) для реалізації пошуку
    public IEnumerable<T> Where(Func<T, bool> predicate) => _data.Where(predicate);

    public T? FirstOrDefault(Func<T, bool> predicate) => _data.FirstOrDefault(predicate);

    public IReadOnlyList<T> All() => _data;
}