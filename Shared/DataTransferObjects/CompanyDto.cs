namespace Shared.DataTransferObjects;
[Serializable]
public record CompanyDto(Guid Id, string Name, string FullAddress);
public record CompanyForCreationDto(string Name, string Address, string Country,
IEnumerable<EmployeeForCreationDto> Employees);
public record EmployeeForCreationDto(string Name, int Age, string Position);
public record EmployeeDto(Guid Id, string Name, int Age, string Position);