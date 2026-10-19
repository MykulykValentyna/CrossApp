namespace Core.Storage;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

public sealed class FileBookStore(string path) : IBookStore 
{ 
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true }; 
    private readonly Dictionary<string, BookCopy> _cache = new(StringComparer.OrdinalIgnoreCase); 
    private readonly string _path = Path.GetFullPath(path); 
    private bool _loaded; 

    private void EnsureLoaded() 
    { 
        if (_loaded) return; 
        if (File.Exists(_path)) 
        { 
            var dtos = JsonSerializer.Deserialize<List<BookCopyDto>>(File.ReadAllText(_path)) ?? []; 
            foreach (var dto in dtos) 
            { 
                var b = BookCopy.FromDto(dto); 
                _cache[b.Id] = b; 
            } 
        } 
        _loaded = true; 
    } 

    private void Flush() 
    { 
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!); 
        File.WriteAllText(_path, 
            JsonSerializer.Serialize(_cache.Values.Select(b => b.ToDto()).ToList(), Options)); 
    } 

    public IReadOnlyList<BookCopy> List() 
    { 
        EnsureLoaded(); 
        return _cache.Values.ToList(); 
    } 

    public BookCopy? GetById(string id) 
    { 
        EnsureLoaded(); 
        return _cache.GetValueOrDefault(id); 
    } 

    public void Add(BookCopy item) 
    { 
        ArgumentNullException.ThrowIfNull(item); 
        EnsureLoaded(); 
        
        if (_cache.ContainsKey(item.Id)) 
            throw new InvalidOperationException($"Примірник з id={item.Id} уже існує."); 
            
        _cache.Add(item.Id, item); 
        Flush(); 
    } 

    public void Update(BookCopy item) 
    { 
        EnsureLoaded(); 
        _cache[item.Id] = item; 
        Flush(); 
    } 

    public bool Remove(string id) 
    { 
        EnsureLoaded(); 
        if (!_cache.Remove(id)) return false; 
        
        Flush(); 
        return true; 
    } 

    public IReadOnlyList<BookCopy> Search(Func<BookCopy, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        EnsureLoaded(); 
        return _cache.Values.Where(predicate).ToList();
    }
}