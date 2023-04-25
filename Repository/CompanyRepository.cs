using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
   public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
   {
   }

   public async Task<IEnumerable<Company>> GetAllCompanies(bool trackChanges)=>
             await  FindAll(trackChanges).OrderBy(c=>c.Name).ToListAsync();

   public async Task<Company> GetCompany(Guid companyId, bool trackChanges) =>
    await FindbyCondition(c => c.Id.Equals(companyId), trackChanges)
    .SingleOrDefaultAsync() ?? throw new CompanyNotFoundException(companyId);

   public void CreateCompany(Company company) => Create(company);
   public async Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> ids, bool trackChanges) =>
       await FindbyCondition(x => ids.Contains(x.Id), trackChanges)
       .ToListAsync();
   public void DeleteCompany(Company company) => Delete(company);
}
