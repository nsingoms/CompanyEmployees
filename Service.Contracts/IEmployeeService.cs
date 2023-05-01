using Entities.LinkModels;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service.Contracts;

public interface IEmployeeService
{
   Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployees(Guid companyId,
                                 LinkParameters linkParameters, bool trackChanges);

   Task<EmployeeDto> GetEmployee(Guid companyId, Guid id, bool trackChanges);
   Task<EmployeeDto> CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto
   employeeForCreation, bool trackChanges);
   Task DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges);
   Task UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool
     empTrackChanges);
}
