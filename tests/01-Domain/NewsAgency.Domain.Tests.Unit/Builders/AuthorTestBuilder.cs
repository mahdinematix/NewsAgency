using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.ValueObjects;

namespace NewsAgency.Domain.Tests.Unit.Builders;

public class AuthorTestBuilder
{
    private FirstName _firstName = new(new string('a', 2));
    private LastName _lastName = new(new string('b', 2));
    private Email _email = new("test@example.com");
    private PhoneNumber _phoneNumber = new("00989123456789");

    public Author Build()
    {
        return new Author(_firstName, _lastName, _email, _phoneNumber);
    }

    public AuthorTestBuilder WithFirstName(FirstName firstName)
    {
        _firstName = firstName;
        return this;
    }

    public AuthorTestBuilder WithLastName(LastName lastName)
    {
        _lastName = lastName;
        return this;
    }

    public AuthorTestBuilder WithEmail(Email email)
    {
        _email = email;
        return this;
    }

    public AuthorTestBuilder WithPhoneNumber(PhoneNumber phoneNumber)
    {
        _phoneNumber = phoneNumber;
        return this;
    }
}