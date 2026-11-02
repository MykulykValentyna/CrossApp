using Core.Domain;
using Core.Dto;

namespace Core;

public static class SampleData
{
    public static IReadOnlyList<BookCopy> Books { get; } =
    [
        CreateBook("B001", "978-1", "Кобзар", "Поезія", 12, true),
        CreateBook("B002", "978-2", "Тигролови", "Пригоди", 8, true),
        CreateBook("B003", "978-3", "Місто", "Роман", 15, true),
        CreateBook("B004", "978-4", "Лісова пісня", "Драма", 4, true),
        CreateBook("B005", "978-5", "Захар Беркут", "Історичний роман", 11, true),
        CreateBook("B006", "978-6", "Кайдашева сім'я", "Класика", 6, true),
        CreateBook("B007", "978-7", "Маруся Чурай", "Історичний роман", 9, true),
        CreateBook("B008", "978-8", "Зачарована Десна", "Автобіографічна проза", 5, true),
        CreateBook("B009", "978-9", "Чорна рада", "Історичний роман", 7, true),
        CreateBook("B010", "978-10", "Intermezzo", "Новела", 3, true),
        CreateBook("B011", "978-11", "Майстер корабля", "Роман", 10, true),
        CreateBook("B012", "978-12", "Енеїда", "Поезія", 14, true),
        CreateBook("B013", "978-13", "Людина", "Роман", 2, false),
        CreateBook("B014", "978-14", "Украдене щастя", "Драма", 6, false),
        CreateBook("B015", "978-15", "Земля", "Класика", 13, false),
        CreateBook("B016", "978-16", "Наталка Полтавка", "Драма", 4, false),
        CreateBook("B017", "978-17", "Лебеді материнства", "Поезія", 8, false),
        CreateBook("B018", "978-18", "Пригоди Тома Сойєра", "Пригоди", 16, false)
    ];

    public static IReadOnlyList<Loan> Loans { get; } =
    [
        CreateLoan("L001", "B001", "Reader-A", -40, -12),
        CreateLoan("L002", "B002", "Reader-A", -35, -5),
        CreateLoan("L003", "B003", "Reader-A", -20, 12),
        CreateLoan("L004", "B004", "Reader-A", -18, 8),
        CreateLoan("L005", "B005", "Reader-A", -15, 15),
        CreateLoan("L006", "B006", "Reader-B", -30, -3),
        CreateLoan("L007", "B007", "Reader-B", -25, 10),
        CreateLoan("L008", "B008", "Reader-B", -22, 14),
        CreateLoan("L009", "B009", "Reader-B", -17, 7),
        CreateLoan("L010", "B010", "Reader-C", -28, -8),
        CreateLoan("L011", "B011", "Reader-C", -21, 9),
        CreateLoan("L012", "B012", "Reader-C", -16, 16),
        CreateLoan("L013", "B013", "Reader-D", -100, -70, -65),
        CreateLoan("L014", "B014", "Reader-D", -90, -60, -55),
        CreateLoan("L015", "B015", "Reader-E", -80, -50, -45),
        CreateLoan("L016", "B016", "Reader-E", -70, -40, -35),
        CreateLoan("L017", "B017", "Reader-F", -60, -30, -25),
        CreateLoan("L018", "B018", "Reader-F", -50, -20, -15)
    ];

    private static BookCopy CreateBook(
        string id,
        string isbn,
        string title,
        string genre,
        int quantity,
        bool isIssued)
    {
        var book = BookCopy.Create(id, isbn, title, genre, quantity);
        if (isIssued)
            book.Issue();

        return book;
    }

    private static Loan CreateLoan(
        string id,
        string copyId,
        string readerId,
        int issuedDaysAgo,
        int dueDaysFromToday,
        int? returnedDaysAgo = null)
    {
        DateTime issuedOn = DateTime.Today.AddDays(issuedDaysAgo);
        DateTime? returnedOn = returnedDaysAgo.HasValue
            ? DateTime.Today.AddDays(returnedDaysAgo.Value)
            : null;

        return Loan.FromDto(new LoanDto(
            id,
            copyId,
            readerId,
            issuedOn,
            returnedOn,
            DateTime.Today.AddDays(dueDaysFromToday)));
    }
}
