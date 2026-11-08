using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Application.Contracts.Article.Queries;
using NewsAgency.EndPoints.WebApi.Extensions;
using NewsAgency.EndPoints.WebApi.HATEOAS;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Models.Articles;
using NewsAgency.EndPoints.WebApi.Routing;

namespace NewsAgency.EndPoints.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArticlesController : ControllerBase
{
    private readonly ISender _sender;

    public ArticlesController(ISender sender)
    {
        _sender = sender;
    }


    [HttpGet(Name = ApiRouteNames.Articles.GetAll)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAllArticlesQuery(),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ToActionResult(result);
        }

        var articles = result.Data
            .Select(article => new ApiResource<ArticleViewModel>
            {
                Data = article,
                Links = ArticleLinks.ForDetails(Url, article)
            })
            .ToList();

        var response = new ApiResponse<List<ApiResource<ArticleViewModel>>>
        {
            Data = articles,
            IsSuccess = result.IsSuccess,
            Message = result.Message,
            ErrorCode = result.ErrorCode,
            Links = ArticleLinks.ForCollection(Url)
        };

        return Ok(response);
    }


    [HttpGet("{id:long}", Name = ApiRouteNames.Articles.GetById)]
    public async Task<IActionResult> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetArticleByIdQuery(id),
            cancellationToken);

        return this.ToArticleResult(result);
    }

    [HttpGet("by-title", Name = ApiRouteNames.Articles.GetByTitle)]
    public async Task<IActionResult> GetByTitle(
        [FromQuery] string title,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetArticleByTitleQuery(title),
            cancellationToken);

        return this.ToArticleResult(result);
    }

    [HttpPost(Name = ApiRouteNames.Articles.Create)]
    public async Task<IActionResult> Create(
        CreateArticleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ToActionResult(result);
        }

        var createdArticle = result.Data;

        var response = new ApiResponse<long>
        {
            Data = createdArticle.Id,
            IsSuccess = true,
            Message = result.Message,
            ErrorCode = result.ErrorCode,
            Links = ArticleLinks.ForDetails(Url, createdArticle)
        };

        return CreatedAtRoute(
            ApiRouteNames.Articles.GetById,
            new { id = createdArticle.Id },
            response);
    }

    [HttpPut("{id:long}", Name = ApiRouteNames.Articles.Edit)]
    public async Task<IActionResult> Edit(
        [FromRoute] long id,
        [FromBody] EditArticleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new EditArticleCommand(
            id,
            request.Title,
            request.Content,
            request.AuthorId,
            request.CategoryId);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToArticleResult(result);
    }

    [HttpPost("{id:long}/publish", Name = ApiRouteNames.Articles.Publish)]
    public async Task<IActionResult> Publish(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new PublishArticleCommand(id);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToArticleResult(result);
    }

    [HttpPost("{id:long}/reject", Name = ApiRouteNames.Articles.Reject)]
    public async Task<IActionResult> Reject(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new RejectArticleCommand(id);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToArticleResult(result);
    }

    [HttpPost("{id:long}/archive", Name = ApiRouteNames.Articles.Archive)]
    public async Task<IActionResult> Archive(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new ArchiveArticleCommand(id);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToArticleResult(result);
    }

    [HttpPost("{id:long}/move-to-draft", Name = ApiRouteNames.Articles.MoveToDraft)]
    public async Task<IActionResult> MoveToDraft(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new MoveToDraftArticleCommand(id);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToArticleResult(result);
    }

}

