using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;
using NewsAgency.Utilities;

namespace NewsAgency.Application.AuthorServices.CommandHandlers.Create;

public class CreateAuthorCommandHandler : ICommandHandler<CreateAuthorCommand, OperationResult<AuthorViewModel>>
{
    private readonly IAuthorCommandRepository _commandRepository;
    private readonly IAuthorQueryRepository _queryRepository;

    public CreateAuthorCommandHandler(IAuthorCommandRepository commandRepository, IAuthorQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<AuthorViewModel>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<AuthorViewModel>();
        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        try
        {
            var email = new Email(request.Email);
            var phoneNumber = new PhoneNumber(request.PhoneNumber);
            if (await _queryRepository.Exists(x => x.Email == email || x.PhoneNumber == phoneNumber, cancellationToken))
            {
                return operation.Failed(OperationErrorCode.Conflict, ApplicationMessages.ValueCannotBeDuplicated);
            }
            var author = new Author(new FirstName(request.FirstName), new LastName(request.LastName),
                new Email(request.Email), new PhoneNumber(request.PhoneNumber));
            await _commandRepository.CreateAsync(author, cancellationToken);
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