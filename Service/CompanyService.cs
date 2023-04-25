using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class CompanyService : ICompanyService
{
   private readonly IRepositoryManager _repositoryManager;
   private readonly ILoggerManager _loggerManager;
   private readonly IMapper _mapper;

   public CompanyService(IRepositoryManager repositoryManager,
                         ILoggerManager loggerManager,
                         IMapper mapper)
   {
      _repositoryManager = repositoryManager;
      _loggerManager = loggerManager;
      _mapper = mapper;
   }

   public async Task<IEnumerable<CompanyDto>> GetAllCompanies(bool trackChanges)
   {

      var companies = await _repositoryManager.Company.GetAllCompanies(trackChanges);

      var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

      return companiesDto;

   }

   public async Task<CompanyDto> GetCompany(Guid id, bool trackChanges)
   {
      var company = await GetCompanyAndCheckIfItExists(id, trackChanges);
      var companyDto = _mapper.Map<CompanyDto>(company);
      return companyDto;
   }

   public async Task<CompanyDto> CreateCompany(CompanyForCreationDto company)
   {
      var companyEntity = _mapper.Map<Company>(company);
      _repositoryManager.Company.CreateCompany(companyEntity);
      await _repositoryManager.SaveAsync();
      var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
      return companyToReturn;
   }
   public async Task<IEnumerable<CompanyDto>> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
   {
      if (ids is null)
         throw new IdParametersBadRequestException();
      var companyEntities = await _repositoryManager.Company.GetByIds(ids, trackChanges);
      if (ids.Count() != companyEntities.Count())
         throw new CollectionByIdsBadRequestException();
      var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
      return companiesToReturn;
   }
   public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollection
   (IEnumerable<CompanyForCreationDto> companyCollection)
   {
      if (companyCollection is null)
         throw new CompanyCollectionBadRequest();
      var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
      foreach (var company in companyEntities)
      {
         _repositoryManager.Company.CreateCompany(company);
      }
     await _repositoryManager.SaveAsync();
      var companyCollectionToReturn =
      _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
      var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
      return (companies: companyCollectionToReturn, ids: ids);
   }
   public async Task DeleteCompany(Guid companyId, bool trackChanges)
   {
      var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges); ;
     
      _repositoryManager.Company.DeleteCompany(company);
      await _repositoryManager.SaveAsync();
   }
   public async Task UpdateCompany(Guid companyId, CompanyForUpdateDto companyForUpdate, bool
   trackChanges)
   {
      var companyEntity = await GetCompanyAndCheckIfItExists(companyId, trackChanges);
      _mapper.Map(companyForUpdate, companyEntity);
      await _repositoryManager.SaveAsync();
   }
   private async Task<Company> GetCompanyAndCheckIfItExists(Guid id, bool trackChanges)
   {
      var company = await _repositoryManager.Company.GetCompany(id, trackChanges);
      if (company is null)
         throw new CompanyNotFoundException(id);
      return company;
   }
}
