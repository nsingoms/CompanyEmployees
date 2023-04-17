using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class EmployeeService : IEmployeeService
{
   private readonly IRepositoryManager _repositoryManager;
   private readonly ILoggerManager _loggerManager;
   private readonly IMapper _mapper;

   public EmployeeService(IRepositoryManager repositoryManager,
                          ILoggerManager loggerManager,
                          IMapper mapper)
   {
      _repositoryManager = repositoryManager;
      _loggerManager = loggerManager;
      _mapper = mapper;
   }
   public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
   {
      var company = _repositoryManager.Company.GetCompany(companyId, trackChanges);
      if (company is null)
         throw new CompanyNotFoundException(companyId);
      var employeesFromDb = _repositoryManager.Employee.GetEmployees(companyId,
      trackChanges);
      var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
      return employeesDto;
   }
   public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
   {
      var company = _repositoryManager.Company.GetCompany(companyId, trackChanges);
      if (company is null)
         throw new CompanyNotFoundException(companyId);
      var employeeDb = _repositoryManager.Employee.GetEmployee(companyId, id, trackChanges);
      if (employeeDb is null)
         throw new EmployeeNotFoundException(id);
      var employee = _mapper.Map<EmployeeDto>(employeeDb);
      return employee;
   }
   public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto
   employeeForCreation, bool trackChanges)
   {
      var company = _repositoryManager.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
      var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
      _repositoryManager.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
      _repositoryManager.Save();
      var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
      return employeeToReturn;
   }
}