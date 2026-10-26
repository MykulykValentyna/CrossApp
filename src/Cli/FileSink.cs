using System;
using System.IO;
using Core.Abstractions;

namespace Cli;

public sealed class FileSink(string filePath) : INotificationSink
{
    public void Notify(string message) => File.AppendAllText(filePath, message + Environment.NewLine);
}