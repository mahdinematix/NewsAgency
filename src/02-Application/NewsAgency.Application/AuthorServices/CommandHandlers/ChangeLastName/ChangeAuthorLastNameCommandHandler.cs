using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;
using NewsAgency.Utilities;

namespace NewsAgency.Application.AuthorServices.CommandHandlers.ChangeLastName;

public class ChangeAuthorLastNameCommandHandler : ICommandHandler<ChangeAuthorLastNameCommand, OperationResult<AuthorViewModel>>
{
    private readonly IAuthorCommandRepository _commandRepository;
    private readonly IAuthorQueryRepository _queryRepository;

    public ChangeAuthorLastNameCommandHandler(IAuthorCommandRepository commandRepository, IAuthorQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<AuthorViewModel>> Handle(ChangeAuthorLastNameCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<AuthorViewModel>();
        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }
        try
        {
            var author = await _queryRepository.GetByAsync(request.Id, cancellationToken);

            if (author is null)
            {
                return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
            }
            var newLastName = new LastName(request.LastName);

            if (await _queryRepository.Exists(
                    x => x.LastName == newLastName && x.Id != request.Id,
                    cancellationToken))
            {
                return operation.Failed(
                    OperationErrorCode.Conflict,
                    ApplicationMessages.ValueCannotBeDuplicated);
            }

            author.ChangeLastName(newLastName);
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded(new AuthorViewModel
            {
                Id = author.Id,
                FirstName = author.FirstName.Value,
                LastName = author.LastName.Value,
                PhoneNumber = author.PhoneNumber.Value,
                Email = author.Email.Value,
                CreationDate = author.CreationDate.ToFarsi()
            });
        }
        catch (InvalidValueObjectStateException ex)
        {
            return operation.Failed(OperationErrorCode.Validation, ex.Message);
        }
    }
}