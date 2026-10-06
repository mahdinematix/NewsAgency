using FluentAssertions;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.Tests.Unit.AuthorTests;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("00989123456789")]
    [InlineData("989123456789")]
    [InlineData("09123456789")]
    [InlineData("9123456789")]
    public void Should_Create_When_Value_Is_Valid(string value)
    {
        //act
        var phoneNumber = new PhoneNumber(value);

        //assert
        phoneNumber.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Throw_Exception_When_Value_Is_Null_Or_WhiteSpace(string value)
    {
        //act
        Action createPhoneNumber = () => new PhoneNumber(value);

        //assert
        createPhoneNumber.Should().ThrowExactly<InvalidValueObjectStateException>();
    }

    [Theory]
    [InlineData(9)]
    [InlineData(15)]
    public void Should_Throw_Exception_When_Value_Length_Is_Invalid(int length)
    {
        //arrange
        string value = new string('1',length);

        //act
        Action createPhoneNumber = () => new PhoneNumber(value);

        //assert
        createPhoneNumber.Should().ThrowExactly<InvalidValueObjectStateException>();
    }

    [Theory]
    [InlineData("0912345678a")]
    [InlineData("091234567-")]
    [InlineData("091234567 ")]
    public void Should_Throw_Exception_When_Value_Contains_Non_Digit(string value)
    {
        //act
        Action createPhoneNumber = () => new PhoneNumber(value);

        //assert
        createPhoneNumber.Should().ThrowExactly<InvalidValueObjectStateException>();
    }

}

