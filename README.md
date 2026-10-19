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