# Принципи ISP та DIP: Шлях до чистої архітектури

Цей документ містить аналіз двох ключових принципів SOLID, які допомагають створювати гнучкі та тестовані системи: **Interface Segregation Principle** (ISP) та **Dependency Inversion Principle** (DIP).

---

## 1. Interface Segregation Principle (ISP)
**Принцип розділення інтерфейсу** стверджує: *Клієнти не повинні залежати від методів, які вони не використовують.*

### Приклад порушення (Fat Interface)
Уявімо інтерфейс для багатофункціонального пристрою (МФУ), який змушує простий принтер реалізовувати непотрібні йому функції.

```csharp
// Порушення ISP: інтерфейс занадто "товстий"
public interface IMachine {
    void Print();
    void Scan();
    void Fax();
}

public class OldSchoolPrinter : IMachine {
    public void Print() => Console.WriteLine("Друк документа...");
    
    // Проблема: клас змушений реалізувати методи, які він не підтримує
    public void Scan() => throw new NotImplementedException("Не підтримується!");
    public void Fax() => throw new NotImplementedException("Не підтримується!");
}