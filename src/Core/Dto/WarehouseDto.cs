namespace Core.Dto;

public interface IDomainRecord;

public record WarehouseDto(
    string Id,
    string Name,
    string Location
) : IDomainRecord;