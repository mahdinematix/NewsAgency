using FluentAssertions;
using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.ValueObjects;

namespace NewsAgency.Domain.Tests.Unit.AuthorTests;

public class CreateAuthorTests
{
    [Fact]
    public void Should_Create_When_Data_Is_Valid()
    {
        //arrange
        long id = 1;
        FirstName firstName = new(new string('a', 2));
        LastName lastName = new(new string('b', 2));
        Email email = new("test@example.com");
        PhoneNumber phoneNumber = new("00989123456789");

        //act
        var author = new Author(id, firstName, lastName, email, phoneNumber);

        //assert
        author.Id.Should().Be(id);
        author.FirstName.Should().Be(firstName);
        author.LastName.Should().Be(lastName);
        author.Email.Should().Be(email);
        author.PhoneNumber.Should().Be(phoneNumber);
    }
}

