using NewsAgency.Domain.CategoryAgg.Entities;

namespace NewsAgency.Infra.Data.EFCore.CommandRepositories;

public class CategoryCommandRepository : GenericCommandRepository<Category, long>
{
    public CategoryCommandRepository(NewsAgencyContext context) : base(context)
    {
    }

}

