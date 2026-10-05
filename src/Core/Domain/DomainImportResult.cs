namespace Core.Domain;

public class DomainImportResult<TEntity>
{
    public IReadOnlyList<TEntity> Entities { get; }
    public IReadOnlyList<string> Errors { get; }

    public DomainImportResult(IReadOnlyList<TEntity> entities, IReadOnlyList<string> errors)
    {
        Entities = entities;
        Errors = errors;
    }
}