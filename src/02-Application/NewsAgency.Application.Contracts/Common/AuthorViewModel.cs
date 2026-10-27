using NewsAgency.Domain.AuthorAgg.ValueObjects;

namespace NewsAgency.Application.Contracts.Common;

public class AuthorViewModel
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
