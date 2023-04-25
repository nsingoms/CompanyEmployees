using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts;

public interface IEmployeeRepository
{
   Task<PagedList<Employee>> GetEmployees(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges);
   Task<Employee> GetEmployee(Guid companyId, Guid id, bool trackChanges);
   void CreateEmployeeForCompany(Guid companyId, Employee employee);
   void DeleteEmployee(Employee employee);
  
}