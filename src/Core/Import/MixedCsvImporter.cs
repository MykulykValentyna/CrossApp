using System.Globalization;
using Core.Dto;

namespace Core.Import;

public static class MixedCsvImporter
{
    private const char Separator = ';';

    public static ImportResult<IDomainRecord> Load(string path)
    {
        var items = new List<IDomainRecord>();
        var errors = new List<string>();
        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            int number = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

            switch (parts)
            {
                // Товар: P;id;sku;name;unit;quantity
                case ["P", _, "", _, _, _] or ["P", _, _, "", _, _]:
                    errors.Add($"рядок {number}: SKU або назва товару порожні");
                    break;

                case ["P", _, _, _, _, var qty] when !int.TryParse(qty, NumberStyles.Integer, CultureInfo.InvariantCulture, out int q) || q < 0:
                    errors.Add($"рядок {number}: некоректна кількість '{qty}'");
                    break;

                case ["P", var id, var sku, var name, var unit, var qty]:
                    items.Add(new ProductDto(id, sku, name, unit, int.Parse(qty, CultureInfo.InvariantCulture)));
                    break;

                // Склад: W;id;name;location
                case ["W", "", _, _] or ["W", _, "", _]:
                    errors.Add($"рядок {number}: ID або назва складу порожні");
                    break;

                case ["W", var id, var name, var location]:
                    items.Add(new WarehouseDto(id, name, location));
                    break;

                // Невідомий тип
                case [var type, ..]:
                    errors.Add($"рядок {number}: невідомий префікс запису '{type}'");
                    break;

                default:
                    errors.Add($"рядок {number}: некоректний формат рядка");
                    break;
            }
        }

        return new ImportResult<IDomainRecord>(items, errors);
    }
}