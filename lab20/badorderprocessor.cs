public class OrderProcessor
{
    public void ProcessOrder(Order order)
    {
        if (order.TotalAmount <= 0)
        {
            Console.WriteLine("Помилка: Сума замовлення повинна бути більшою за нуль.");
            return;
        }
        Console.WriteLine("Підключення до бази даних...");
        Console.WriteLine($"Виконання запиту: INSERT INTO Orders (Id, Customer, Amount) VALUES ({order.Id}, '{order.CustomerName}', {order.TotalAmount})");
        Console.WriteLine("Запис у базі даних успішно створено.");
        Console.WriteLine("Відкриття SMTP-з'єднання з поштовим сервером...");
        Console.WriteLine($"Відправка листа для: {order.CustomerName}");
        Console.WriteLine("Текст: Ваше замовлення прийнято та обробляється.");
        Console.WriteLine("Email успішно відправлено.");
        order.Status = OrderStatus.Processed;
        Console.WriteLine($"Статус замовлення змінено на: {order.Status}");
    }
}