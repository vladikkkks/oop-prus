using System;
using System.Collections.Generic;
using System.Text;

namespace lab22
{
    // Початкова структура (Порушення LSP)
    public class Employee
    {
        public string Name { get; set; }
        public virtual decimal CalculateSalary() => 15000m;
    }

    public class Volunteer : Employee
    {
        public override decimal CalculateSalary()
        {
            throw new NotSupportedException("Волонтери не отримують зарплату!");
        }
    }

    // Рефакторинг (Дотримання LSP)
    public interface IStaffMember
    {
        string Name { get; set; }
        void DisplayInfo();
    }

    public interface IPayable
    {
        decimal CalculateSalary();
    }

    public class SalariedEmployee : IStaffMember, IPayable
    {
        public string Name { get; set; }
        public void DisplayInfo() => Console.WriteLine($"Штатний співробітник: {Name}");
        public decimal CalculateSalary() => 20000m;
    }

    public class VolunteerStaff : IStaffMember
    {
        public string Name { get; set; }
        public void DisplayInfo() => Console.WriteLine($"Волонтер: {Name}");
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Демонстрація порушення
            Console.WriteLine("--- Тест порушення LSP ---");
            var employees = new List<Employee> 
            { 
                new Employee { Name = "Владислав" }, 
                new Volunteer { Name = "Олександр" } 
            };

            try
            {
                foreach (var emp in employees)
                {
                    Console.WriteLine($"{emp.Name}: {emp.CalculateSalary()} грн.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }

            Console.WriteLine("\n--- Тест дотримання LSP ---");
            var staff = new List<IStaffMember>
            {
                new SalariedEmployee { Name = "Марія" },
                new VolunteerStaff { Name = "Сергій" }
            };

            foreach (var member in staff)
            {
                member.DisplayInfo();
                if (member is IPayable payable)
                {
                    Console.WriteLine($"Нараховано: {payable.CalculateSalary()} грн.");
                }
            }

            Console.ReadKey();
        }
    }
}