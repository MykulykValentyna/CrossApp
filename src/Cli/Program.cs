using Core.Domain;
using Core.Dto;
using System;
using System.Collections.Generic;

Console.WriteLine("=== Сценарій 1: успіх ===");
BookCopy copy = BookCopy.Create("COPY-123", "978-3-16-148410-0");
Console.WriteLine(copy);

DateTime issueDate = DateTime.Now.AddDays(-5);
Loan loan = Loan.Open(copy, "READER-001", issueDate, 0); 
Console.WriteLine($"[Успіх] Книгу видано. Стан примірника: {copy}");

loan.Close(DateTime.Now, copy);
Console.WriteLine($"[Успіх] Книгу повернуто. Стан примірника: {copy}");
Console.WriteLine();


Console.WriteLine("=== Сценарій 2: порушення інваріантів ===");
BookCopy brokenCopy = BookCopy.Create("COPY-999", "978-0-12-345678-9");

TryDo("Порожній ISBN при створенні", () => BookCopy.Create("COPY-002", "   "));

TryDo("Повторна видача вже виданого примірника", () => 
{
    Loan.Open(brokenCopy, "READER-002", DateTime.Now, 0);
    brokenCopy.Issue();
});

TryDo("Дата повернення раніше дати видачі", () => 
{
    BookCopy tempCopy = BookCopy.Create("COPY-003", "978-1-23-456789-0");
    Loan tempLoan = Loan.Open(tempCopy, "READER-003", DateTime.Now, 0);
    tempLoan.Close(DateTime.Now.AddDays(-1), tempCopy); 
});

TryDo("Спроба закрити вже закриту видачу", () => 
{
    BookCopy tempCopy = BookCopy.Create("COPY-004", "111-2-33-456789-0");
    Loan tempLoan = Loan.Open(tempCopy, "READER-004", DateTime.Now.AddDays(-2), 0);
    tempLoan.Close(DateTime.Now, tempCopy);
    tempLoan.Close(DateTime.Now, tempCopy);
});

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"  {title}: виняток НЕ спрацював — інваріант відсутній!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  {title}: {ex.GetType().Name} — {ex.Message}");
    }
}

Console.WriteLine("\n=== ДОДАТКОВІ ЗАВДАННЯ ===");

Console.WriteLine("--- 1. Імпорт з помилками ---");
var mockDtos = new List<BookCopyDto>
{
    new BookCopyDto("C-01", "123-456", false), 
    new BookCopyDto("C-02", " ", false)       
};

var importResult = Core.Import.EntityMapper.MapBookCopies(mockDtos);
Console.WriteLine($"Успішно відновлено сутностей: {importResult.Entities.Count}");
Console.WriteLine($"Кількість помилок інваріантів: {importResult.Errors.Count}");
Console.WriteLine($"Текст помилки: {importResult.Errors[0]}");

Console.WriteLine("\n--- 2. Інваріант на дві сутності (Ліміт) ---");
var freshCopy = BookCopy.Create("C-03", "999-999");
try
{
    Console.WriteLine("Спроба видати книгу читачу, у якого вже 5 видач...");
    Loan.Open(freshCopy, "READER-OVERLOAD", DateTime.Now, activeLoansCount: 5); 
}
catch (Exception ex)
{
    Console.WriteLine($"УСПІХ! Виняток: {ex.Message}");
}

Console.WriteLine("\n--- 3. Явний стан-перелічування ---");
Order myOrder = Order.Create("ORD-555");
Console.WriteLine($"Поточний стан: {myOrder.Status}");

myOrder.ChangeStatus(OrderStatus.Confirmed);
Console.WriteLine($"Після підтвердження: {myOrder.Status}");

try
{
    Console.WriteLine("Спроба скасувати підтверджене замовлення...");
    myOrder.ChangeStatus(OrderStatus.Cancelled);
}
catch (Exception ex)
{
    Console.WriteLine($"УСПІХ! Виняток машини станів: {ex.Message}");
}