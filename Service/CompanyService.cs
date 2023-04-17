﻿using AutoMapper;
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

   public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges)
   {

      var companies = _repositoryManager.Company.GetAllCompanies(trackChanges);

      var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

      return companiesDto;

   }

   public CompanyDto GetCompany(Guid id, bool trackChanges)
   {
      var company = _repositoryManager.Company.GetCompany(id, trackChanges) ?? throw new CompanyNotFoundException(id);
      var companyDto = _mapper.Map<CompanyDto>(company);
      return companyDto;
   }

   public CompanyDto CreateCompany(CompanyForCreationDto company)
   {
      var companyEntity = _mapper.Map<Company>(company);
      _repositoryManager.Company.CreateCompany(companyEntity);
      _repositoryManager.Save();
      var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
      return companyToReturn;
   }
   public IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
   {
      if (ids is null)
         throw new IdParametersBadRequestException();
      var companyEntities = _repository.Company.GetByIds(ids, trackChanges);
      if (ids.Count() != companyEntities.Count())
         throw new CollectionByIdsBadRequestException();
      var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
      return companiesToReturn;
   }
}
