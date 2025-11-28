using System;
using System.Collections.Generic;
using System.Linq; // Необхідно для методів розширення (Average, OrderBy, FirstOrDefault)
using System.Text;

namespace Lab6_Lambda_Delegates
{
    // --- ТЕОРІЯ: Оголошення делегата ---
    // Делегат - це "шаблон" методу. Цей делегат може зберігати посилання на БУДЬ-ЯКИЙ метод,
    // який приймає два числа double і повертає double.
    public delegate double MathOperation(double a, double b);

    // Клас для другої частини завдання
    public class BankAccount
    {
        public string Number { get; set; }
        public decimal Balance { get; set; }
        public BankAccount(string number, decimal balance) 
        { 
            Number = number; 
            Balance = balance; 
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Налаштування консолі для коректного відображення української мови (UTF-8)
            Console.OutputEncoding = Encoding.UTF8;

            // =========================================================
            // ЧАСТИНА 1: List<int> та вбудовані делегати (Predicate, Comparison, Func)
            // =========================================================
            Console.WriteLine("=== Частина 1: Робота з числами ===");

            // Створюємо список цілих чисел
            List<int> numbers = new List<int> { 5, 12, 3, 8, 1, 19, 4, 6, 11 };
            Console.WriteLine($"Початковий список: {string.Join(", ", numbers)}");


            // --- 1. Predicate<T> (Фільтрація) ---
            // Predicate завжди повертає bool (true або false).
            // Лямбда: x => x % 2 == 0 (читається: "число x є парним?")
            Predicate<int> isEven = x => x % 2 == 0;
            
            // Метод FindAll використовує Predicate для відбору елементів
            List<int> evenNumbers = numbers.FindAll(isEven);
            Console.WriteLine($"\n[Predicate] Тільки парні числа: {string.Join(", ", evenNumbers)}");


            // --- 2. Comparison<T> (Сортування) ---
            // Comparison приймає два об'єкти і повертає int (-1, 0, 1), що вказує на порядок.
            // Лямбда: (x, y) => y.CompareTo(x)
            // Примітка: x.CompareTo(y) - сортує за зростанням.
            //           y.CompareTo(x) - сортує за спаданням (обернений порядок).
            Comparison<int> sortDescending = (x, y) => y.CompareTo(x);
            
            // Метод Sort змінює порядок елементів у самому списку
            evenNumbers.Sort(sortDescending); 
            Console.WriteLine($"[Comparison] Відсортовані за спаданням: {string.Join(", ", evenNumbers)}");


            // --- 3. Func<T, T, TResult> (Обчислення суми) ---
            // Func - це універсальний делегат, який завжди повертає значення.
            // Тут: (int, int) -> повертає int.
            // currentSum - накопичена сума, item - поточний елемент списку.
            Func<int, int, int> sumFunc = (currentSum, item) => currentSum + item;

            // Aggregate проходить по списку і застосовує Func до кожного елемента, накопичуючи результат.
            int totalSum = evenNumbers.Aggregate(sumFunc);
            Console.WriteLine($"[Func] Сума парних чисел: {totalSum}\n");


            // =========================================================
            // ЧАСТИНА 2: BankAccount та LINQ
            // =========================================================
            Console.WriteLine("=== Частина 2: Банківські рахунки (LINQ) ===");

            List<BankAccount> accounts = new List<BankAccount>
            {
                new BankAccount("UA001", 1500.50m),
                new BankAccount("UA002", 500.00m),
                new BankAccount("UA003", 12000.75m),
                new BankAccount("UA004", 320.00m), 
                new BankAccount("UA005", 2500.00m)
            };

            // --- Action<T> ---
            // Action нічого не повертає (void). Ідеально для виводу на екран.
            // Лямбда: acc => Console.WriteLine(...)
            Console.WriteLine("Список рахунків:");
            Action<BankAccount> printAccount = acc => Console.WriteLine($"   Рахунок {acc.Number} : {acc.Balance} грн");
            
            // ForEach виконує Action для кожного елемента списку
            accounts.ForEach(printAccount);


            // --- LINQ Average (використовує Func) ---
            // Ми кажемо: "візьми з кожного об'єкта acc поле Balance і порахуй середнє"
            decimal avg = accounts.Average(acc => acc.Balance);
            Console.WriteLine($"\nСередній баланс: {avg:F2} грн");


            // --- LINQ OrderBy + FirstOrDefault ---
            // OrderBy(acc => acc.Balance) - сортує колекцію за зростанням балансу.
            // FirstOrDefault() - бере перший елемент (найменший) або null, якщо список порожній.
            var minAcc = accounts.OrderBy(acc => acc.Balance).FirstOrDefault();

            // Перевірка на null (Good practice)
            if (minAcc != null)
            {
                Console.WriteLine($"Мінімальний баланс у клієнта: {minAcc.Number} ({minAcc.Balance} грн)");
            }
            
            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}