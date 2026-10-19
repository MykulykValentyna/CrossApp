using System;
using Cli;
using Core.Abstractions;
using Core.Services;

IBookStore store = StoreFactory.Create(args); 

var service = new LendingService(store);

Console.WriteLine($"Сховище: {store.GetType().Name}\n");

try 
{
    Console.WriteLine("--- Виконання успішних операцій ---");
    
    var copy1 = service.AddBook("978-0132350884");
    Console.WriteLine($"[ДОДАНО]: {copy1}");

    var copy2 = service.AddBook("978-0131103627");
    Console.WriteLine($"[ДОДАНО]: {copy2}");

    var copy3 = service.AddBook("978-0201835953");
    Console.WriteLine($"[ДОДАНО]: {copy3}");

    service.IssueCopy(copy1.Id);
    Console.WriteLine($"[ВИДАНО]: Примірник з id={copy1.Id}");

    Console.WriteLine("\n--- Перевірка пошуку вільних примірників ---");
    var availableBooks = service.FindAvailable();
    Console.WriteLine($"Знайдено доступних примірників: {availableBooks.Count}");
    foreach (var b in availableBooks)
    {
        Console.WriteLine($"  {b}");
    }

    Console.WriteLine("\n--- Спроба виконати недопустиму операцію ---");
    
    service.IssueCopy("invalid-id-123"); 
}
catch (InvalidOperationException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[ПОМИЛКА БІЗНЕС-ЛОГІКИ]: {ex.Message}");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[НЕОЧІКУВАНА ПОМИЛКА]: {ex.Message}");
    Console.ResetColor();
}

Console.WriteLine("\n--- Поточний стан бібліотеки ---");
var allCopies = service.All();

if (allCopies.Count == 0)
{
    Console.WriteLine("  Бібліотека порожня.");
}
else
{
    foreach (var b in allCopies) 
    {
        Console.WriteLine($"  {b}"); 
    }
}