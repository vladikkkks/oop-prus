using System;
using System.Collections.Generic;

namespace lab20
{
    public enum OrderStatus
    {
        New,
        PendingValidation,
        Processed,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order(int id, string customerName, decimal totalAmount)
        {
            Id = id;
            CustomerName = customerName;
            TotalAmount = totalAmount;
            Status = OrderStatus.New;
        }
    }

    public interface IOrderValidator
    {
        bool IsValid(Order order);
    }

    public interface IOrderRepository
    {
        void Save(Order order);
        Order GetById(int id);
    }

    public interface IEmailService
    {
        void SendOrderConfirmation(Order order);
    }

    public class SimpleOrderValidator : IOrderValidator
    {
        public bool IsValid(Order order) => order.TotalAmount > 0;
    }

    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders = new List<Order>();
        public void Save(Order order)
        {
            _orders.Add(order);
            Console.WriteLine($"[БД] Замовлення #{order.Id} збережено.");
        }
        public Order GetById(int id) => _orders.Find(o => o.Id == id);
    }

    public class ConsoleEmailService : IEmailService
    {
        public void SendOrderConfirmation(Order order)
        {
            Console.WriteLine($"[Email] Підтвердження надіслано клієнту {order.CustomerName}.");
        }
    }

    public class OrderService
    {
        private readonly IOrderValidator _validator;
        private readonly IOrderRepository _repository;
        private readonly IEmailService _emailService;

        public OrderService(IOrderValidator validator, IOrderRepository repository, IEmailService emailService)
        {
            _validator = validator;
            _repository = repository;
            _emailService = emailService;
        }

        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"\nОбробка замовлення #{order.Id}...");

            if (!_validator.IsValid(order))
            {
                Console.WriteLine($"[Помилка] Замовлення #{order.Id} невалідну (сума: {order.TotalAmount}).");
                order.Status = OrderStatus.Cancelled;
                return;
            }

            order.Status = OrderStatus.Processed;
            _repository.Save(order);
            _emailService.SendOrderConfirmation(order);
            
            Console.WriteLine($"Успішно: Статус замовлення #{order.Id} змінено на {order.Status}.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var validator = new SimpleOrderValidator();
            var repository = new InMemoryOrderRepository();
            var emailService = new ConsoleEmailService();
            var orderService = new OrderService(validator, repository, emailService);

            Order validOrder = new Order(1, "Тест Валідний", 500.00m);
            orderService.ProcessOrder(validOrder);

            Order invalidOrder = new Order(2, "Тест Невалідний", -100.00m);
            orderService.ProcessOrder(invalidOrder);

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}