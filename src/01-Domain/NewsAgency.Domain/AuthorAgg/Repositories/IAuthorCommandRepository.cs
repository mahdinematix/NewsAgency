using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.Common.Repositories;

namespace NewsAgency.Domain.AuthorAgg.Repositories;

public interface IAuthorCommandRepository : IGenericCommandRepository<Author, long>
{
}