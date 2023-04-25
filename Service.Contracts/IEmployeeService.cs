using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Collections.Generic;
using System.Dynamic;

namespace Service.Contracts;

public interface IEmployeeService
{
   Task<(IEnumerable<ExpandoObject> employees, MetaData metaData)> GetEmployees(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges);
   Task<EmployeeDto> GetEmployee(Guid companyId, Guid id, bool trackChanges);
   Task<EmployeeDto> CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto
   employeeForCreation, bool trackChanges);
   Task DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges);
   Task UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool
     empTrackChanges);
}
