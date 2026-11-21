using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.EndPoints.WebApi.Extensions;
using NewsAgency.EndPoints.WebApi.HATEOAS;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Models.Categories;
using NewsAgency.EndPoints.WebApi.Routing;

namespace NewsAgency.EndPoints.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet(Name = ApiRouteNames.Categories.GetAll)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAllCategoriesQuery(),
            cancellationToken);

        return this.ToCategoryCollectionResult(result);
    }

    [HttpGet("{id:long}", Name = ApiRouteNames.Categories.GetById)]
    public async Task<IActionResult> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCategoryByIdQuery(id),
            cancellationToken);

        return this.ToCategoryResult(result);
    }

    [HttpGet("by-title", Name = ApiRouteNames.Categories.GetByTitle)]
    public async Task<IActionResult> GetByTitle(
        [FromQuery] string title,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCategoryByTitleQuery(title),
            cancellationToken);

        return this.ToCategoryResult(result);
    }

    [HttpGet("{id:long}/parent", Name = ApiRouteNames.Categories.GetParentByChildId)]
    public async Task<IActionResult> GetParentByChildId(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCategoryParentByChildIdQuery(id),
            cancellationToken);

        return this.ToCategoryParentResult(result, id);
    }

    [HttpGet("{id:long}/children", Name = ApiRouteNames.Categories.GetChildrenByParentId)]
    public async Task<IActionResult> GetChildrenByParentId(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCategoryChildrenByParentIdQuery(id),
            cancellationToken);

        return this.ToCategoryCollectionResult(result);
    }

    [HttpPost(Name = ApiRouteNames.Categories.Create)]
    public async Task<IActionResult> Create(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ToActionResult(result);
        }

        var createdCategory = result.Data;

        var response = new ApiResponse<long>
        {
            Data = createdCategory.Id,
            IsSuccess = true,
            Message = result.Message,
            ErrorCode = result.ErrorCode,
            Links = CategoryLinks.ForDetails(Url, createdCategory)
        };

        return CreatedAtRoute(
            ApiRouteNames.Categories.GetById,
            new { id = createdCategory.Id },
            response);
    }

    [HttpPut("{id:long}/title", Name = ApiRouteNames.Categories.ChangeTitle)]
    public async Task<IActionResult> ChangeTitle(
        long id,
        [FromBody] ChangeCategoryTitleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeCategoryTitleCommand(
            id,
            request.Title);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToCategoryResult(result);
    }

    [HttpPut("{id:long}/parent", Name = ApiRouteNames.Categories.ChangeParent)]
    public async Task<IActionResult> ChangeParent(
        long id,
        [FromBody] ChangeCategoryParentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeCategoryParentCommand(
            id,
            request.ParentId);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToCategoryResult(result);
    }

    [HttpDelete("{id:long}", Name = ApiRouteNames.Categories.Remove)]
    public async Task<IActionResult> Remove(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new RemoveCategoryCommand(id);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToCategoryResult(result);
    }

    [HttpPost("{id:long}/restore", Name = ApiRouteNames.Categories.Restore)]
    public async Task<IActionResult> Restore(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new RestoreCategoryCommand(id);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToCategoryResult(result);
    }
}



