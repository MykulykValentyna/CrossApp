using System.Globalization;
using Core.Dto;

namespace Core.Import;

public static class ProductCsvImporter
{
    // Роздільник — крапка з комою: запобігає конфліктам із комами в назвах чи описах
    private const char Separator = ';';

    public static ImportResult<ProductDto> Load(string path)
    {
        var items = new List<ProductDto>();
        var errors = new List<string>();

        // Явно зчитуємо всі рядки у кодуванні UTF-8
        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            int number = i + 1;
            string line = lines[i];

            // Пропускаємо порожні рядки та рядки-коментарі
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            // Пропускаємо заголовок (якщо присутній)
            if (number == 1 && line.StartsWith("id", StringComparison.OrdinalIgnoreCase))
                continue;

            switch (ParseLine(line))
            {
                case ParseOk ok:
                    items.Add(ok.Value);
                    break;
                case ParseFailed failed:
                    errors.Add($"рядок {number}: {failed.Reason}");
                    break;
            }
        }

        return new ImportResult<ProductDto>(items, errors);
    }

    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

        return parts switch
        {
            // 1. Патерн властивостей: перевірка на недостатню кількість колонок
            { Length: < 5 } => new ParseFailed($"очікую 5 колонок, отримав {parts.Length}"),

            // 2. Патерн списку + логічний or: перевірка на порожні обов'язкові поля (SKU або Name)
            [_, "", _, _, _] or [_, _, "", _, _] => new ParseFailed("SKU або назва порожні"),

            // 3. Патерн списку + додаткова умова when з InvariantCulture
            [_, _, _, _, var qty] when !int.TryParse(qty, NumberStyles.Integer, CultureInfo.InvariantCulture, out int q) || q < 0
                => new ParseFailed($"кількість '{qty}' не є невід'ємним числом"),

            // 4. Позиційний розбір валідного списку з 5 колонок
            [var id, var sku, var name, var unit, var qty]
                => new ParseOk(new ProductDto(id, sku, name, unit, int.Parse(qty, CultureInfo.InvariantCulture))),

            // 5. Запасний патерн (wildcard pattern): надлишок колонок
            _ => new ParseFailed($"занадто багато колонок: {parts.Length}")
        };
    }

    // Закрита ієрархія результатів розбору одного рядка
    private abstract record ParseOutcome;
    private sealed record ParseOk(ProductDto Value) : ParseOutcome;
    private sealed record ParseFailed(string Reason) : ParseOutcome;
}