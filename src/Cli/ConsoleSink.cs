using System;
using Core.Abstractions;

namespace Cli;

public sealed class ConsoleSink : INotificationSink
{
    public void Notify(string message) => Console.WriteLine(message);
}