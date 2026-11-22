using MediatR;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;
using NewsAgency.Utilities;

namespace NewsAgency.Application.CategoryServices.CommandHandlers.Create;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, OperationResult<CategoryViewModel>>
{
    private readonly ICategoryCommandRepository _commandRepository;
    private readonly ICategoryQueryRepository _queryRepository;

    public CreateCategoryCommandHandler(ICategoryCommandRepository commandRepository, ICategoryQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<CategoryViewModel>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<CategoryViewModel>();
        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        try
        {
            var title = new CategoryTitle(request.Title);
            if (await _queryRepository.Exists(x=>x.Title == title, cancellationToken))
            {
                return operation.Failed(OperationErrorCode.Conflict, ApplicationMessages.ValueCannotBeDuplicated);
            }
            var category = new Category(new CategoryTitle(request.Title), request.ParentId);
            var parentIsRemoved = false;

            if (request.ParentId.HasValue)
            {
                var parent = await _queryRepository.GetByAsync(
                    request.ParentId.Value,
                    cancellationToken);

                if (parent is null)
                {
                    return operation.Failed(
                        OperationErrorCode.NotFound,
                        ApplicationMessages.RecordNotFound);
                }

                parentIsRemoved = parent.IsRemoved;
            }
            var hasActiveChildren = await _queryRepository.HasActiveChildrenAsync(
                category.Id,
                cancellationToken);
            await _commandRepository.CreateAsync(category, cancellationToken);
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded(new CategoryViewModel
            {
                Id = category.Id,
                Title = category.Title.Value,
                IsRemoved = category.IsRemoved,
                ParentId = category.ParentId,
                CreationDate = category.CreationDate.ToFarsi(),
                Actions = new CategoryActionsViewModel
                {
                    CanRemove = category.CanRemove(hasActiveChildren),
                    CanRestore = category.CanRestore(parentIsRemoved)
                }
            });
        }
        catch (InvalidValueObjectStateException ex)
        {
            return operation.Failed(OperationErrorCode.Validation, ex.Message);
        }
    }
}