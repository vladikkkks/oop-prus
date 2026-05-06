using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IndependentWork24
{
    // --- ПАТЕРН COMPOSITE ---
    public interface IComponent
    {
        decimal GetAmount();
        string GetName();
    }

    public class Transaction : IComponent
    {
        private readonly decimal _amount;
        private readonly string _name;

        public Transaction(string name, decimal amount)
        {
            _name = name;
            _amount = amount;
        }

        public decimal GetAmount() 
        {
            // Імітація важких обчислень
            Thread.Sleep(50); 
            return _amount; 
        }
        public string GetName() => _name;
    }

    public class TransactionGroup : IComponent
    {
        private readonly List<IComponent> _children = new();
        private readonly string _name;

        public TransactionGroup(string name) => _name = name;
        public void Add(IComponent c) => _children.Add(c);

        public decimal GetAmount() => _children.Sum(c => c.GetAmount());
        public string GetName() => _name;
    }

    // --- ПАТЕРН DECORATOR ---
    public abstract class AmountDecorator : IComponent
    {
        protected IComponent _component;
        protected AmountDecorator(IComponent c) => _component = c;
        public virtual decimal GetAmount() => _component.GetAmount();
        public virtual string GetName() => _component.GetName();
    }

    public class TaxDecorator : AmountDecorator
    {
        public TaxDecorator(IComponent c) : base(c) { }
        public override decimal GetAmount() => base.GetAmount() * 1.20m; // +20% податку
        public override string GetName() => base.GetName() + " (Taxed)";
    }

    // --- ПАТЕРН PROXY (Caching) ---
    public class CachedAmountProxy : IComponent
    {
        private readonly IComponent _realComponent;
        private decimal? _cachedValue;

        public CachedAmountProxy(IComponent realComponent) => _realComponent = realComponent;

        public decimal GetAmount()
        {
            if (_cachedValue == null)
            {
                _cachedValue = _realComponent.GetAmount();
            }
            return _cachedValue.Value;
        }

        public string GetName() => _realComponent.GetName() + " [Cached]";
        public void ResetCache() => _cachedValue = null;
    }
}