# CrossApp 
Наскрізний проєкт з дисципліни «Крос-платформне програмування»[cite: 1].  
Предметна область: Складський облік товарів[cite: 1].  
Основні моделі: `ProductDto`, `WarehouseDto`[cite: 1].  
Призначення: парсинг, імпорт та облік складських залишків із зовнішніх файлів[cite: 1].
## Структура проєкту
```text
CrossApp/
├── data/
│   ├── sample.csv          # Вхідні дані (10 валідних + 3 тестові помилкові рядки)
│   ├── valid.csv           # Повністю коректний тестовий набір даних
│   ├── mixed.csv           # Різнорідні дані з префіксами типу (P - товари, W - склади)
│   ├── sample.json         # Тестові дані у форматі JSON
│   └── catalog.json        # Згенеровані дані бібліотеки (з'являється при запуску з --file)
├── src/
│   ├── Core/
│   │   ├── Core.csproj
│   │   ├── EnvironmentInfo.cs
│   │   ├── Abstractions/
│   │   │   └── IBookStore.cs           # Контракт для сховища примірників
│   │   ├── Domain/
│   │   │   ├── BookCopy.cs             # Доменна сутність примірника книги
│   │   │   ├── DomainImportResult.cs
│   │   │   ├── Loan.cs                 # Доменна сутність видачі книги
│   │   │   └── Order.cs
│   │   ├── Dto/
│   │   │   ├── BookCopyDto.cs
│   │   │   ├── ImportResult.cs
│   │   │   ├── LoanDto.cs
│   │   │   ├── ProductDto.cs
│   │   │   └── WarehouseDto.cs
│   │   ├── Import/
│   │   │   ├── EntityMapper.cs
│   │   │   ├── MixedCsvImporter.cs
│   │   │   ├── ProductCsvImporter.cs
│   │   │   └── ProductJsonImporter.cs
│   │   ├── Services/
│   │   │   └── LendingService.cs       # Сервісний шар (бізнес-логіка бібліотеки)
│   │   └── Storage/
│   │       ├── FileBookStore.cs        # Файлова реалізація сховища
│   │       └── InMemoryBookStore.cs    # Реалізація сховища в оперативній пам'яті
│   └── Cli/
│       ├── Cli.csproj
│       └── Program.cs                  # Composition Root та точка входу
├── CrossApp.sln
├── README.md
└── .gitignore
```
## Домовленість про каталоги в Core
* `Core/Dto/` — незмінні `record`-типи передачі та зберігання даних (тиждень 3)[cite: 1].
* `Core/Import/` — класи розбору вхідних файлів та валідації (тиждень 3)[cite: 1].
* `Core/Domain/` — сутності з інваріантами та бізнес-поведінкою (тиждень 4)[cite: 1].
* `Core/Storage/` — реалізації файлових сховищ `FileCatalogStore` (тиждень 5)[cite: 1].
## Формат вхідних даних
### CSV-файли (`sample.csv`, `valid.csv`)
* **Кодування:** `UTF-8`[cite: 1].
* **Роздільник полів:** `;` (крапка з комою) для уникнення колізій із комами в назвах[cite: 1].
* **Заголовок:** перший рядок містить назви колонок (`id;sku;name;unit;quantity`), ігнорується парсером за префіксом `id`[cite: 1].
* **Коментарі та пропуски:** рядки, що починаються з `#`, або порожні рядки автоматично відкидаються[cite: 1].
* **Числа:** цілі числа розбираються із явною фіксацією `CultureInfo.InvariantCulture`[cite: 1].
### Різнорідний CSV (`mixed.csv`)
* Перша колонка задає префікс типу: `P` для товару (`ProductDto`), `W` для складу (`WarehouseDto`)[cite: 1].
### JSON-імпорт (`sample.json`)
* Підтримує десеріалізацію списку `List<ProductDto>` за допомогою `System.Text.Json` з ігноруванням регістру ключів[cite: 1].
## Команди
### Збірка рішення
```bash
dotnet build
```
### Запуск CLI
```bash
dotnet run --project src/Cli
dotnet run --project src/Cli -- data/valid.csv
dotnet run --project src/Cli -- data/mixed.csv
dotnet run --project src/Cli -- data/sample.json
dotnet run --project src/Cli -- nonexistent.csv
```
### Публікація
```bash
dotnet publish src/Cli -c Release -r win-x64 --self-contained true
dotnet publish src/Cli -c Release -r win-x64 --self-contained false
```
## Середовище розробки
* **.NET SDK:** 10.0 (сумісний із .NET 8.0)
* **ОС:** Windows 11 Pro x64
## Середовище 
.NET SDK 10.0, Windows 11 Pro x64
## Розміри публікації
| RID       | Режим               | Розмір publish | Потрібен runtime |
|-----------|---------------------|----------------|------------------|
| win-x64   | self-contained      | ~76.7 МБ       | ні               |
| win-x64   | framework-dependent | ~0.2 МБ        | так (.NET 10)    |
| linux-x64 | self-contained      | ~78.8 МБ       | ні               |
### Інваріанти доменної моделі (Лабораторна робота 4)
1. **ISBN не порожній**: Перевіряється у фабричному методі `BookCopy.Create`. Кидає `ArgumentException`.
2. **Не видати вже виданий примірник**: Перевіряється у методі `BookCopy.Issue()`. Кидає `InvalidOperationException`.
3. **Дата повернення не раніше дати видачі**: Перевіряється у методі `Loan.Close()`. Кидає `ArgumentOutOfRangeException`.
4. **Не закрити закриту видачу**: Перевіряється у методі `Loan.Close()`. Кидає `InvalidOperationException`.
## Архітектурні рішення (Лабораторна робота 5)
1. **Dependency Inversion:** `LendingService` залежить виключно від абстракції `IBookStore`, а не від конкретних реалізацій сховищ (`InMemory` чи `File`).
2. **Composition Root:** Єдине місце, де використовуються оператори `new` для створення конкретних сховищ — це точка входу застосунку `Program.cs`. Це дозволяє легко замінити файлове сховище на базу даних у майбутньому без зміни коду сервісу.
3. **Мапінг DTO:** У файловому сховищі на диск серіалізуються `BookCopyDto`, оскільки доменні сутності `BookCopy` мають приватні сетери і не призначені для прямої серіалізації.

## Лабораторна робота 7: звіти бібліотеки

```bash
dotnet run --project src/Cli -- report-top
dotnet run --project src/Cli -- report-low
dotnet run --project src/Cli -- report-groups
dotnet run --project src/Cli -- report-sum
dotnet run --project src/Cli -- report-deferred
dotnet run --project src/Cli -- report-loan-details
dotnet run --project src/Cli -- report-top-genres
dotnet run --project src/Cli -- report-performance
```

За замовчуванням використовуються 18 книжкових записів із різними жанрами й кількістю примірників та 18 записів видач. `--file` перемикає книжки на файлове сховище. Команди `report-readers`, `report-overdue`, `report-genres` і `report-totals` залишені як сумісні псевдоніми відповідно для `report-top`, `report-low`, `report-groups` і `report-sum`.

### Запити та призначення

| Метод | Питання звіту | LINQ-оператори |
|---|---|---|
| `LibraryReportService.TopReaders` | Хто має найбільше видач? | `GroupBy`, `Select`, `OrderByDescending`, `ThenBy`, `Take` |
| `LibraryReportService.OverdueLoans` | Які активні видачі прострочені? | `Where`, `Join`, `OrderBy`, `ThenBy`, `Select` |
| `LibraryReportService.BooksByGenre` | Скільки назв і примірників у кожному жанрі? | `GroupBy`, `Select`, `Sum`, `OrderByDescending` |
| `LibraryReportService.Totals` | Які кількість, сума, середнє і максимум примірників? | `Select`, `ToList`, `Sum`, `Average`, `Max` |
| `AdvancedReportService.GetLoanDetails` | Яка книга відповідає кожному запису видачі? | `Join`, `Select`, `OrderBy`, `ThenBy` |
| `AdvancedReportService.TopReadersCustom` | Топ читачів із власним сортуванням Top-N | `GroupBy`, `Select`, `TopBy` |
| `AdvancedReportService.TopGenresCustom` | Топ жанрів за кількістю примірників | `GroupBy`, `Select`, `Sum`, `TopBy` |

У `BooksByGenre` ключ групи — значення `Genre`, а `group.Count()` рахує кількість записів-книг у жанрі. `group.Sum(book => book.Quantity)` додає кількості примірників. Проєкція повертає `GroupSummary`, а не доменні сутності.

```csharp
books.GroupBy(book => book.Genre, StringComparer.OrdinalIgnoreCase)
     .Select(group => new GroupSummary(
         group.Key,
         group.Count(),
         group.Sum(book => book.Quantity)))
     .OrderByDescending(summary => summary.TotalQuantity)
     .ThenBy(summary => summary.Key, StringComparer.Ordinal)
     .ToList();
```

### Приклад виводу команд

`report-top`:

```text
Читач                     Видач
Reader-A                      5
Reader-B                      4
Reader-C                      3
Reader-D                      2
Reader-E                      2
```

`report-low`:

```text
ID видачі                Назва книги                  Читач                   Термін повернення
L001                     Кобзар                       Reader-A                       21.10.2026
L010                     Intermezzo                   Reader-C                       25.10.2026
L002                     Тигролови                    Reader-A                       28.10.2026
L006                     Кайдашева сім'я              Reader-B                       30.10.2026
```

`report-groups`:

```text
Жанр                       Книг    Примірників
Поезія                        3             34
Історичний роман              3             27
Роман                         3             27
Пригоди                       2             24
Класика                       2             19
Драма                         3             14
Автобіографічна проза         1              5
Новела                        1              3
```

`report-sum`:

```text
Книг: 18, примірників: 153, середня кількість примірників: 8,50, максимум: 16
```

`report-top-genres`:

```text
Жанр                       Книг    Примірників
Поезія                        3             34
Історичний роман              3             27
Роман                         3             27
Пригоди                       2             24
Класика                       2             19
```

`report-deferred`:

```text
Дефіцитних до зміни джерела: 11
Дефіцитних після зміни джерела: 12
У матеріалізованому знімку: 12
```

`report-loan-details` (дати нижче — приклад запуску 02.11.2026; у програмі вони обчислюються від поточної системної дати):

```text
Книга                        Читач                   Термін повернення
Людина                       Reader-D                       24.08.2026
Украдене щастя               Reader-D                       03.09.2026
Земля                        Reader-E                       13.09.2026
Наталка Полтавка              Reader-E                       23.09.2026
Лебеді материнства           Reader-F                       03.10.2026
Пригоди Тома Сойєра          Reader-F                       13.10.2026
Кобзар                       Reader-A                       21.10.2026
Intermezzo                   Reader-C                       25.10.2026
Тигролови                    Reader-A                       28.10.2026
Кайдашева сім'я              Reader-B                       30.10.2026
Чорна рада                   Reader-B                       09.11.2026
Лісова пісня                 Reader-A                       10.11.2026
Майстер корабля              Reader-C                       11.11.2026
Маруся Чурай                 Reader-B                       12.11.2026
Місто                        Reader-A                       14.11.2026
Зачарована Десна             Reader-B                       16.11.2026
Захар Беркут                 Reader-A                       17.11.2026
Енеїда                       Reader-C                       18.11.2026
```

`report-performance` генерує 100 000 записів, порівнює час звіту без проміжного `ToList()` та з ним і перевіряє однаковість результатів. Один запуск у середовищі розробки:

```text
Генерація 100 000 записів...
Час БЕЗ проміжного ToList: 11 мс
Час З проміжним ToList: 7 мс
```

Час залежить від середовища та навантаження; один запуск не є універсальним висновком про продуктивність.

### Відкладене виконання та матеріалізація

`Where` створює запит, але не перебирає джерело; перший `Count()` запускає виконання. Оскільки після нього до локального списку додається книга з кількістю 3, другий `Count()` бачить на один результат більше: 11 проти 12. `ToList()` фіксує результат у знімку, тому очищення вихідного списку не змінює його розмір (12). Запити звітів матеріалізуються в Core через `ToList()`, щоб CLI міг без повторного виконання перебирати готовий `IReadOnlyList`.

### Додаткові завдання

- `Join` реалізовано у звітах прострочених видач і `AdvancedReportService.GetLoanDetails`, результат проєктується у DTO.
- Власний `TopBy` має другий критерій сортування та використаний у двох методах `AdvancedReportService`.
- `BooksByGenreQuerySyntax` реалізує групування у query syntax; методний варіант `BooksByGenre` простіше читати як послідовність операторів, а query syntax компактніше показує конструкцію `group ... into`.
- `report-performance` вимірює два варіанти оброблення 100 000 записів через `Stopwatch`. Проміжний `ToList()` зберігає відфільтровані дані окремо, але додає виділення пам'яті та копіювання; заміри часу одного запуску не є універсальним висновком про продуктивність.