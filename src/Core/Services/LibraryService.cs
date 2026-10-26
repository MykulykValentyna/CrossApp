using Core.Domain; 
using Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;

namespace Core.Services;

public sealed class LibraryService(IBookStore store, IEnumerable<INotificationSink> sinks)
{
    private readonly IBookStore _store = store ?? throw new ArgumentNullException(nameof(store));
    private readonly IEnumerable<INotificationSink> _sinks = sinks ?? [];
    
    public event EventHandler<LibraryChangedEventArgs>? Changed;

    private void OnChanged(string copyId, LibraryChangeKind kind, string? readerId = null, DateTimeOffset? dueDate = null)
    {
        Changed?.Invoke(this, new LibraryChangedEventArgs(copyId, kind, readerId, dueDate));

        string message = $"[Sink] Зміна стану: {kind} для {copyId}";
        foreach (var sink in _sinks)
        {
            sink.Notify(message);
        }
    }

    public BookCopy AddBook(string id, string isbn)
    {
        var copy = BookCopy.Create(id, isbn);
        _store.Add(copy);
        OnChanged(copy.Id, LibraryChangeKind.BookAdded);
        return copy;
    }

    public void IssueBook(string copyId, string readerId, DateTimeOffset dueDate)
    {
        var copy = _store.GetById(copyId) ?? throw new InvalidOperationException($"Немає запису з id={copyId}.");
        
        copy.Issue();
        _store.Update(copy);
        
        OnChanged(copy.Id, LibraryChangeKind.CopyIssued, readerId, dueDate);
    }

    public IReadOnlyList<BookCopy> FindAll(Func<BookCopy, bool> predicate) =>
        _store.List().Where(predicate).ToList();
}