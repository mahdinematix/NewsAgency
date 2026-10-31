using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.AuthorServices.CommandHandlers.ChangePhoneNumber;

public class ChangeAuthorPhoneNumberCommandHandler : ICommandHandler<ChangeAuthorPhoneNumberCommand, OperationResult>
{
    private readonly IAuthorCommandRepository _commandRepository;
    private readonly IAuthorQueryRepository _queryRepository;

    public ChangeAuthorPhoneNumberCommandHandler(IAuthorCommandRepository commandRepository, IAuthorQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult> Handle(ChangeAuthorPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult();
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

            var newPhoneNumber = new PhoneNumber(request.PhoneNumber);

            if (await _queryRepository.Exists(
                    x => x.PhoneNumber.Value == newPhoneNumber.Value && x.Id != request.Id,
                    cancellationToken))
            {
                return operation.Failed(
                    OperationErrorCode.Conflict,
                    ApplicationMessages.ValueCannotBeDuplicated);
            }

            author.ChangePhoneNumber(newPhoneNumber);
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded();
        }
        catch (InvalidValueObjectStateException ex)
        {
            return operation.Failed(OperationErrorCode.Validation, ex.Message);
        }
    }
}