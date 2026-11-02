using Core.Domain;
using Core;
using Core.Reports;

namespace Cli;

internal static class Program
{
    private static void Main(string[] args)
    {
        var store = StoreFactory.Create(args);
        var books = store.List();
        IReadOnlyList<Loan> loans = SampleData.Loans;

        string command = args.FirstOrDefault(argument =>
            !argument.StartsWith("--", StringComparison.Ordinal)) ?? "report-top";

        switch (command)
        {
            case "report-top":
            case "report-readers":
                Print(AdvancedReportService.TopReadersCustom(loans, 5));
                break;

            case "report-low":
            case "report-overdue":
                Print(LibraryReportService.OverdueLoans(books, loans));
                break;

            case "report-loan-details":
                Print(AdvancedReportService.GetLoanDetails(books, loans));
                break;

            case "report-groups":
            case "report-genres":
                Print(LibraryReportService.BooksByGenre(books));
                break;

            case "report-top-genres":
                Print(AdvancedReportService.TopGenresCustom(books, 5));
                break;

            case "report-sum":
            case "report-totals":
                var totals = LibraryReportService.Totals(books);
                Console.WriteLine(
                    $"Книг: {totals.Items}, примірників: {totals.Total}, " +
                    $"середня кількість примірників: {totals.Average:F2}, максимум: {totals.Max}");
                break;

            case "report-deferred":
                var deferred = LibraryReportService.DeferredExecutionDemo(books);
                Console.WriteLine($"Дефіцитних до зміни джерела: {deferred.BeforeChange}");
                Console.WriteLine($"Дефіцитних після зміни джерела: {deferred.AfterChange}");
                Console.WriteLine($"У матеріалізованому знімку: {deferred.SnapshotCount}");
                break;

            case "report-performance":
                PerformanceTest.Run();
                break;

            default:
                Console.WriteLine(
                    "Команди: report-top | report-low | report-groups | report-sum | " +
                    "report-deferred | report-loan-details | report-top-genres | report-performance");
                break;
        }
    }

    private static void Print(IReadOnlyList<ReaderReport> reports)
    {
        if (reports.Count == 0)
        {
            Console.WriteLine("Звіт порожній.");
            return;
        }

        Console.WriteLine($"{"Читач",-24} {"Видач",6}");
        foreach (var report in reports)
            Console.WriteLine($"{report.ReaderId,-24} {report.LoanCount,6}");
    }

    private static void Print(IReadOnlyList<OverdueLoanReport> reports)
    {
        if (reports.Count == 0)
        {
            Console.WriteLine("Прострочених видач немає.");
            return;
        }

        Console.WriteLine($"{"ID видачі",-24} {"Назва книги",-28} {"Читач",-20} {"Термін повернення",20}");
        foreach (var report in reports)
            Console.WriteLine(
                $"{report.Id,-24} {report.BookTitle,-28} {report.ReaderId,-20} {report.DueDate,20:d}");
    }

    private static void Print(IReadOnlyList<GroupSummary> reports)
    {
        if (reports.Count == 0)
        {
            Console.WriteLine("Даних для групування немає.");
            return;
        }

        Console.WriteLine($"{"Жанр",-24} {"Книг",6} {"Примірників",14}");
        foreach (var report in reports)
            Console.WriteLine($"{report.Key,-24} {report.Count,6} {report.TotalQuantity,14}");
    }

    private static void Print(IReadOnlyList<BookLoanDetails> reports)
    {
        if (reports.Count == 0)
        {
            Console.WriteLine("Даних про видачі немає.");
            return;
        }

        Console.WriteLine($"{"Книга",-28} {"Читач",-20} {"Термін повернення",20}");
        foreach (var report in reports)
            Console.WriteLine(
                $"{report.BookTitle,-28} {report.ReaderId,-20} {report.DueDate,20:d}");
    }
}
