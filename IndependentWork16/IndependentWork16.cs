using System;

namespace IndependentWork16
{
    // --- 1. Інтерфейси (Абстракції) ---
    public interface ICredentialValidator 
    { 
        bool Validate(string username, string password); 
    }

    public interface ITokenGenerator 
    { 
        string GenerateToken(string username); 
    }

    public interface ILogger 
    { 
        void Log(string message); 
    }

    // --- 2. Реалізації інтерфейсів (Concrete Classes) ---
    public class CredentialValidator : ICredentialValidator
    {
        public bool Validate(string username, string password)
        {
            // Проста перевірка: логін не порожній, пароль від 6 символів
            return !string.IsNullOrEmpty(username) && password.Length >= 6;
        }
    }

    public class JwtTokenGenerator : ITokenGenerator
    {
        public string GenerateToken(string username)
        {
            // Імітація генерації токена
            return $"auth_token_{username}_{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[LOG {DateTime.Now:HH:mm:ss}]: {message}");
        }
    }

    // --- 3. Рефакторинговий AuthService (Дотримується SRP та DIP) ---
    public class AuthService
    {
        private readonly ICredentialValidator _validator;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger _logger;

        // Залежності передаються через конструктор (Dependency Injection)
        public AuthService(ICredentialValidator validator, ITokenGenerator tokenGenerator, ILogger logger)
        {
            _validator = validator;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }

        public void Login(string username, string password)
        {
            _logger.Log($"Attempting login for user: {username}");

            if (!_validator.Validate(username, password))
            {
                _logger.Log("Login failed: Validation error (invalid format).");
                Console.WriteLine("Error: Invalid username or password format.");
                return;
            }

            // Умовно вважаємо, що якщо валідація пройшла — створюємо токен
            string token = _tokenGenerator.GenerateToken(username);
            
            _logger.Log($"User '{username}' logged in successfully.");
            Console.WriteLine($"SUCCESS: Welcome, {username}!");
            Console.WriteLine($"Your Access Token: {token}");
        }
    }

    // --- 4. Точка входу в програму ---
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Створюємо екземпляри компонентів
            var validator = new CredentialValidator();
            var tokenGen = new JwtTokenGenerator();
            var logger = new ConsoleLogger();

            // Збираємо сервіс (Впровадження залежностей)
            var authService = new AuthService(validator, tokenGen, logger);

            Console.WriteLine("=== Самостійна робота №16 (Варіант 14) ===\n");

            // Тест 1: Невдала валідація
            Console.WriteLine("--- Тест 1: Короткий пароль ---");
            authService.Login("admin", "123"); 

            Console.WriteLine();

            // Тест 2: Успішний вхід
            Console.WriteLine("--- Тест 2: Коректні дані ---");
            authService.Login("RCIT_Student", "super_secret_password");

            Console.WriteLine("\nРоботу виконав: Прус Владислав, група ІПЗ-3/1");
            Console.ReadKey();
        }
    }
}