using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Dynamic;

namespace Service;

internal sealed class EmployeeService : IEmployeeService
{
   private readonly IRepositoryManager _repositoryManager;
   private readonly ILoggerManager _loggerManager;
   private readonly IMapper _mapper;
   private readonly IDataShaper<EmployeeDto> _dataShaper;

   public EmployeeService(IRepositoryManager repositoryManager,
                          ILoggerManager loggerManager,
                          IMapper mapper,
                           IDataShaper<EmployeeDto> dataShaper)
   {
      _repositoryManager = repositoryManager;
      _loggerManager = loggerManager;
      _mapper = mapper;
      _dataShaper = dataShaper;
   }
   public async Task<(IEnumerable<ExpandoObject> employees, MetaData metaData)> GetEmployees(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
   {
      if (!employeeParameters.ValidAgeRange)
         throw new MaxAgeRangeBadRequestException();
      await CheckIfCompanyExists(companyId, trackChanges);
      var employeesWithMetaData = await _repositoryManager.Employee
                  .GetEmployees(companyId, employeeParameters, trackChanges);
      var employeesDto =
      _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);
      var shapedData = _dataShaper.ShapeData(employeesDto,
employeeParameters.Fields);

      return (employees: shapedData, metaData: employeesWithMetaData.MetaData);
   }
   public async Task<EmployeeDto> GetEmployee(Guid companyId, Guid id, bool trackChanges)
   {
      await CheckIfCompanyExists(companyId, trackChanges);
      var employeeDb = await _repositoryManager.Employee.GetEmployee(companyId, id, trackChanges) ?? throw new EmployeeNotFoundException(id);
      var employee = _mapper.Map<EmployeeDto>(employeeDb);
      return employee;
   }
   public async Task<EmployeeDto> CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto
   employeeForCreation, bool trackChanges)
   {
      await CheckIfCompanyExists(companyId, trackChanges);
      var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
      _repositoryManager.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
      await _repositoryManager.SaveAsync();
      var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
      return employeeToReturn;
   }
   public async Task DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges)
   {
      await CheckIfCompanyExists(companyId, trackChanges);
      var employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id,
      trackChanges);
      _repositoryManager.Employee.DeleteEmployee(employeeDb);
      await _repositoryManager.SaveAsync();
   }
   public async Task UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto
   employeeForUpdate,
   bool compTrackChanges, bool empTrackChanges)
   {
      await CheckIfCompanyExists(companyId, compTrackChanges);
      var employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id,
      empTrackChanges);
      _mapper.Map(employeeForUpdate, employeeDb);
      await _repositoryManager.SaveAsync();
   }

   private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
   {
      var company = await _repositoryManager.Company.GetCompany(companyId,
      trackChanges);
      if (company is null)
         throw new CompanyNotFoundException(companyId);
   }
   private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists
   (Guid companyId, Guid id, bool trackChanges)
   {
      var employeeDb = await _repositoryManager.Employee.GetEmployee(companyId, id,
      trackChanges);
      if (employeeDb is null)
         throw new EmployeeNotFoundException(id);
      return employeeDb;
   }
}