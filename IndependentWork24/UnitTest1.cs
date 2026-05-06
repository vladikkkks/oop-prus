using Xunit;
using IndependentWork24; // Підключаємо простір імен твого проєкту
using System.Diagnostics;

namespace IndependentWork24.Tests
{
    public class IntegrationTests
    {
        [Fact] 
        public void Composite_CalculatesSumCorrectly()
        {
            // Перевірка, чи правильно сумуються транзакції в групі
            var group = new TransactionGroup("Root");
            group.Add(new Transaction("T1", 100));
            group.Add(new Transaction("T2", 200));
            
            Assert.Equal(300, group.GetAmount());
        }

        [Fact] 
        public void Decorator_AppliesTaxToGroup()
        {
            // Перевірка, чи працює декоратор податку на цілу групу
            var group = new TransactionGroup("Root");
            group.Add(new Transaction("T1", 100));
            var taxedGroup = new TaxDecorator(group);
            
            Assert.Equal(120, taxedGroup.GetAmount());
        }

        [Fact] 
        public void Proxy_CachesValueSuccessfully()
        {
            // Перевірка кешування: другий виклик має бути значно швидшим
            var group = new TransactionGroup("Heavy");
            group.Add(new Transaction("T1", 100));
            var proxy = new CachedAmountProxy(group);

            var sw = Stopwatch.StartNew();
            proxy.GetAmount(); // 1-й виклик (імітація затримки)
            sw.Stop();
            var firstCall = sw.ElapsedMilliseconds;

            sw.Restart();
            proxy.GetAmount(); // 2-й виклик (має взяти з кешу)
            sw.Stop();
            var secondCall = sw.ElapsedMilliseconds;

            // Другий виклик має бути майже миттєвим (0-1 мс)
            Assert.True(secondCall < firstCall);
        }

        [Fact] 
        public void Composite_EmptyGroup_ReturnsZero()
        {
            // Граничний кейс: сума порожньої групи має бути 0
            var emptyGroup = new TransactionGroup("Empty");
            Assert.Equal(0, emptyGroup.GetAmount());
        }
    }
}