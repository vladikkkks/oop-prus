using System;
using System.Collections.Generic;
using System.Threading;

namespace IndependentWork23
{
    // --- ПАТЕРН ADAPTER ---
    // Мета: пристосувати старий обробник подій до нового інтерфейсу

    public class EventData { public string Message { get; set; } = ""; }

    // Adaptee (Клас з несумісним інтерфейсом)
    public class OldEventListener
    {
        public void OnEvent(EventData data)
        {
            Console.WriteLine($"[ADAPTEE] Стара система отримала подію: {data.Message}");
        }
    }

    // Target (Інтерфейс, який очікує сучасний клієнт)
    public interface IEventHandler
    {
        void Handle(string message);
    }

    // Adapter
    public class OldEventAdapter : IEventHandler
    {
        private readonly OldEventListener _oldListener;

        public OldEventAdapter(OldEventListener oldListener)
        {
            _oldListener = oldListener;
        }

        public void Handle(string message)
        {
            // Перетворюємо string у EventData, як того очікує стара бібліотека
            var data = new EventData { Message = message };
            _oldListener.OnEvent(data);
        }
    }

    // --- ПАТЕРН FACADE ---
    // Мета: надати спрощений інтерфейс для складної підсистеми обробки

    public class EventSource { public void RaiseEvent(string msg) => Console.WriteLine($"[FACADE] Подія ініційована: {msg}"); }
    public class Logger { public void Log(string msg) => Console.WriteLine($"[FACADE] Лог: {msg}"); }
    public class Notifier { public void Notify(string msg) => Console.WriteLine($"[FACADE] Сповіщення надіслано: {msg}"); }

    public class EventProcessingFacade
    {
        private readonly EventSource _source = new();
        private readonly Logger _logger = new();
        private readonly Notifier _notifier = new();

        public void ProcessEvent(string message)
        {
            Console.WriteLine("\n--- Facade: Початок комплексної обробки ---");
            _source.RaiseEvent(message);
            _logger.Log(message);
            _notifier.Notify(message);
            Console.WriteLine("--- Facade: Обробка завершена ---");
        }
    }

    // --- ПАТЕРН PROXY ---
    // Мета: обмежити частоту публікації подій (Rate Limiting)

    public interface IEventPublisher
    {
        void Publish(string message);
    }

    public class RealEventPublisher : IEventPublisher
    {
        public void Publish(string message)
        {
            Console.WriteLine($"[REAL SUBJECT] Подія '{message}' опублікована в шину даних.");
        }
    }

    public class RateLimitEventPublisherProxy : IEventPublisher
    {
        private readonly IEventPublisher _realPublisher;
        private DateTime _lastRequestTime;
        private readonly TimeSpan _minInterval = TimeSpan.FromSeconds(2);

        public RateLimitEventPublisherProxy(IEventPublisher realPublisher)
        {
            _realPublisher = realPublisher;
            _lastRequestTime = DateTime.MinValue;
        }

        public void Publish(string message)
        {
            if (DateTime.Now - _lastRequestTime < _minInterval)
            {
                Console.WriteLine($"[PROXY] ВІДХИЛЕНО: Занадто часто! Почекайте {(_minInterval - (DateTime.Now - _lastRequestTime)).TotalSeconds:F1} сек.");
                return;
            }

            _lastRequestTime = DateTime.Now;
            _realPublisher.Publish(message);
        }
    }

    // --- MAIN ---

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Демонстрація ADAPTER
            Console.WriteLine("=== 1. ПАТЕРН ADAPTER ===");
            IEventHandler handler = new OldEventAdapter(new OldEventListener());
            handler.Handle("Системна помилка 404");

            // 2. Демонстрація FACADE
            Console.WriteLine("\n=== 2. ПАТЕРН FACADE ===");
            var facade = new EventProcessingFacade();
            facade.ProcessEvent("Оновлення безпеки");

            // 3. Демонстрація PROXY (Rate Limiting)
            Console.WriteLine("\n=== 3. ПАТЕРН PROXY (Обмеження частоти) ===");
            IEventPublisher publisher = new RateLimitEventPublisherProxy(new RealEventPublisher());
            
            publisher.Publish("Перша подія"); // Пройде
            publisher.Publish("Друга подія (швидка)"); // Буде відхилено
            
            Console.WriteLine("... очікування 2 секунди ...");
            Thread.Sleep(2100);
            
            publisher.Publish("Третя подія (після паузи)"); // Пройде
        }
    }
}