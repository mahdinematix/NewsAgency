using FluentAssertions;
using NewsAgency.Domain.Tests.Unit.TestDoubles;

namespace NewsAgency.Domain.Tests.Unit.Common;

public class BaseValueObjectTests
{
    [Fact]
    public void Should_Be_Equal_When_Equality_Components_Are_The_Same()
    {
        //arrange
        string value = "Ali";
        TestValueObject t1 = new(value);
        TestValueObject t2 = new(value);

        //assert
        t1.Should().Be(t2);

    }

    [Fact]
    public void Should_Be_Not_Equal_When_Values_Are_Different()
    {
        //arrange
        TestValueObject t1 = new("value1");
        TestValueObject t2 = new("value2");

        //assert
        t1.Should().NotBe(t2);

    }

    [Fact]
    public void Should_Return_True_When_Equality_Operator_Compares_Equal_Values()
    {
        //arrange
        string value = "Ali";
        TestValueObject t1 = new(value);
        TestValueObject t2 = new(value);

        //act
        bool checkEquality = t1 == t2;

        //assert
        checkEquality.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_False_When_Equality_Operator_Compares_NotEqual_Values()
    {
        //arrange
        TestValueObject t1 = new("value1");
        TestValueObject t2 = new("value2");

        //act
        bool checkEquality = t1 == t2;

        //assert
        checkEquality.Should().BeFalse();
    }

    [Fact]
    public void Should_Return_False_When_Inequality_Operator_Compares_Equal_Values()
    {
        //arrange
        string value = "Ali";
        TestValueObject t1 = new(value);
        TestValueObject t2 = new(value);

        //act
        bool checkInequality = t1 != t2;

        //assert
        checkInequality.Should().BeFalse();
    }

    [Fact]
    public void Should_Return_True_When_Inequality_Operator_Compares_NotEqual_Values()
    {
        //arrange
        TestValueObject t1 = new("value1");
        TestValueObject t2 = new("value2");

        //act
        bool checkInequality = t1 != t2;

        //assert
        checkInequality.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_True_When_Equality_Operator_Compares_Null_Values()
    {
        //arrange
        TestValueObject? t1 = null;
        TestValueObject? t2 = null;

        //act
        bool checkEquality = t1 == t2;

        //assert
        checkEquality.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_False_When_Inequality_Operator_Compares_Null_Values()
    {
        //arrange
        TestValueObject? t1 = null;
        TestValueObject? t2 = null;

        //act
        bool checkInequality = t1 != t2;

        //assert
        checkInequality.Should().BeFalse();
    }

    [Fact]
    public void Should_Return_Same_HashCode_When_Values_Are_Equal()
    {
        // arrange
        string value = "Ali";
        TestValueObject t1 = new(value);
        TestValueObject t2 = new(value);

        // act
        int hashCode1 = t1.GetHashCode();
        int hashCode2 = t2.GetHashCode();

        // assert
        hashCode1.Should().Be(hashCode2);
    }
}

