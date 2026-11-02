namespace Core.Dto;

public record BookCopyDto(
    string Id,
    string Isbn,
    bool IsIssued,
    string Title = "",
    string Genre = "",
    int Quantity = 1);