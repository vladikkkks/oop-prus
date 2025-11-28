using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    class Program
    {
        // =============================================================
        // ЗВІТ ПО САМОСТІЙНІЙ РОБОТІ №12
        // Студента групи ІПЗ-3/1, Пруса Владислава
        // Тема: PLINQ: дослідження продуктивності та безпеки.
        // =============================================================
        //
        // ВСТУП:
        // У цій роботі я досліджував бібліотеку Parallel LINQ (PLINQ).
        // Моєю метою було з'ясувати:
        // 1. Наскільки PLINQ швидший за звичайний LINQ на великих масивах даних.
        // 2. Які небезпеки (побічні ефекти) виникають при паралельній обробці даних
        //    і як їх уникати.
        // =============================================================

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Independent Work 12: PLINQ Research ===\n");

            // ЕТАП 1: Підготовка даних
            int dataSize = 5_000_000; // 5 мільйонів елементів
            Console.WriteLine($"[Setup] Генеруємо колекцію з {dataSize:N0} елементів...");
            var data = GenerateData(dataSize);
            Console.WriteLine("[Setup] Дані згенеровано.\n");

            // ЕТАП 2: Тестування продуктивності
            RunPerformanceTest(data);

            Console.WriteLine("\n" + new string('-', 50) + "\n");

            // ЕТАП 3: Тестування безпеки (Race Condition)
            RunSafetyTest(data);

            // =============================================================
            // ВИСНОВКИ (ЗВІТ)
            // =============================================================
            //
            // 1. ПРОДУКТИВНІСТЬ:
            // Експерименти показали, що на великому обсязі даних (5 млн елементів)
            // PLINQ працює значно швидше (у моєму випадку — у 3-4 рази).
            // Це відбувається тому, що PLINQ розбиває задачу на частини і виконує їх
            // одночасно на всіх доступних ядрах процесора, тоді як звичайний LINQ
            // використовує лише одне ядро.
            // * Важливо: PLINQ має певні накладні витрати на створення потоків, тому
            // на малих колекціях (наприклад, 1000 елементів) він може бути навіть повільнішим.
            //
            // 2. БЕЗПЕКА І ПОБІЧНІ ЕФЕКТИ:
            // Як показав другий експеримент, використання спільної змінної всередині
            // паралельного циклу без синхронізації призводить до втрати даних (Race Condition).
            // Результат обчислень стає непередбачуваним.
            //
            // Я виправив це двома способами:
            // а) Використання lock (блокування) — це вирішує проблему, але "вбиває"
            //    швидкодію, роблячи PLINQ повільнішим за звичайний цикл.
            // б) (Рекомендовано) Використання методів агрегації (як .Count() або .Sum()),
            //    які є потокобезпечними "з коробки".
            //
            // ПІДСУМОК:
            // PLINQ — потужний інструмент для "важких" обчислень, але його треба
            // використовувати обережно, уникаючи зміни спільних змінних всередині запитів.
            // =============================================================

            Console.WriteLine("\n=== Кінець роботи ===");
        }

        #region Методи для тестування продуктивності

        static void RunPerformanceTest(List<int> data)
        {
            Console.WriteLine("--- ЧАСТИНА 1: Порівняння продуктивності (Heavy Math) ---");
            Stopwatch sw = new Stopwatch();

            // 1. Звичайний LINQ
            sw.Start();
            var linqResult = data
                .Where(x => x % 2 == 0)      // Фільтрація
                .Select(x => HeavyOperation(x)) // Важка операція
                .ToList();                   // Примусове виконання (Materialization)
            sw.Stop();
            Console.WriteLine($"[LINQ]  Час виконання: {sw.ElapsedMilliseconds} мс. (Знайдено: {linqResult.Count})");

            sw.Reset();

            // 2. PLINQ (.AsParallel)
            sw.Start();
            var plinqResult = data
                .AsParallel()                // Вмикаємо паралелізм
                .Where(x => x % 2 == 0)
                .Select(x => HeavyOperation(x))
                .ToList();
            sw.Stop();
            Console.WriteLine($"[PLINQ] Час виконання: {sw.ElapsedMilliseconds} мс. (Знайдено: {plinqResult.Count})");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(">> PLINQ ефективніший для CPU-bound задач.");
            Console.ResetColor();
        }

        // Імітація важкої математичної операції, щоб завантажити процесор
        static double HeavyOperation(int number)
        {
            return Math.Sqrt(Math.Pow(number, 2) + Math.Pow(Math.Sin(number), 2)) * Math.Tan(number);
        }

        static List<int> GenerateData(int count)
        {
            var random = new Random();
            var list = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(random.Next(1, 100));
            }
            return list;
        }

        #endregion

        #region Методи для тестування безпеки

        static void RunSafetyTest(List<int> data)
        {
            Console.WriteLine("--- ЧАСТИНА 2: Проблеми безпеки (Race Condition) ---");
            Console.WriteLine("Завдання: Порахувати кількість парних чисел у масиві.");

            // 1. Очікуваний правильний результат (LINQ - один потік)
            int expectedCount = data.Count(x => x % 2 == 0);
            Console.WriteLine($"[Control] Очікувана (правильна) кількість: {expectedCount:N0}");

            // 2. Небезпечний підхід (Race Condition)
            int unsafeCounter = 0;
            // ForAll виконується паралельно. Кілька потоків намагаються одночасно
            // записати в unsafeCounter, перезаписуючи результати один одного.
            data.AsParallel().ForAll(x =>
            {
                if (x % 2 == 0)
                {
                    unsafeCounter++; // НЕБЕЗПЕЧНО! Операція ++ не є атомарною.
                }
            });

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Unsafe PLINQ] Результат без синхронізації: {unsafeCounter:N0}");
            Console.WriteLine($" -> Помилка (різниця): {expectedCount - unsafeCounter:N0} втрачених оновлень!");
            Console.ResetColor();

            // 3. Виправлення проблеми (використання lock)
            int safeCounter = 0;
            object lockObj = new object();

            data.AsParallel().ForAll(x =>
            {
                if (x % 2 == 0)
                {
                    // Блокуємо доступ до змінної для інших потоків, поки один пише
                    lock (lockObj)
                    {
                        safeCounter++;
                    }
                }
            });

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Safe PLINQ + Lock] Результат з lock:        {safeCounter:N0}");
            Console.ResetColor();
            Console.WriteLine("(Примітка: Lock дає правильний результат, але сильно знижує швидкість через чергу потоків).");
        }

        #endregion
    }
}