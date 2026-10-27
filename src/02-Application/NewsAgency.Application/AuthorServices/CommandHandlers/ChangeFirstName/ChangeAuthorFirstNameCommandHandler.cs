using MediatR;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.AuthorServices.CommandHandlers.ChangeFirstName;

public class ChangeAuthorFirstNameCommandHandler : IRequestHandler<ChangeAuthorFirstNameCommand, OperationResult>
{
    private readonly IAuthorCommandRepository _commandRepository;
    private readonly IAuthorQueryRepository _queryRepository;

    public ChangeAuthorFirstNameCommandHandler(IAuthorCommandRepository commandRepository, IAuthorQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult> Handle(ChangeAuthorFirstNameCommand request, CancellationToken cancellationToken)
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

            author.ChangeFirstName(new FirstName(request.FirstName));
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded();
        }
        catch (InvalidValueObjectStateException ex)
        {
            return operation.Failed(ex.Message);
        }
    }
}