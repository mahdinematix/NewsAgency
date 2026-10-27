using MediatR;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.AuthorServices.CommandHandlers.ChangeLastName;

public class ChangeAuthorLastNameCommandHandler : IRequestHandler<ChangeAuthorLastNameCommand, OperationResult>
{
    private readonly IAuthorCommandRepository _commandRepository;
    private readonly IAuthorQueryRepository _queryRepository;

    public ChangeAuthorLastNameCommandHandler(IAuthorCommandRepository commandRepository, IAuthorQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult> Handle(ChangeAuthorLastNameCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult();
        if (request is null)
        {
            return operation.Failed(ApplicationMessages.ValueMustNotBeNull);
        }
        try
        {
            var author = await _queryRepository.GetByAsync(request.Id, cancellationToken);

            if (author is null)
            {
                return operation.Failed(ApplicationMessages.RecordNotFound);
            }

            author.ChangeLastName(new LastName(request.LastName));
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded();
        }
        catch (InvalidValueObjectStateException ex)
        {
            return operation.Failed(ex.Message);
        }
    }
}