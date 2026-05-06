using Xunit;
using IndependentWork19; // Singleton & Factory (ID)
using IndependentWork20; // Strategy & Observer (Weather)

namespace IndependentWork21.Tests
{
    public class IntegrationPatternsTests
    {
        // --- ПОЗИТИВНІ СЦЕНАРІЇ ---

        [Fact]
        public void FullCycle_FactoryAndStrategy_Success()
        {
            // 1. Перевірка Factory + Singleton (ID)
            var idService = IdService.Instance;
            idService.SetFactory(new SequentialFactory());
            string id1 = idService.GetNextId();
            
            // 2. Перевірка Strategy (Weather)
            var context = new DataContext(new CelsiusToFahrenheitStrategy());
            // Очікуємо, що система відпрацює без помилок
            context.ExecuteProcessing("20");

            Assert.Equal("00001", id1);
        }

        [Fact]
        public void Observer_MultipleSubscribers_ReceiveNotifications()
        {
            var publisher = new DataPublisher();
            int notificationCount = 0;

            publisher.DataProcessed += (data) => notificationCount++;
            publisher.DataProcessed += (data) => notificationCount++;

            publisher.PublishDataProcessed("Test Data");

            Assert.Equal(2, notificationCount);
        }

        [Fact]
        public void Singleton_StatePersistence_AcrossCalls()
        {
            var instance1 = IdService.Instance;
            var instance2 = IdService.Instance;
            
            instance1.SetFactory(new SequentialFactory());
            instance1.GetNextId(); // 00001
            
            Assert.Same(instance1, instance2);
            Assert.Equal("00002", instance2.GetNextId());
        }

        // --- НЕГАТИВНІ / ГРАНИЧНІ СЦЕНАРІЇ ---

        [Fact]
        public void Strategy_InvalidInputData_ShouldNotCrash()
        {
            var context = new DataContext(new CelsiusToFahrenheitStrategy());
            
            // Граничний випадок: некоректні дані (текст замість числа)
            var exception = Record.Exception(() => context.ExecuteProcessing("НЕ_ЧИСЛО"));
            
            Assert.Null(exception); // Система має обробити помилку всередині, не падаючи
        }

        [Fact]
        public void IdService_NoFactorySet_ThrowsException()
        {
            // Скидаємо стан (в ідеалі через рефлексію або новий процес, 
            // але для тесту припустимо, що фабрика не була встановлена)
            var service = IdService.Instance;
            // Спеціально не викликаємо SetFactory
            
            Assert.Throws<System.Exception>(() => service.GetNextId());
        }
    }
}