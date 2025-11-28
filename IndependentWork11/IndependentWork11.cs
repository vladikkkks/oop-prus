using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using Polly.Wrap;
using System;
using System.Threading;

namespace IndependentWork11
{
    class Program
    {
        // =============================================================
        // ЗВІТ ПО САМОСТІЙНІЙ РОБОТІ №11
        // Студента групи ІПЗ-3/1
        // Тема: Дослідження бібліотеки Polly (Retry, Circuit Breaker)
        // =============================================================
        //
        // ВСТУП:
        // Для виконання цієї роботи я обрав два сценарії використання Polly, 
        // які є найбільш актуальними для реальних проєктів:
        // 1. Захист бази даних від перевантажень (патерн Circuit Breaker).
        // 2. Обробка повільних відповідей зовнішнього API (патерни Timeout + Fallback).
        // =============================================================

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Щоб коректно відображалась кирилиця
            Console.WriteLine("=== Самостійна робота №11: Сценарії використання Polly ===\n");

            // Запуск першого сценарію
            RunCircuitBreakerScenario();

            Console.WriteLine("\n" + new string('-', 50) + "\n");

            // Запуск другого сценарію
            RunTimeoutFallbackScenario();

            // =============================================================
            // ЗАГАЛЬНІ ВИСНОВКИ
            // =============================================================
            //
            // 1. Щодо Circuit Breaker (Автоматичний вимикач):
            // Цей патерн критично важливий для стабільності системи. Він запобігає ситуації, 
            // коли ми продовжуємо надсилати запити до сервісу, який вже "впав", 
            // тим самим не даючи йому відновитися.
            //
            // 2. Щодо PolicyWrap (Timeout + Fallback):
            // Комбінування політик дозволяє робити систему гнучкою. 
            // Timeout гарантує, що клієнт не чекатиме вічно, а Fallback забезпечує 
            // "м'яку деградацію" (graceful degradation) — тобто повернення хоча б якихось даних (кешу) замість помилки.
            //
            // ЗАГАЛОМ ПО POLLY:
            // Використання цієї бібліотеки значно спрощує код. Вся логіка обробки помилок (retries, timeouts) 
            // виноситься з бізнес-логіки в окремі налаштування політик.
            // =============================================================
            
            Console.WriteLine("\n=== Кінець роботи ===");
            Console.ReadLine();
        }

        #region Сценарій 1: Circuit Breaker

        // -----------------------------------------------------------
        // СЦЕНАРІЙ 1: Захист Бази Даних (Circuit Breaker)
        // -----------------------------------------------------------
        // ОПИС ПРОБЛЕМИ:
        // Уявімо, що база даних тимчасово недоступна або перевантажена. 
        // Постійні спроби підключитися лише погіршать ситуацію.
        //
        // РІШЕННЯ (Політика Polly):
        // Використовуємо Circuit Breaker.
        // Якщо стається 2 помилки поспіль, ми "розмикаємо ланцюг".
        // Протягом 3 секунд всі нові запити відхиляються миттєво (Fail Fast), 
        // не навантажуючи БД.
        //
        // ОЧІКУВАНА ПОВЕДІНКА:
        // 1. Перші 2 запити впадуть (імітація збою).
        // 2. Circuit Breaker перейде у стан Open (Розірвано).
        // 3. Наступні запити будуть заблоковані політкою.
        // 4. Після паузи стан зміниться на Half-Open, і один запит пройде успішно.

        private static void RunCircuitBreakerScenario()
        {
            Console.WriteLine("--- Сценарій 1: Захист БД (Circuit Breaker) ---");

            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(3),
                    onBreak: (ex, breakDelay) =>
                    {
                        // Спрацьовує при розриві ланцюга
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[CB Статус] Ланцюг РОЗІРВАНО! Блокування запитів на {breakDelay.TotalSeconds} с. Причина: {ex.Message}");
                        Console.ResetColor();
                    },
                    onReset: () =>
                    {
                        // Спрацьовує при відновленні
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[CB Статус] Ланцюг ЗАМКНЕНО. Роботу відновлено.");
                        Console.ResetColor();
                    },
                    onHalfOpen: () =>
                    {
                        // Спрацьовує при пробному запиті
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("[CB Статус] Ланцюг НАПІВВІДКРИТИЙ. Тестовий запит...");
                        Console.ResetColor();
                    }
                );

            // Цикл імітує запити користувачів
            for (int i = 0; i < 6; i++)
            {
                try
                {
                    Console.Write($"Запит #{i + 1}: ");
                    // Виконуємо дію через політику
                    circuitBreakerPolicy.Execute(() => SimulateDbAccess(i));
                }
                catch (BrokenCircuitException)
                {
                    // Помилка від Polly, коли ланцюг розірвано
                    Console.WriteLine("Заблоковано запобіжником (Fail fast).");
                }
                catch (Exception ex)
                {
                    // Помилка самої БД
                    Console.WriteLine($"Невдача: {ex.Message}");
                }

                // Пауза між запитами
                Thread.Sleep(1000); 
            }
        }

        // Метод імітації БД
        private static void SimulateDbAccess(int attemptIndex)
        {
            // Імітуємо збій на перших двох спробах (індекси 0 та 1)
            if (attemptIndex < 2) 
            {
                throw new Exception("Тайм-аут підключення до БД");
            }
            
            Console.WriteLine("Дані успішно збережено в БД!");
        }

        #endregion

        #region Сценарій 2: Timeout + Fallback

        // -----------------------------------------------------------
        // СЦЕНАРІЙ 2: Повільне API (Timeout + Fallback)
        // -----------------------------------------------------------
        // ОПИС ПРОБЛЕМИ:
        // Зовнішній API іноді "зависає" надовго. Користувач не повинен чекати 10 секунд.
        //
        // РІШЕННЯ (Політика Polly):
        // Комбінація (Wrap):
        // 1. Timeout: Обмежуємо час виконання до 2 секунд.
        // 2. Fallback: Якщо виникає помилка (в т.ч. Timeout), повертаємо старі дані з кешу, 
        //    щоб програма не "впала".
        //
        // РЕАЛІЗАЦІЯ:
        // Метод SimulateSlowExternalApi спеціально робить затримку 5 секунд.

        private static void RunTimeoutFallbackScenario()
        {
            Console.WriteLine("--- Сценарій 2: Повільне API (Timeout + Fallback) ---");

            // 1. Політика тайм-ауту (2 секунди)
            var timeoutPolicy = Policy.Timeout(TimeSpan.FromSeconds(2), TimeoutStrategy.Pessimistic);

            // 2. Політика запасного варіанту (Fallback)
            var fallbackPolicy = Policy<string>
                .Handle<TimeoutRejectedException>() // Ловимо саме тайм-аут
                .Or<Exception>()
                .Fallback(() => 
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[Fallback спрацював] API відповідає надто довго. Повертаємо кешовані дані.");
                    Console.ResetColor();
                    return "ДАНІ_З_КЕШУ_V1";
                });

            // 3. Обгортаємо: Fallback поверх Timeout
            var policyWrap = fallbackPolicy.Wrap(timeoutPolicy);

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Виклик зовнішнього API (імітація зависання)...");

            // Виконуємо
            string result = policyWrap.Execute(() => SimulateSlowExternalApi());

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Результат для користувача: {result}");
        }

        // Імітація повільного сервісу
        private static string SimulateSlowExternalApi()
        {
            // Спимо 5 секунд (ліміт 2 с)
            Thread.Sleep(5000); 
            return "СВІЖІ_ДАНІ_З_API";
        }

        #endregion
    }
}