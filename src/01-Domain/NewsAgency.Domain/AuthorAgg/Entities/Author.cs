using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Common;

namespace NewsAgency.Domain.AuthorAgg.Entities;

public class Author : BaseEntity
{
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }

    public Author(long id, FirstName firstName, LastName lastName, Email email, PhoneNumber phoneNumber)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public void ChangeFirstName(FirstName firstName)
    {
        FirstName = firstName;
    }

    public void ChangeLastName(LastName lastName)
    {
        LastName = lastName;
    }

    public void ChangeEmail(Email email)
    {
        Email = email;
    }

    public void ChangePhoneNumber(PhoneNumber phoneNumber)
    {
        PhoneNumber = phoneNumber;
    }

}
