using System;
using System.Diagnostics;
using System.Linq;
using Core.Domain;

namespace Cli;

public static class PerformanceTest
{
    public static void Run()
    {
        Console.WriteLine("Генерація 100 000 записів...");
        var largeData = Enumerable.Range(1, 100_000)
            .Select(i =>
            {
                var book = BookCopy.Create(i.ToString(), $"ISBN-{i % 50:D3}");
                if (i % 10 == 0)
                    book.Issue();
                return book;
            })
            .ToList();

        var sw = new Stopwatch();

        sw.Start();
        var lazyReport = largeData
            .Where(book => !book.IsIssued)
            .GroupBy(book => book.Isbn)
            .Select(group => new { Isbn = group.Key, Copies = group.Count() })
            .ToList();
        sw.Stop();
        var timeLazy = sw.ElapsedMilliseconds;

        sw.Restart();
        var greedyReport = largeData
            .Where(book => !book.IsIssued)
            .ToList()
            .GroupBy(book => book.Isbn)
            .Select(group => new { Isbn = group.Key, Copies = group.Count() })
            .ToList();
        sw.Stop();
        var timeGreedy = sw.ElapsedMilliseconds;

        if (!lazyReport.SequenceEqual(greedyReport))
            throw new InvalidOperationException("Результати LINQ-звітів відрізняються.");

        Console.WriteLine($"Час БЕЗ проміжного ToList: {timeLazy} мс");
        Console.WriteLine($"Час З проміжним ToList: {timeGreedy} мс");
    }
}