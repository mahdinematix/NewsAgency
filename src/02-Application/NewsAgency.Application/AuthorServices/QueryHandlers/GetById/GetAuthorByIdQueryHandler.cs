using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Repositories;

namespace NewsAgency.Application.AuthorServices.QueryHandlers.GetById;

public class GetAuthorByIdQueryHandler : IQueryHandler<GetAuthorByIdQuery, OperationResult<AuthorViewModel>>
{
    private readonly IAuthorQueryRepository _queryRepository;

    public GetAuthorByIdQueryHandler(IAuthorQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<AuthorViewModel>> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<AuthorViewModel>();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        var author = await _queryRepository.GetByAsync(request.Id, cancellationToken);

        if (author is null)
        {
            return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
        }

        return operation.Succeeded(new AuthorViewModel
        {
            Id = author.Id,
            Email = author.Email.Value,
            FirstName = author.FirstName.Value,
            LastName = author.LastName.Value,
            PhoneNumber = author.PhoneNumber.Value
        });

    }
}