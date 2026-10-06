using NewsAgency.Domain.AuthorAgg.ValueObjects;

namespace NewsAgency.Domain.AuthorAgg.Entities;

public class Author
{
    public long Id { get; private set; }
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
}

