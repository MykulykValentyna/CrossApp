namespace Core.Abstractions;
using Core.Domain;

public interface IBookStore 
{ 
    IReadOnlyList<BookCopy> List(); 
    BookCopy? GetById(string id); 
    void Add(BookCopy item); 
    void Update(BookCopy item); 
    bool Remove(string id); 
    IReadOnlyList<BookCopy> Search(Func<BookCopy, bool> predicate);
}