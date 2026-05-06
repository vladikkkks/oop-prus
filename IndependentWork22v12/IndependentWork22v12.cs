using System;
using System.Collections.Generic;

namespace IndependentWork22
{
    // --- ПАТЕРН COMPOSITE ---

    // Спільний інтерфейс для всіх компонентів
    public interface IComponent
    {
        decimal GetAmount();
        string GetDescription();
    }

    // Leaf: Окрема транзакція
    public class SingleTransaction : IComponent
    {
        private string _description;
        private decimal _amount;

        public SingleTransaction(string description, decimal amount)
        {
            _description = description;
            _amount = amount;
        }

        public decimal GetAmount() => _amount;
        public string GetDescription() => _description;
    }

    // Composite: Групова транзакція (пакет)
    public class BatchTransaction : IComponent
    {
        private List<IComponent> _children = new List<IComponent>();
        private string _title;

        public BatchTransaction(string title)
        {
            _title = title;
        }

        public void Add(IComponent component) => _children.Add(component);
        public void Remove(IComponent component) => _children.Remove(component);

        public decimal GetAmount()
        {
            decimal total = 0;
            foreach (var child in _children)
                total += child.GetAmount();
            return total;
        }

        public string GetDescription() => $"{_title} (Batch)";
    }

    // --- ПАТЕРН DECORATOR ---

    // Абстрактний декоратор
    public abstract class TransactionDecorator : IComponent
    {
        protected IComponent _component;

        public TransactionDecorator(IComponent component)
        {
            _component = component;
        }

        public virtual decimal GetAmount() => _component.GetAmount();
        public virtual string GetDescription() => _component.GetDescription();
    }

    // Конкретний декоратор 1: Комісія
    public class FeeDecorator : TransactionDecorator
    {
        private decimal _feePercent;

        public FeeDecorator(IComponent component, decimal feePercent) : base(component)
        {
            _feePercent = feePercent;
        }

        public override decimal GetAmount()
        {
            // Додаємо комісію до суми
            decimal baseAmount = base.GetAmount();
            return baseAmount + (baseAmount * _feePercent / 100);
        }

        public override string GetDescription() => base.GetDescription() + " [+Fee]";
    }

    // Конкретний декоратор 2: Конвертація валют
    public class CurrencyConverterDecorator : TransactionDecorator
    {
        private decimal _rate;
        private string _currencyCode;

        public CurrencyConverterDecorator(IComponent component, decimal rate, string currencyCode) : base(component)
        {
            _rate = rate;
            _currencyCode = currencyCode;
        }

        public override decimal GetAmount() => base.GetAmount() * _rate;
        public override string GetDescription() => base.GetDescription() + $" [In {_currencyCode}]";
    }

    // --- MAIN ---

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Створюємо окремі транзакції (Leaf)
            var t1 = new SingleTransaction("Оплата Інтернету", 300);
            var t2 = new SingleTransaction("Покупка продуктів", 1200);
            var t3 = new SingleTransaction("Підписка Netflix", 400);

            // 2. Створюємо групову транзакцію (Composite)
            var dailyExpenses = new BatchTransaction("Щоденні витрати");
            dailyExpenses.Add(t1);
            dailyExpenses.Add(t2);

            var monthlyBatch = new BatchTransaction("Місячний звіт");
            monthlyBatch.Add(dailyExpenses); // Додаємо композіт у композіт
            monthlyBatch.Add(t3);

            // 3. Застосовуємо декоратори
            // Додаємо комісію 5% до окремої транзакції
            IComponent decoratedT1 = new FeeDecorator(t1, 5);

            // Конвертуємо весь пакет витрат у USD (курс 0.026) та додаємо комісію банку 2%
            IComponent usdBatch = new CurrencyConverterDecorator(monthlyBatch, 0.026m, "USD");
            IComponent finalReport = new FeeDecorator(usdBatch, 2);

            // 4. Вивід результатів
            Console.WriteLine("=== Фінансова система (Composite + Decorator) ===\n");

            PrintInfo(t1);
            PrintInfo(decoratedT1);
            Console.WriteLine("-------------------------------------------------");
            PrintInfo(monthlyBatch);
            PrintInfo(finalReport);
        }

        static void PrintInfo(IComponent component)
        {
            Console.WriteLine($"Опис: {component.GetDescription()}");
            Console.WriteLine($"Сума: {component.GetAmount():F2}");
            Console.WriteLine();
        }
    }
}