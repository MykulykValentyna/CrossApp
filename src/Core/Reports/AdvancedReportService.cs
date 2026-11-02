using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domain;

namespace Core.Reports;

public static class AdvancedReportService
{
    public static IReadOnlyList<BookLoanDetails> GetLoanDetails(
        IEnumerable<BookCopy> books,
        IEnumerable<Loan> loans) =>
        books.Join(
            loans,
            book => book.Id,
            loan => loan.CopyId,
            (book, loan) => new BookLoanDetails(book.Title, loan.ReaderId, loan.DueDate))
        .OrderBy(detail => detail.DueDate)
        .ThenBy(detail => detail.ReaderId, StringComparer.Ordinal)
        .ToList();

    public static IReadOnlyList<ReaderReport> TopReadersCustom(IEnumerable<Loan> loans, int n) =>
        loans.GroupBy(loan => loan.ReaderId, StringComparer.OrdinalIgnoreCase)
             .Select(group => new ReaderReport(group.Key, group.Count()))
             .TopBy(
                 report => report.LoanCount,
                 report => report.ReaderId,
                 n,
                 thenByComparer: StringComparer.Ordinal)
             .ToList();

    public static IReadOnlyList<GroupSummary> TopGenresCustom(IEnumerable<BookCopy> books, int n) =>
        books.GroupBy(book => book.Genre, StringComparer.OrdinalIgnoreCase)
             .Select(group => new GroupSummary(
                 group.Key,
                 group.Count(),
                 group.Sum(book => book.Quantity)))
             .TopBy(
                 summary => summary.TotalQuantity,
                 summary => summary.Key,
                 n,
                 thenByComparer: StringComparer.Ordinal)
             .ToList();

    public static IReadOnlyList<GroupSummary> BooksByGenreQuerySyntax(IEnumerable<BookCopy> books)
    {
        var query =
            from book in books
            group book by book.Genre into genreGroup
            orderby genreGroup.Sum(item => item.Quantity) descending, genreGroup.Key
            select new GroupSummary(
                genreGroup.Key,
                genreGroup.Count(),
                genreGroup.Sum(item => item.Quantity));

        return query.ToList();
    }
}
