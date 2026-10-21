using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.Common.Repositories;

namespace NewsAgency.Domain.AuthorAgg.Repositories;

public interface IAuthorQueryRepository : IGenericQueryRepository<Author, long>
{
    Task<Author?> GetByFirstNameAsync(string firstName, CancellationToken cancellationToken);
    Task<Author?> GetByLastNameAsync(string lastName, CancellationToken cancellationToken);
    Task<Author?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Author?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
}