using Microsoft.EntityFrameworkCore;
using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.QueryRepositories;

public class AuthorQueryRepository: GenericQueryRepository<Author, long> , IAuthorQueryRepository
{
    public AuthorQueryRepository(NewsAgencyContext context) : base(context)
    {
    }

    public async Task<Author?> GetByFirstNameAsync(string firstName, CancellationToken cancellationToken)
    {
        var authorFirstName = FirstName.FromString(firstName);

        return await Context.Authors
            .FirstOrDefaultAsync(x => x.FirstName == authorFirstName, cancellationToken);
    }

    public async Task<Author?> GetByLastNameAsync(string lastName, CancellationToken cancellationToken)
    {
        var authorLastName = LastName.FromString(lastName);

        return await Context.Authors
            .FirstOrDefaultAsync(x => x.LastName == authorLastName, cancellationToken);
    }

    public async Task<Author?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var authorEmail = Email.FromString(email);

        return await Context.Authors
            .FirstOrDefaultAsync(x => x.Email == authorEmail, cancellationToken);
    }

    public async Task<Author?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        var authorPhoneNumber = PhoneNumber.FromString(phoneNumber);

        return await Context.Authors
            .FirstOrDefaultAsync(x => x.PhoneNumber == authorPhoneNumber, cancellationToken);
    }
}
