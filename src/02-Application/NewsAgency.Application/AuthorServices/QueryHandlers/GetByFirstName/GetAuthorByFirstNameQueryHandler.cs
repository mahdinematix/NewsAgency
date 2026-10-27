using MediatR;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Repositories;

namespace NewsAgency.Application.AuthorServices.QueryHandlers.GetByFirstName;

public class GetAuthorByFirstNameQueryHandler : IRequestHandler<GetAuthorByFirstNameQuery, OperationResult<AuthorViewModel>>
{
    private readonly IAuthorQueryRepository _queryRepository;

    public GetAuthorByFirstNameQueryHandler(IAuthorQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<AuthorViewModel>> Handle(GetAuthorByFirstNameQuery request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<AuthorViewModel>();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        var author = await _queryRepository.GetByFirstNameAsync(request.FirstName, cancellationToken);

        if (author is null)
        {
            return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
        }

        return operation.Succeeded(new AuthorViewModel
        {
           FirstName = author.FirstName.Value,
           Email = author.Email.Value,
           LastName = author.LastName.Value,
           Id = author.Id,
           PhoneNumber = author.PhoneNumber.Value
        });
    }
}
