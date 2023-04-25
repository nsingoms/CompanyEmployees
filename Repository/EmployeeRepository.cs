using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extentions;
using Shared.RequestFeatures;

namespace Repository;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
   public EmployeeRepository(RepositoryContext repositoryContext)
      : base(repositoryContext)
   {
   }
   public async Task<PagedList<Employee>> GetEmployees(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
   {
      var employees = await FindbyCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
            .Search(employeeParameters.SearchTerm)
            .Sort(employeeParameters.OrderBy)
            .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
            .Take(employeeParameters.PageSize)
            .ToListAsync();
      var count = await FindbyCondition(e => e.CompanyId.Equals(companyId), trackChanges
      ).CountAsync();
      return new PagedList<Employee>(employees, count, employeeParameters.PageNumber, employeeParameters.PageSize);
   }
   public async Task<Employee> GetEmployee(Guid companyId, Guid id, bool trackChanges) =>
      await FindbyCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id),
      trackChanges).SingleOrDefaultAsync();
   public void CreateEmployeeForCompany(Guid companyId, Employee employee)
   {
      employee.CompanyId = companyId;
      Create(employee);
   }
   public void DeleteEmployee(Employee employee) => Delete(employee);
}