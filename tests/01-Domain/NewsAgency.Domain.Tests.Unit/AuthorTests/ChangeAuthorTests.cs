using FluentAssertions;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Factories;

namespace NewsAgency.Domain.Tests.Unit.AuthorTests
{
    public class ChangeAuthorTests
    {
        [Fact]
        public void Should_Change_FirstName_When_Data_Is_Valid()
        {
            //arrange
            var author = AuthorFactory.Create();
            FirstName newFirstName = new(new string('c', 3));


            //act
            author.ChangeFirstName(newFirstName);

            //assert
            author.FirstName.Should().Be(newFirstName);
        }
        [Fact]
        public void Should_Change_LastName_When_Data_Is_Valid()
        {
            //arrange
            var author = AuthorFactory.Create();
            LastName newLastName = new(new string('c', 4));

            //act
            author.ChangeLastName(newLastName);

            //assert
            author.LastName.Should().Be(newLastName);
        }

        [Fact]
        public void Should_Change_Email_When_Data_Is_Valid()
        {
            //arrange
            var author = AuthorFactory.Create();
            Email newEmail = new("changeTest@example.com");

            //act
            author.ChangeEmail(newEmail);

            //assert
            author.Email.Should().Be(newEmail);
        }

        [Fact]
        public void Should_Change_PhoneNumber_When_Data_Is_Valid()
        {
            //arrange
            var author = AuthorFactory.Create();
            PhoneNumber newPhoneNumber = new("00989123456780");

            //act
            author.ChangePhoneNumber(newPhoneNumber);

            //assert
            author.PhoneNumber.Should().Be(newPhoneNumber);
        }

    }
}
