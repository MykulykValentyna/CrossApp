namespace Core.Storage;

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Abstractions;
using Core.Domain;

public sealed class CachingBookStore(IBookStore innerStore) : IBookStore
{
    private readonly IBookStore _inner = innerStore ?? throw new ArgumentNullException(nameof(innerStore));
    private Dictionary<string, BookCopy>? _cache;

    private void EnsureCache()
    {
        if (_cache != null) return;
        _cache = _inner.List().ToDictionary(b => b.Id, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<BookCopy> List()
    {
        EnsureCache();
        return _cache!.Values.ToList();
    }

    public BookCopy? GetById(string id)
    {
        EnsureCache();
        return _cache!.GetValueOrDefault(id);
    }

    public void Add(BookCopy item)
    {
        _inner.Add(item);
        EnsureCache();
        _cache![item.Id] = item;
    }

    public void Update(BookCopy item)
    {
        _inner.Update(item);
        EnsureCache();
        _cache![item.Id] = item;
    }

    public bool Remove(string id)
    {
        bool removed = _inner.Remove(id);
        if (removed && _cache != null)
        {
            _cache.Remove(id);
        }
        return removed;
    }
    
    public IReadOnlyList<BookCopy> Search(Func<BookCopy, bool> predicate) 
    {
        EnsureCache();
        return _cache!.Values.Where(predicate).ToList();
    }
}