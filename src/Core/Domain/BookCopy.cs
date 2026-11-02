using Core.Dto;

namespace Core.Domain;

public sealed class BookCopy
{
    public string Id { get; }
    public string Isbn { get; }
    public string Title { get; }
    public string Genre { get; }
    public int Quantity { get; }
    public bool IsIssued { get; private set; }

    private BookCopy(string id, string isbn, string title, string genre, int quantity, bool isIssued)
    {
        Id = id;
        Isbn = isbn;
        Title = title;
        Genre = genre;
        Quantity = quantity;
        IsIssued = isIssued;
    }

    public static BookCopy Create(string id, string isbn) =>
        Create(id, isbn, isbn, "Інше", 1);

    public static BookCopy Create(string id, string isbn, string title, string genre, int quantity = 1)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор примірника обов'язковий", nameof(id));

        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN не може бути порожнім", nameof(isbn));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Назва книги не може бути порожньою", nameof(title));
        if (string.IsNullOrWhiteSpace(genre))
            throw new ArgumentException("Жанр книги не може бути порожнім", nameof(genre));
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Кількість примірників не може бути від'ємною.");

        return new BookCopy(id.Trim(), isbn.Trim(), title.Trim(), genre.Trim(), quantity, false);
    }

    public void Issue()
    {
        if (IsIssued)
            throw new InvalidOperationException($"Примірник {Id} вже виданий, повторна видача неможлива"); 
            
        IsIssued = true;
    }

    public void Return()
    {
        if (!IsIssued)
            throw new InvalidOperationException($"Примірник {Id} не видавався, його не можна повернути");
            
        IsIssued = false;
    }

    public BookCopyDto ToDto() => new(Id, Isbn, IsIssued, Title, Genre, Quantity);

    public static BookCopy FromDto(BookCopyDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (string.IsNullOrWhiteSpace(dto.Isbn))
            throw new ArgumentException("ISBN не може бути порожнім", nameof(dto.Isbn));

        string title = string.IsNullOrWhiteSpace(dto.Title) ? dto.Isbn : dto.Title;
        string genre = string.IsNullOrWhiteSpace(dto.Genre) ? "Інше" : dto.Genre;
        var book = Create(dto.Id, dto.Isbn, title, genre, dto.Quantity);
        if (dto.IsIssued)
            book.Issue();

        return book;
    }

    public override string ToString() =>
        $"Примірник {Id}: {Title} [ISBN: {Isbn}, жанр: {Genre}] — {(IsIssued ? "Виданий" : "Доступний")}";
}