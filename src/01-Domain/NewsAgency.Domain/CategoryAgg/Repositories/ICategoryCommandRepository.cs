using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.Common.Repositories;

namespace NewsAgency.Domain.CategoryAgg.Repositories;

public interface ICategoryCommandRepository : IGenericCommandRepository<Category, long>
{
}