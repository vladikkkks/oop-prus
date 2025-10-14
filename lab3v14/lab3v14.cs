using System;
using System.Collections.Generic;
using System.Linq;

// -------------------------------------------------------------------------------------
// БАЗОВИЙ КЛАС: USER (Наслідування: Основи)
// -------------------------------------------------------------------------------------
// Базовий абстрактний клас. Не можна створити його екземпляр напряму.
public abstract class User
{
    // Приватне поле для інкапсуляції логіну
    private readonly string _login;
    
    // Захищене поле (`protected`): доступне в базовому класі та всіх похідних класах.
    // Використовується для обчислення активності.
    protected int operationsCount;

    // Властивість для доступу до логіну (тільки для читання)
    public string Login
    {
        get { return _login; }
    }

    // Властивість для доступу до кількості операцій (тільки для читання)
    public int OperationsCount
    {
        get { return operationsCount; }
    }

    // Конструктор базового класу
    public User(string login)
    {
        // Ініціалізація логіну
        _login = login;
        operationsCount = 0;
        Console.WriteLine($"[КОНСТРУКТОР User] Створено користувача: {Login}");
    }

    // Абстрактний метод: Вимога. Повинен бути реалізований (`override`)
    // у кожному похідному класі. Визначає загальний контракт поведінки.
    public abstract void PerformAction(string actionDescription);

    // Віртуальний метод: Може бути перевизначений (`override`) у похідних класах,
    // але має базову реалізацію.
    public virtual void DisplayUserInfo()
    {
        Console.WriteLine($"\n--- Інформація про користувача ---");
        Console.WriteLine($"Логін: {Login}");
        Console.WriteLine($"Кількість операцій: {operationsCount}");
    }

    // Деструктор (Фіналізатор): Вимога. Викликається, коли об'єкт знищується збирачем сміття.
    ~User()
    {
        Console.WriteLine($"[ДЕСТРУКТОР] Об'єкт User ({Login}) знищено.");
    }
}

// -------------------------------------------------------------------------------------
// ПОХІДНИЙ КЛАС: ADMIN (Наслідування)
// -------------------------------------------------------------------------------------
public class Admin : User
{
    private readonly string _accessLevel;

    // Конструктор похідного класу. Використовує `base(...)` для виклику конструктора базового класу.
    public Admin(string login, string accessLevel) : base(login)
    {
        this._accessLevel = accessLevel;
        Console.WriteLine($"[КОНСТРУКТОР Admin] Адміністратор {Login} з рівнем доступу: {_accessLevel}");
    }

    // Перевизначення (`override`) абстрактного методу: реалізація адміністративної дії.
    public override void PerformAction(string actionDescription)
    {
        Console.WriteLine($"[Admin {Login}] Виконав адміністративну дію: '{actionDescription}'");
        // Збільшення захищеного поля базового класу
        operationsCount += 2; // Адмінські дії рахуємо як 2 операції
    }

    // Перевизначення (`override`) віртуального методу: додає специфічну для Admin інформацію.
    public override void DisplayUserInfo()
    {
        // Виклик базової реалізації методу за допомогою `base.`
        base.DisplayUserInfo();
        Console.WriteLine($"Тип: Адміністратор (Access: {_accessLevel})");
    }
}

// -------------------------------------------------------------------------------------
// ПОХІДНИЙ КЛАС: GUEST (Наслідування)
// -------------------------------------------------------------------------------------
public class Guest : User
{
    private readonly DateTime _sessionStartTime;

    // Конструктор похідного класу. Використовує `base(...)` для виклику конструктора базового класу.
    public Guest(string login) : base(login)
    {
        _sessionStartTime = DateTime.Now;
        Console.WriteLine($"[КОНСТРУКТОР Guest] Гість {Login} розпочав сесію: {_sessionStartTime:HH:mm:ss}");
    }

    // Перевизначення (`override`) абстрактного методу: реалізація дії гостя.
    public override void PerformAction(string actionDescription)
    {
        Console.WriteLine($"[Guest {Login}] Перегляд контенту: '{actionDescription}'");
        // Збільшення захищеного поля базового класу
        operationsCount++;
    }

    // Перевизначення (`override`) віртуального методу: додає специфічну для Guest інформацію.
    public override void DisplayUserInfo()
    {
        // Виклик базової реалізації методу
        base.DisplayUserInfo();
        Console.WriteLine($"Тип: Гість (Session started: {_sessionStartTime:HH:mm:ss})");
    }
}

// -------------------------------------------------------------------------------------
// ГОЛОВНА ПРОГРАМА (Демонстрація)
// -------------------------------------------------------------------------------------
public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("--- Лабораторна робота №3: Наслідування та Поліморфізм ---");

        // Створення об'єктів
        Admin admin1 = new Admin("boss", "SuperAdmin");
        Guest guest1 = new Guest("viewer77");
        Admin admin2 = new Admin("moderator", "Standard");
        
        Console.WriteLine("\n--- Виконання операцій ---");
        
        // Виклик перевизначених методів, що збільшують лічильник
        admin1.PerformAction("видалення старого профілю");
        guest1.PerformAction("перегляд головної сторінки");
        admin1.PerformAction("блокування IP-адреси");
        admin2.PerformAction("модерація коментарів");
        guest1.PerformAction("читання статті");
        guest1.PerformAction("закриття вікна");
        
        // -------------------------------------------------------------------------------------
        // ДЕМОНСТРАЦІЯ ПОЛІМОРФІЗМУ
        // -------------------------------------------------------------------------------------
        Console.WriteLine("\n--- Демонстрація Поліморфізму на колекції ---");
        
        // Колекція об'єктів базового типу User, що містить похідні об'єкти.
        List<User> userList = new List<User>
        {
            admin1,
            guest1,
            admin2,
            new Guest("anonymous") // Створення об'єкта без змінної
        };

        // Ітерація: При виклику методу, в часі виконання, викликається
        // відповідна реалізація похідного класу (runtime polymorphism).
        foreach (User user in userList)
        {
            // Поліморфний виклик абстрактного/перевизначеного методу
            user.PerformAction("загальний лог активності"); 
            // Поліморфний виклик віртуального/перевизначеного методу
            user.DisplayUserInfo();
        }

        // -------------------------------------------------------------------------------------
        // ОБЧИСЛЕННЯ: Пошук найбільш активного користувача
        // -------------------------------------------------------------------------------------
        Console.WriteLine("\n--- Обчислення: Користувач з найбільшою активністю ---");

        // Виправлення CS8600: використовуємо User? (нульований тип), оскільки FirstOrDefault() може повернути null.
        User? mostActiveUser = userList.OrderByDescending(u => u.OperationsCount).FirstOrDefault();

        // Обов'язкова перевірка на null для безпечного доступу до полів
        if (mostActiveUser != null)
        {
            Console.WriteLine($"\nЗНАЙДЕНО: Користувач з найбільшою активністю:");
            Console.WriteLine($"Логін: {mostActiveUser.Login}");
            Console.WriteLine($"Кількість операцій: {mostActiveUser.OperationsCount}");
            // Поліморфний виклик для детальної інформації
            mostActiveUser.DisplayUserInfo();
        }
        
        // Демонстрація виклику деструкторів
        Console.WriteLine("\n--- Виклик збирача сміття для демонстрації деструкторів ---");
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}