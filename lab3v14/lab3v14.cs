using System;
using System.Collections.Generic;
class User
{
    public string Login { get; set; }
    public int Operations { get; protected set; }
    public User(string login)
    {
        Login = login;
        Operations = 0;
    }
    public virtual void PerformOperation()
    {
        Operations++;
        Console.WriteLine($"{Login} виконав базову операцію. Всього: {Operations}");
    }
    public virtual void ShowInfo()
    {
        Console.WriteLine($"Користувач: {Login}, Операцій: {Operations}");
    }
}
class Admin : User
{
    public Admin(string login) : base(login) { }

    public override void PerformOperation()
    {
        Operations += 2;
        Console.WriteLine($"[ADMIN] {Login} керує системою. Операцій: {Operations}");
    }

    public override void ShowInfo()
    {
        Console.WriteLine($"Адмін: {Login}, Активність: {Operations}");
    }
}
class Guest : User
{
    public Guest(string login) : base(login) { }

    public override void PerformOperation()
    {
        Operations++;
        Console.WriteLine($"[GUEST] {Login} переглянув сторінку. Операцій: {Operations}");
    }
    public override void ShowInfo()
    {
        Console.WriteLine($"Гість: {Login}, Перегляди: {Operations}");
    }
}
class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        List<User> users = new List<User>
        {
            new Admin("root_admin"),
            new Guest("guest123"),
            new Admin("moderator"),
            new Guest("anon_user")
        };
        users[0].PerformOperation();
        users[1].PerformOperation();
        users[0].PerformOperation();
        users[2].PerformOperation();
        users[2].PerformOperation();
        users[3].PerformOperation();

        Console.WriteLine("\n=== Інформація про користувачів ===");
        foreach (var u in users)
            u.ShowInfo();
        User mostActive = users[0];
        foreach (var u in users)
            if (u.Operations > mostActive.Operations)
                mostActive = u;

        Console.WriteLine($"\nНайактивніший користувач: {mostActive.Login} ({mostActive.Operations} операцій)");
    }
}
