using System;

namespace lab30v5
{
    public class MathHelper
    {
        // Перевірка на просте число
        public bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;
            
            for (int i = 3; i <= Math.Sqrt(number); i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        // Обчислення факторіалу
        public long Factorial(int n)
        {
            if (n < 0) 
                throw new ArgumentException("Факторіал не визначений для від'ємних чисел.");
            if (n == 0 || n == 1) 
                return 1;
            
            long result = 1;
            for (int i = 2; i <= n; i++) 
                result *= i;
                
            return result;
        }

        // Знаходження n-го числа Фібоначчі
        public long Fibonacci(int n)
        {
            if (n < 0) 
                throw new ArgumentException("Індекс Фібоначчі не може бути від'ємним.");
            if (n == 0) return 0;
            if (n == 1) return 1;
            
            long a = 0, b = 1;
            for (int i = 2; i <= n; i++)
            {
                long temp = a + b;
                a = b;
                b = temp;
            }
            return b;
        }

        // Найбільший спільний дільник (НСД) за алгоритмом Евкліда
        public int GCD(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
    }
}