using FluentAssertions;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.Tests.Unit.AuthorTests;

public class FirstNameTests
{
    [Theory]
    [InlineData(2)]
    [InlineData(50)]
    public void Should_Create_When_Value_Length_Is_Valid(int length)
    {
        //arrange
        string value = new string('a', length);

        //act
        var firstName = new FirstName(value);

        //assert
        firstName.Value.Should().Be(value);
    }


    [Theory]
    [InlineData(1)]
    [InlineData(51)]
    public void Should_Throw_Exception_When_Value_Length_Is_Invalid(int length)
    {
        //arrange
        string value = new string('a', length);

        //act
        Action createFirstName= () => new FirstName(value);

        //assert
        createFirstName.Should().ThrowExactly<InvalidValueObjectStateException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Throw_Exception_When_Value_Is_Null_Or_WhiteSpace(string value)
    {
        //act
        Action createFirstName = () => new FirstName(value);

        //assert
        createFirstName.Should().ThrowExactly<InvalidValueObjectStateException>();
    }
}