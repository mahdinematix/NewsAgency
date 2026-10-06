using FluentAssertions;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.Tests.Unit.AuthorTests;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("john.doe@example.com")]
    [InlineData("user123@test.co")]
    public void Should_Create_When_Email_Format_Is_Valid(string value)
    {
        //act
        var email = new Email(value);

        //assert
        email.Value.Should().Be(value);
    }

    [Fact]
    public void Should_Throw_Exception_When_Value_Failure_To_Observe_Max_Length()
    {
        //arrange
        int maxlength = 256;
        string value = new string('a', maxlength) + "@example.com";

        //act
        Action createEmail = () => new Email(value);

        //assert
        createEmail.Should().ThrowExactly<InvalidValueObjectStateException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Throw_Exception_When_Value_Is_Null_Or_WhiteSpace(string value)
    {
        //act
        Action createEmail = () => new Email(value);

        //assert
        createEmail.Should().ThrowExactly<InvalidValueObjectStateException>();
    }

    [Theory]
    [InlineData("test@.com")]
    [InlineData("test")]
    [InlineData("@example")]
    [InlineData("@example.com")]
    [InlineData("test@example")]
    [InlineData("test @example.com")]
    public void Should_Throw_Exception_When_Email_Format_Is_Invalid(string value)
    {
        //act
        Action createEmail = () => new Email(value);

        //assert
        createEmail.Should().ThrowExactly<InvalidValueObjectStateException>();
    }
}

