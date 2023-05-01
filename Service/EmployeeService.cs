using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service;

internal sealed class EmployeeService : IEmployeeService
{
   private readonly IRepositoryManager _repositoryManager;
   private readonly ILoggerManager _loggerManager;
   private readonly IMapper _mapper;
   private readonly IEmployeeLinks _employeeLinks;


   public EmployeeService(IRepositoryManager repositoryManager,
                          ILoggerManager loggerManager,
                          IMapper mapper,
                            IEmployeeLinks employeeLinks)
   {
      _repositoryManager = repositoryManager;
      _loggerManager = loggerManager;
      _mapper = mapper;
      _employeeLinks = employeeLinks;
   }
   public async Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployees(Guid companyId, LinkParameters linkParameters, bool trackChanges)
   {
      if (!linkParameters.EmployeeParameters.ValidAgeRange)
         throw new MaxAgeRangeBadRequestException();

      await CheckIfCompanyExists(companyId, trackChanges);

      var employeesWithMetaData = await _repositoryManager.Employee
                  .GetEmployees(companyId, linkParameters.EmployeeParameters, trackChanges);
     
      var employeesDto =
      _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);

      var links = _employeeLinks.TryGenerateLinks(employeesDto,
                  linkParameters.EmployeeParameters.Fields,
                  companyId, linkParameters.Context);

      return (linkResponse: links, metaData: employeesWithMetaData.MetaData);
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