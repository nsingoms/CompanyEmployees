using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace CompanyEmployees;

public class MappingProfile : Profile
{
   public MappingProfile()
   {
      CreateMap<Company, CompanyDto>()
      .ForCtorParam("FullAddress",
      opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));
      CreateMap<CompanyForCreationDto, Company>();
      CreateMap<EmployeeForCreationDto, Employee>();
      CreateMap<EmployeeForUpdateDto, Employee>();
      CreateMap<Employee, EmployeeDto>(); CreateMap<CompanyForUpdateDto, Company>();
   }
}
