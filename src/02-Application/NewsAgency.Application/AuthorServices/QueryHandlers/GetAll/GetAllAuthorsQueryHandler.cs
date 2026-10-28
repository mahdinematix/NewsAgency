using MediatR;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Repositories;

namespace NewsAgency.Application.AuthorServices.QueryHandlers.GetAll;

public class GetAllAuthorsQueryHandler : IRequestHandler<GetAllAuthorsQuery, OperationResult<List<AuthorViewModel>>>
{
    private readonly IAuthorQueryRepository _queryRepository;

    public GetAllAuthorsQueryHandler(IAuthorQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<List<AuthorViewModel>>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<List<AuthorViewModel>>();
        var authors = await _queryRepository.GetAllAsync(cancellationToken);

        if (authors is null)
        {
            return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
        }

        return operation.Succeeded(authors.Select(x => new AuthorViewModel
        {
            Id = x.Id,
            Email = x.Email.Value,
            FirstName = x.FirstName.Value,
            LastName = x.LastName.Value,
            PhoneNumber = x.PhoneNumber.Value
        }).ToList());

    }
}