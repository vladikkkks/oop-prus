using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Lab7_RetryPattern
{
    // ==========================================
    // 1. Клас для імітації роботи з файлами
    // ==========================================
    public class FileProcessor
    {
        private int _failedAttempts = 0;

        // Метод імітує помилку FileNotFoundException перші 3 рази
        public void UpdateUserProfile(string path, string profileJson)
        {
            _failedAttempts++;
            Console.WriteLine($"[FileProcessor] Спроба запису #{_failedAttempts}...");

            if (_failedAttempts <= 3)
            {
                Console.WriteLine("   -> Помилка: Файл не знайдено!");
                throw new FileNotFoundException($"Файл за шляхом '{path}' не знайдено.");
            }

            Console.WriteLine("   -> Успіх: Профіль оновлено у файлі.");
        }
    }

    // ==========================================
    // 2. Клас для імітації мережевих запитів
    // ==========================================
    public class NetworkClient
    {
        private int _failedAttempts = 0;

        // Метод імітує помилку HttpRequestException перші 2 рази
        public bool PostUserProfile(string url, string profileJson)
        {
            _failedAttempts++;
            Console.WriteLine($"[NetworkClient] Спроба відправки #{_failedAttempts}...");

            if (_failedAttempts <= 2)
            {
                Console.WriteLine("   -> Помилка: Мережа недоступна (503 Service Unavailable)!");
                throw new HttpRequestException("Помилка з'єднання з сервером.");
            }

            Console.WriteLine("   -> Успіх: Дані відправлено на сервер.");
            return true;
        }
    }

    // ==========================================
    // 3. Універсальний RetryHelper (Патерн Retry)
    // ==========================================
    public static class RetryHelper
    {
        /// <summary>
        /// Виконує операцію з повторними спробами у разі невдачі.
        /// </summary>
        /// <typeparam name="T">Тип результату, який повертає операція</typeparam>
        /// <param name="operation">Делегат (метод), який треба виконати</param>
        /// <param name="retryCount">Кількість повторних спроб</param>
        /// <param name="initialDelay">Початковий час очікування</param>
        /// <param name="shouldRetry">Логіка: чи варто повторювати для цієї конкретної помилки?</param>
        public static T ExecuteWithRetry<T>(
            Func<T> operation, 
            int retryCount = 3, 
            TimeSpan initialDelay = default, 
            Func<Exception, bool> shouldRetry = null)
        {
            if (initialDelay == default) initialDelay = TimeSpan.FromSeconds(1);

            for (int attempt = 0; attempt <= retryCount; attempt++)
            {
                try
                {
                    // Намагаємось виконати операцію
                    return operation();
                }
                catch (Exception ex)
                {
                    // Якщо це остання спроба - просто викидаємо помилку далі (rethrow)
                    if (attempt == retryCount)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[RetryHelper] Всі спроби вичерпано. Остання помилка: {ex.Message}");
                        Console.ResetColor();
                        throw;
                    }

                    // Перевіряємо через shouldRetry, чи підходить цей тип помилки для повтору
                    if (shouldRetry != null && !shouldRetry(ex))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[RetryHelper] Помилка '{ex.GetType().Name}' не підлягає повтору. Перериваємо.");
                        Console.ResetColor();
                        throw;
                    }

                    // Обчислення експоненційної затримки: Delay * 2^attempt
                    // Наприклад: 1с -> 2с -> 4с
                    double delayMilliseconds = initialDelay.TotalMilliseconds * Math.Pow(2, attempt);
                    TimeSpan delay = TimeSpan.FromMilliseconds(delayMilliseconds);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[RetryHelper] Зловлено помилку: {ex.Message}");
                    Console.WriteLine($"[RetryHelper] Очікування {delay.TotalSeconds:F1} сек перед спробою #{attempt + 2}...");
                    Console.ResetColor();

                    // Пауза перед наступною спробою
                    Thread.Sleep(delay);
                }
            }

            // Цей код недосяжний, але потрібен для компілятора
            throw new Exception("Невідома помилка в RetryHelper");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Console.WriteLine("=== Лабораторна робота №7: Retry Pattern ===\n");

            // Ініціалізація об'єктів
            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();

            // Визначаємо політику повторів (shouldRetry):
            // Повторюємо ТІЛЬКИ якщо це FileNotFoundException або HttpRequestException
            Func<Exception, bool> retryPolicy = ex => 
                ex is FileNotFoundException || ex is HttpRequestException;

            // ---------------------------------------------------------
            // Сценарій 1: FileProcessor (метод void)
            // ---------------------------------------------------------
            Console.WriteLine("--- Сценарій 1: Оновлення локального файлу ---");
            
            try
            {
                // Оскільки метод UpdateUserProfile повертає void, а Generic потребує повернення типу T,
                // ми обгортаємо виклик у лямбду, яка повертає true (dummy value).
                RetryHelper.ExecuteWithRetry<bool>(() => 
                {
                    fileProcessor.UpdateUserProfile("data/user.json", "{name: 'Vlad'}");
                    return true; 
                }, 
                retryCount: 4, // Даємо достатньо спроб (помилок буде 3, успіх на 4-й)
                initialDelay: TimeSpan.FromSeconds(0.5), // Починаємо з 0.5 сек
                shouldRetry: retryPolicy);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CRITICAL ERROR: {ex.Message}");
            }

            Console.WriteLine();

            // ---------------------------------------------------------
            // Сценарій 2: NetworkClient (метод повертає bool)
            // ---------------------------------------------------------
            Console.WriteLine("--- Сценарій 2: Відправка даних на сервер ---");

            try
            {
                bool result = RetryHelper.ExecuteWithRetry<bool>(() => 
                {
                    return networkClient.PostUserProfile("http://api.server.com", "{name: 'Vlad'}");
                }, 
                retryCount: 3, 
                initialDelay: TimeSpan.FromSeconds(1), 
                shouldRetry: retryPolicy);

                Console.WriteLine($"Фінальний результат операції: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CRITICAL ERROR: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}