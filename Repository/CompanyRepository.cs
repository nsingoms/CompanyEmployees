using Contracts;
using Entities.Models;


namespace Repository;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
   public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
   {
   }

   public IEnumerable<Company> GetAllCompanies(bool trackChanges)=>
               FindAll(trackChanges).OrderBy(c=>c.Name).ToList();

   public Company GetCompany(Guid companyId, bool trackChanges) =>
    FindbyCondition(c => c.Id.Equals(companyId), trackChanges)
    .SingleOrDefault();

   public void CreateCompany(Company company) => Create(company);
   public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges) =>
    FindbyCondition(x => ids.Contains(x.Id), trackChanges)
    .ToList();
}
