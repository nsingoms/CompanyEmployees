﻿using CompanyEmployees.Presentation.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies")]
[ApiController]
public class CompaniesController : ControllerBase
{
   private readonly IServiceManager _service;
   public CompaniesController(IServiceManager service) => _service = service;
  
   
   [HttpOptions]
   public IActionResult GetCompaniesOptions()
   {
      Response.Headers.Add("Allow", "GET, OPTIONS, POST");
      return Ok();
   }

   [HttpGet]
   public async Task<IActionResult> GetCompanies()
   {

      var companies = await _service.CompanyService.GetAllCompanies(trackChanges: false);
      return Ok(companies);

   }


   [HttpGet("{id:guid}", Name = "CompanyById")]
   public async Task<IActionResult> GetCompany(Guid id)
   {

      var company = await _service.CompanyService.GetCompany(id, trackChanges: false);
      return Ok(company);
   }


   [HttpGet("collection/({ids})", Name = "CompanyCollection")]
   public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType=typeof(ArrayModelBinder))]IEnumerable <Guid> ids)
   {
      var companies = await _service.CompanyService.GetByIds(ids, trackChanges: false);
      return Ok(companies);
   }

   [HttpPost("collection")]
   public async Task<IActionResult> CreateCompanyCollection([FromBody]
IEnumerable<CompanyForCreationDto> companyCollection)
   {
      var result =
      await _service.CompanyService.CreateCompanyCollection(companyCollection);
      return CreatedAtRoute("CompanyCollection", new { result.ids },
      result.companies);
   }


   [HttpPost]
   [ServiceFilter(typeof(ValidationFilterAttribute))]
   public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
   {
      var createdCompany = await _service.CompanyService.CreateCompany(company);
      return CreatedAtRoute("CompanyById", new { id = createdCompany.Id },
      createdCompany);
   }

   [HttpDelete("{id:guid}")]
   public async Task<IActionResult> DeleteCompany(Guid id)
   {
     await _service.CompanyService.DeleteCompany(id, trackChanges: false);
      return NoContent();
   }


   [HttpPut("{id:guid}")]
   [ServiceFilter(typeof(ValidationFilterAttribute))]

   public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
   {
      await _service.CompanyService.UpdateCompany(id, company, trackChanges: true);
      return NoContent();
   }


}
