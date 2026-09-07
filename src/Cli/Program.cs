using System.Runtime.InteropServices;
using System.Text.Json;

var info = new
{
    OSDescription = RuntimeInformation.OSDescription,
    OSEnvironment = Environment.OSVersion.ToString(),
    ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
    NetVersion = Environment.Version.ToString(),
    Runtime = RuntimeInformation.FrameworkDescription,
    BaseDirectory = AppContext.BaseDirectory,
    CurrentDirectory = Environment.CurrentDirectory
};

if (args.Contains("--json"))
{
    string jsonString = JsonSerializer.Serialize(info);
    Console.WriteLine(jsonString);
}
else
{
    Console.WriteLine("CrossApp – практикум з крос-платформного програмування"); 
    Console.WriteLine("Студент: Микулик Валентина, група ФЕІ-22с"); 
    Console.WriteLine(new string('-', 52)); 
    Console.WriteLine($"ОС (OSDescription)   : {info.OSDescription}"); 
    Console.WriteLine($"ОС (Environment)     : {info.OSEnvironment}"); 
    Console.WriteLine($"Архітектура процесу  : {info.ProcessArchitecture}"); 
    Console.WriteLine($"Версія .NET (CLR)    : {info.NetVersion}"); 
    Console.WriteLine($"Runtime              : {info.Runtime}"); 
    Console.WriteLine($"Каталог застосунку   : {info.BaseDirectory}"); 
    Console.WriteLine($"Поточний каталог     : {info.CurrentDirectory}"); 
    Console.WriteLine(new string('-', 52)); 
    Console.WriteLine("Предметна область: Бібліотека (Book, BookCopy, Reader, Loan)");
}