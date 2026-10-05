using Core.Dto;

namespace Core.Domain;

public sealed class Loan
{
    public string Id { get; }
    public string CopyId { get; }
    public string ReaderId { get; }
    public DateTime IssuedOn { get; }
    public DateTime? ReturnedOn { get; private set; }

    public bool IsClosed => ReturnedOn.HasValue;

    private Loan(string id, string copyId, string readerId, DateTime issuedOn, DateTime? returnedOn)
    {
        Id = id;
        CopyId = copyId;
        ReaderId = readerId;
        IssuedOn = issuedOn;
        ReturnedOn = returnedOn;
    }

    public static Loan Open(BookCopy copy, string readerId, DateTime issuedOn, int activeLoansCount)
    {
        if (copy == null) throw new ArgumentNullException(nameof(copy));
        if (string.IsNullOrWhiteSpace(readerId)) throw new ArgumentException("ID читача обов'язковий", nameof(readerId));

        if (activeLoansCount >= 5)
            throw new InvalidOperationException($"Читач {readerId} вже має {activeLoansCount} відкритих видач. Ліміт: 5.");

        copy.Issue(); 

        return new Loan(Guid.NewGuid().ToString(), copy.Id, readerId, issuedOn, null);
    }

    public void Close(DateTime returnedOn, BookCopy copy)
    {
        if (IsClosed)
            throw new InvalidOperationException($"Видача {Id} вже закрита. Повторне закриття неможливе."); 

        if (returnedOn < IssuedOn)
            throw new ArgumentOutOfRangeException(nameof(returnedOn), returnedOn, 
                $"Дата повернення ({returnedOn:d}) не може бути раніше дати видачі ({IssuedOn:d})"); 

        ReturnedOn = returnedOn;
        copy.Return();
    }

    public LoanDto ToDto() => new(Id, CopyId, ReaderId, IssuedOn, ReturnedOn);

    public static Loan FromDto(LoanDto dto) => 
        new Loan(dto.Id, dto.CopyId, dto.ReaderId, dto.IssuedOn, dto.ReturnedOn);
}