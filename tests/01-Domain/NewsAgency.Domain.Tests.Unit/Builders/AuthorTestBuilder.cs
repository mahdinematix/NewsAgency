using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.AuthorAgg.Entities;

namespace NewsAgency.Domain.Tests.Unit.Builders;

public class AuthorTestBuilder
{
    private long _id = 1;
    private FirstName _firstName = new(new string('a', 2));
    private LastName _lastName = new(new string('b', 2));
    private Email _email = new("test@example.com");
    private PhoneNumber _phoneNumber = new("00989123456789");

    public Author Build()
    {
        return new Author(_id, _firstName, _lastName, _email, _phoneNumber);
    }

}
