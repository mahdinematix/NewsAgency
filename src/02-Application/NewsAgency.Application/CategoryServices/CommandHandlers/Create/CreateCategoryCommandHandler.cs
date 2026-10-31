using MediatR;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.CategoryServices.CommandHandlers.Create;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, OperationResult<long>>
{
    private readonly ICategoryCommandRepository _commandRepository;
    private readonly ICategoryQueryRepository _queryRepository;

    public CreateCategoryCommandHandler(ICategoryCommandRepository commandRepository, ICategoryQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<long>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<long>();
        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        try
        {
            if (await _queryRepository.Exists(x=>x.Title.Value == request.Title, cancellationToken))
            {
                return operation.Failed(OperationErrorCode.Conflict, ApplicationMessages.ValueCannotBeDuplicated);
            }
            var category = new Category(new CategoryTitle(request.Title), request.ParentId);
            await _commandRepository.CreateAsync(category, cancellationToken);
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded(category.Id);
        }
        catch (InvalidValueObjectStateException ex)
        {
            return operation.Failed(OperationErrorCode.Validation, ex.Message);
        }
    }
}