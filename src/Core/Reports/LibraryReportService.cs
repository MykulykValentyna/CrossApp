using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domain;

namespace Core.Reports;

public static class LibraryReportService
{
    public static IReadOnlyList<ReaderReport> TopReaders(IEnumerable<Loan> loans, int n) =>
        loans.GroupBy(loan => loan.ReaderId, StringComparer.OrdinalIgnoreCase)
             .Select(group => new ReaderReport(group.Key, group.Count()))
             .OrderByDescending(report => report.LoanCount)
             .ThenBy(report => report.ReaderId, StringComparer.Ordinal)
             .Take(Math.Max(0, n))
             .ToList();

    public static IReadOnlyList<OverdueLoanReport> OverdueLoans(
        IEnumerable<BookCopy> books,
        IEnumerable<Loan> loans)
    {
        ArgumentNullException.ThrowIfNull(books);
        ArgumentNullException.ThrowIfNull(loans);

        var overdueLoans = loans.Where(loan => !loan.IsClosed && loan.DueDate < DateTime.Today);
        return overdueLoans.Join(
                books,
                loan => loan.CopyId,
                book => book.Id,
                (loan, book) => new OverdueLoanReport(
                    loan.Id,
                    book.Title,
                    loan.ReaderId,
                    loan.DueDate))
            .OrderBy(report => report.DueDate)
            .ThenBy(report => report.Id, StringComparer.Ordinal)
            .ToList();
    }

    public static IReadOnlyList<GroupSummary> BooksByGenre(IEnumerable<BookCopy> books) =>
        books.GroupBy(book => book.Genre, StringComparer.OrdinalIgnoreCase)
             .Select(group => new GroupSummary(
                 group.Key,
                 group.Count(),
                 group.Sum(book => book.Quantity)))
             .OrderByDescending(summary => summary.TotalQuantity)
             .ThenBy(summary => summary.Key, StringComparer.Ordinal)
             .ToList();

    public static (int Items, int Total, double Average, int Max) Totals(IEnumerable<BookCopy> books)
    {
        ArgumentNullException.ThrowIfNull(books);
        var quantities = books.Select(book => book.Quantity).ToList();

        return quantities.Count == 0
            ? (0, 0, 0d, 0)
            : (
                quantities.Count,
                quantities.Sum(),
                quantities.Average(),
                quantities.Max());
    }

    public static (int BeforeChange, int AfterChange, int SnapshotCount) DeferredExecutionDemo(
        IEnumerable<BookCopy> books)
    {
        ArgumentNullException.ThrowIfNull(books);
        var source = books.ToList();
        var query = source.Where(book => book.Quantity < 10);
        int beforeChange = query.Count();
        source.Add(BookCopy.Create(
            "deferred-demo",
            "DEMO-ISBN",
            "Демо-книга",
            "Демонстрація",
            3));
        int afterChange = query.Count();
        var snapshot = query.ToList();
        source.Clear();
        return (beforeChange, afterChange, snapshot.Count);
    }
}
