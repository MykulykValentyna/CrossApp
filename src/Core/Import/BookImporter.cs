using System;
using System.Collections.Generic;
using Core.Domain;

namespace Core.Import;

public sealed class BookImporter
{
    public List<BookCopy> Import(string[] csvLines, Action<string>? onSkippedLine = null)
    {
        var result = new List<BookCopy>();
        foreach (var line in csvLines)
        {
            var parts = line.Split(',');
            if (parts.Length != 2)
            {
                onSkippedLine?.Invoke($"Пропущено рядок (неправильний формат): {line}");
                continue;
            }

            try
            {
                result.Add(BookCopy.Create(parts[0], parts[1]));
            }
            catch (ArgumentException)
            {
                onSkippedLine?.Invoke($"Пропущено рядок (невалідна сутність): {line}");
            }
        }
        return result;
    }
}