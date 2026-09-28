# CrossApp 
Наскрізний проєкт з крос-платформного програмування. 
Предметна область: Бібліотека. Сутності: Book, BookCopy, Reader, Loan. 
Призначення: облік видач примірників книг читачам. 
## Структура solution
CrossApp/ 
CrossApp.sln 
README.md 
.gitignore 
src/ 
  Core/ 
    Core.csproj 
    EnvironmentInfo.cs 
    EnvironmentReport.cs        
  Cli/ 
    Cli.csproj                  
    Program.cs
## Домовленість про каталоги в Core на весь семестр
Core/Dto/     – record-типи формату даних (тиждень 3): ProductDto / BookDto / OrderDto 
Core/Domain/  – сутності з поведінкою та інваріантами (тиждень 4) 
Core/Storage/ – реалізації сховищ (тиждень 5) 
## Команди
Збірка:
```
dotnet build
```
Запуск:
```
dotnet run --project src/Cli
```
Публікація:
```
dotnet publish src/Cli -c Release -r win-x64 --self-contained true
dotnet publish src/Cli -c Release -r win-x64 --self-contained false
```
## Середовище 
.NET SDK 10.0, Windows 11 Pro x64
## Розміри публікації
| RID       | Режим               | Розмір publish | Потрібен runtime |
|-----------|---------------------|----------------|------------------|
| win-x64   | self-contained      | ~76.7 МБ       | ні               |
| win-x64   | framework-dependent | ~0.2 МБ        | так (.NET 10)    |
| linux-x64 | self-contained      | ~78.8 МБ       | ні               |