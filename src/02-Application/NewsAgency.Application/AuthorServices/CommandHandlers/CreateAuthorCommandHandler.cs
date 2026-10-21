using MediatR;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.AuthorServices.CommandHandlers;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, OperationResult<long>>
{
    private readonly IAuthorCommandRepository _commandRepository;

    public CreateAuthorCommandHandler(IAuthorCommandRepository commandRepository)
    {
        _commandRepository = commandRepository;
    }

    public async Task<OperationResult<long>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<long>();
        try
        {
            if (request is null)
            {
                return operation.Failed(ApplicationMessages.ValueMustNotBeNull);
            }

            var author = new Author(new FirstName(request.FirstName), new LastName(request.LastName),
                new Email(request.Email), new PhoneNumber(request.PhoneNumber));
            await _commandRepository.CreateAsync(author, cancellationToken);
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded(author.Id);
        }
        catch (InvalidValueObjectStateException ex)
        {
            return operation.Failed(ex.Message);
        }
    }
}