namespace Shared.DataTransferObjects;
[Serializable]
public record CompanyDto(Guid Id, string Name, string FullAddress);
public record CompanyForCreationDto(string Name, string Address, string Country,
IEnumerable<EmployeeForCreationDto> Employees);
