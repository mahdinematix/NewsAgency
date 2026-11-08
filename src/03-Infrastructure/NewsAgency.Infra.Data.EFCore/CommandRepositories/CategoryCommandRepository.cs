using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.Repositories;

namespace NewsAgency.Infra.Data.EFCore.CommandRepositories;

public class CategoryCommandRepository : GenericCommandRepository<Category, long>, ICategoryCommandRepository
{
    public CategoryCommandRepository(NewsAgencyContext context) : base(context)
    {
    }

}

