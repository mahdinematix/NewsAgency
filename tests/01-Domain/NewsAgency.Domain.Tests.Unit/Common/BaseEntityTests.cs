using FluentAssertions;
using NewsAgency.Domain.Tests.Unit.TestDoubles;

namespace NewsAgency.Domain.Tests.Unit.Common;

public class BaseEntityTests
{
    [Fact]
    public void Should_Be_Equal_When_Entities_Have_The_Same_Id()
    {
        //arrange
        TestEntity e1 = new(1);
        TestEntity e2 = new(1);

        //assert
        e1.Should().Be(e2);
    }

    [Fact]
    public void Should_Not_Be_Equal_When_Entities_Have_Different_Ids()
    {
        // arrange
        TestEntity e1 = new(1);
        TestEntity e2 = new(2);

        // assert
        e1.Should().NotBe(e2);
    }

    [Fact]
    public void Should_Not_Be_Equal_When_Entities_Have_Different_Types()
    {
        // arrange
        TestEntity e1 = new(1);
        AnotherTestEntity e2 = new(1);

        // assert
        e1.Should().NotBe(e2);
    }

    [Fact]
    public void Should_Not_Be_Equal_When_One_Entity_Is_Null()
    {
        // arrange
        TestEntity e1 = new(1);
        TestEntity? e2 = null;

        // assert
        e1.Should().NotBe(e2);
    }

    [Fact]
    public void Should_Return_True_When_Equality_Operator_Compares_Null_Entities()
    {
        // arrange
        TestEntity? e1 = null;
        TestEntity? e2 = null;

        // act
        bool result = e1 == e2;

        // assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_False_When_Inequality_Operator_Compares_Null_Entities()
    {
        // arrange
        TestEntity? e1 = null;
        TestEntity? e2 = null;

        // act
        bool result = e1 != e2;

        // assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Should_Return_The_Same_HashCode_When_Entities_Have_The_Same_Id()
    {
        // arrange
        TestEntity e1 = new(1);
        TestEntity e2 = new(1);

        // act
        int hashCode1 = e1.GetHashCode();
        int hashCode2 = e2.GetHashCode();

        // assert
        hashCode1.Should().Be(hashCode2);
    }

}

