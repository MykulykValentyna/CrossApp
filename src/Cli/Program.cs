using System;
using System.IO;
using Core.Domain;
using Core.Events;
using Core.Services;
using Core.Abstractions;
using Core.Storage;
using Core.Import;
using Cli;

var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

INotificationSink[] sinks = [new ConsoleSink(), new FileSink(logPath)];
IBookStore store = new InMemoryBookStore(); 
var service = new LibraryService(store, sinks);

void OnLibraryChanged(object? sender, LibraryChangedEventArgs e)
{
    string line = e.Kind == LibraryChangeKind.CopyIssued 
        ? $"{e.At:HH:mm:ss} {e.Kind,-12} {e.CopyId} → видано читачу {e.ReaderId} до дати {e.DueDate:yyyy-MM-dd}"
        : $"{e.At:HH:mm:ss} {e.Kind,-12} {e.CopyId}";
        
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}

service.Changed += OnLibraryChanged;

int changes = 0;
service.Changed += (_, _) => changes++;

Console.WriteLine("Виконання успішних операцій");
var book1 = service.AddBook("COPY-101", "978-3-16-148410-0");
service.IssueBook(book1.Id, "Reader-42", DateTimeOffset.Now.AddDays(14));

Console.WriteLine($"\nПодій отримано: {changes}");

Console.WriteLine("\nВідписка");
service.Changed -= OnLibraryChanged;

var book2 = service.AddBook("COPY-102", "978-0-201-63361-0");
Console.WriteLine($"Подій отримано: {changes} (збільшилось лише завдяки лямбді)"); 

Console.WriteLine("\nСценарій помилки");
try 
{
    service.IssueBook(book1.Id, "Reader-99", DateTimeOffset.Now.AddDays(7));
} 
catch (Exception ex) 
{
    Console.WriteLine($"Помилка: {ex.Message}");
}

Console.WriteLine("\nРобота з Func");
var availableBooks = service.FindAll(b => !b.IsIssued);
Console.WriteLine($"Доступно книг у бібліотеці: {availableBooks.Count}");

Console.WriteLine("\nТест Action<string> (Імпорт)");
var importer = new BookImporter();
string[] csvData = 
[
    "COPY-201,978-0-201",
    "INVALID_LINE",
    ",", 
    "COPY-202,978-1-234"
];

var imported = importer.Import(csvData, skippedMsg => 
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(skippedMsg);
    Console.ResetColor();
});

Console.WriteLine($"Імпортовано успішно: {imported.Count}");