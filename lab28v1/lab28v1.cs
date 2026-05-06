using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace lab28v1
{
    // --- КЛАСИ ПРЕДМЕТНОЇ ОБЛАСТІ ---
    
    public class Author
    {
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Author Author { get; set; } = new Author();
        public int Year { get; set; }
    }

    // --- РЕПОЗИТОРІЙ З JSON-СЕРІАЛІЗАЦІЄЮ ---

    public class BookRepository
    {
        private List<Book> _books = new List<Book>();

        public void Add(Book book) => _books.Add(book);

        public List<Book> GetAll() => _books;

        public Book? GetById(int id) => _books.FirstOrDefault(b => b.Id == id);

        // Асинхронне збереження у файл
        public async Task SaveToFileAsync(string filename)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                await JsonSerializer.SerializeAsync(fs, _books, options);
            }
            Console.WriteLine("Дані успішно збережено у файл.");
        }

        // Асинхронне завантаження з файлу
        public async Task LoadFromFileAsync(string filename)
        {
            if (!File.Exists(filename)) return;

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                var result = await JsonSerializer.DeserializeAsync<List<Book>>(fs);
                _books = result ?? new List<Book>();
            }
            Console.WriteLine("Дані успішно завантажено з файлу.");
        }
    }

    // --- ГОЛОВНИЙ КЛАС ПРОГРАМИ ---

    class Program
    {
        static async Task Main(string[] args)
        {
            BookRepository repository = new BookRepository();
            string filePath = "library.json";

            // 1. Створення об'єктів
            repository.Add(new Book 
            { 
                Id = 1, 
                Title = "Kobzar", 
                Author = new Author { Name = "Taras Shevchenko", Country = "Ukraine" }, 
                Year = 1840 
            });
            repository.Add(new Book 
            { 
                Id = 2, 
                Title = "The Witcher", 
                Author = new Author { Name = "Andrzej Sapkowski", Country = "Poland" }, 
                Year = 1992 
            });

            // 2. Збереження у файл
            await repository.SaveToFileAsync(filePath);

            // 3. Очищення списку та завантаження з файлу для перевірки
            Console.WriteLine("\nОчищення локальних даних та завантаження з файлу...");
            await repository.LoadFromFileAsync(filePath);

            // 4. Вивід результату
            Console.WriteLine("\nСписок книг у бібліотеці:");
            foreach (var book in repository.GetAll())
            {
                Console.WriteLine($"[{book.Id}] \"{book.Title}\" - {book.Author.Name} ({book.Year})");
            }
        }
    }
}