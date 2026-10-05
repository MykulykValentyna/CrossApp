using Core.Domain;
using Core.Dto;

namespace Core.Import;

public static class EntityMapper
{
    public static DomainImportResult<BookCopy> MapBookCopies(IEnumerable<BookCopyDto> dtos)
    {
        var entities = new List<BookCopy>();
        var errors = new List<string>();

        foreach (var dto in dtos)
        {
            try
            {
                // Відновлення сутності, тут відпрацьовують всі інваріанти методу FromDto
                var entity = BookCopy.FromDto(dto);
                entities.Add(entity);
            }
            catch (Exception ex)
            {
                // Якщо інваріант порушено, додаємо повідомлення в список помилок
                errors.Add($"Помилка імпорту DTO з ID {dto.Id}: {ex.Message}");
            }
        }

        return new DomainImportResult<BookCopy>(entities, errors);
    }
}