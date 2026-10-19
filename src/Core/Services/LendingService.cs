namespace Core.Services;

using System;
using System.Collections.Generic;
using Core.Abstractions;
using Core.Domain;

public sealed class LendingService(IBookStore store) 
{ 
    private readonly IBookStore _store = store ?? throw new ArgumentNullException(nameof(store)); 

    public BookCopy AddBook(string isbn) 
    { 
        string newId = Guid.NewGuid().ToString("N")[..8];
        
        var bookCopy = BookCopy.Create(newId, isbn); 
        
        _store.Add(bookCopy);          
        return bookCopy; 
    } 

    public void IssueCopy(string id) 
    { 
        var bookCopy = _store.GetById(id) 
            ?? throw new InvalidOperationException($"Немає примірника з id={id}."); 
            
        bookCopy.Issue();   
        
        _store.Update(bookCopy); 
    } 

    public void ReturnCopy(string id) 
    { 
        var bookCopy = _store.GetById(id) 
            ?? throw new InvalidOperationException($"Немає примірника з id={id}."); 
            
        bookCopy.Return();   
        
        _store.Update(bookCopy); 
    } 

    public IReadOnlyList<BookCopy> All() => _store.List(); 
    
    public BookCopy? Find(string id) => _store.GetById(id); 

    public IReadOnlyList<BookCopy> FindAvailable() 
    {
        return _store.Search(book => !book.IsIssued);
    }
}