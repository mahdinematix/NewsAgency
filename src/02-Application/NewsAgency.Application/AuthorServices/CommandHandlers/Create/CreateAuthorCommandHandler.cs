using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.AuthorServices.CommandHandlers.Create;

public class CreateAuthorCommandHandler : ICommandHandler<CreateAuthorCommand, OperationResult<long>>
{
    private readonly IAuthorCommandRepository _commandRepository;
    private readonly IAuthorQueryRepository _queryRepository;

    public CreateAuthorCommandHandler(IAuthorCommandRepository commandRepository, IAuthorQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<long>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<long>();
        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        try
        {
            if (await _queryRepository.Exists(x => x.Email.Value == request.Email || x.PhoneNumber.Value == request.PhoneNumber, cancellationToken))
            {
                return operation.Failed(OperationErrorCode.Conflict, ApplicationMessages.ValueCannotBeDuplicated);
            }
            var author = new Author(new FirstName(request.FirstName), new LastName(request.LastName),
                new Email(request.Email), new PhoneNumber(request.PhoneNumber));
            await _commandRepository.CreateAsync(author, cancellationToken);
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded(author.Id);
        }
        catch (InvalidValueObjectStateException ex)
        {
            return operation.Failed(OperationErrorCode.Validation, ex.Message);
        }
    }
}