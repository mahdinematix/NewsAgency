using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.EndPoints.WebApi.Extensions;
using NewsAgency.EndPoints.WebApi.HATEOAS;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Models.Authors;
using NewsAgency.EndPoints.WebApi.Routing;

namespace NewsAgency.EndPoints.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorsController : ControllerBase
{
    private readonly ISender _sender;

    public AuthorsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet(Name = ApiRouteNames.Authors.GetAll)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAllAuthorsQuery(),
            cancellationToken);

        return this.ToAuthorCollectionResult(result);
    }

    [HttpGet("{id:long}", Name = ApiRouteNames.Authors.GetById)]
    public async Task<IActionResult> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAuthorByIdQuery(id),
            cancellationToken);

        return this.ToAuthorResult(result);
    }

    [HttpGet("by-first-name", Name = ApiRouteNames.Authors.GetByFirstName)]
    public async Task<IActionResult> GetByFirstName(
        [FromQuery] string firstName,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAuthorByFirstNameQuery(firstName),
            cancellationToken);

        return this.ToAuthorResult(result);
    }

    [HttpGet("by-last-name", Name = ApiRouteNames.Authors.GetByLastName)]
    public async Task<IActionResult> GetByLastName(
        [FromQuery] string lastName,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAuthorByLastNameQuery(lastName),
            cancellationToken);

        return this.ToAuthorResult(result);
    }

    [HttpGet("by-email", Name = ApiRouteNames.Authors.GetByEmail)]
    public async Task<IActionResult> GetByEmail(
        [FromQuery] string email,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAuthorByEmailQuery(email),
            cancellationToken);

        return this.ToAuthorResult(result);
    }

    [HttpGet("by-phone-number", Name = ApiRouteNames.Authors.GetByPhoneNumber)]
    public async Task<IActionResult> GetByPhoneNumber(
        [FromQuery] string phoneNumber,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAuthorByPhoneNumberQuery(phoneNumber),
            cancellationToken);

        return this.ToAuthorResult(result);
    }


    [HttpPost(Name = ApiRouteNames.Authors.Create)]
    public async Task<IActionResult> Create(
        CreateAuthorCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return this.ToActionResult(result);
        }

        var createdAuthorId = result.Data.Id;

        var response = new ApiResponse<long>
        {
            Data = createdAuthorId,
            IsSuccess = true,
            Message = result.Message,
            ErrorCode = result.ErrorCode,
            Links = AuthorLinks.ForDetails(Url, createdAuthorId)
        };

        return CreatedAtRoute(
            ApiRouteNames.Authors.GetById,
            new { id = createdAuthorId },
            response);
    }

    [HttpPut("{id:long}/first-name", Name = ApiRouteNames.Authors.ChangeFirstName)]
    public async Task<IActionResult> ChangeFirstName(
        long id,
        [FromBody] ChangeAuthorFirstNameRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeAuthorFirstNameCommand(
            id,
            request.FirstName);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToAuthorResult(result);
    }

    [HttpPut("{id:long}/last-name", Name = ApiRouteNames.Authors.ChangeLastName)]
    public async Task<IActionResult> ChangeLastName(
        long id,
        [FromBody] ChangeAuthorLastNameRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeAuthorLastNameCommand(
            id,
            request.LastName);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToAuthorResult(result);
    }

    [HttpPut("{id:long}/email", Name = ApiRouteNames.Authors.ChangeEmail)]
    public async Task<IActionResult> ChangeEmail(
        long id,
        [FromBody] ChangeAuthorEmailRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeAuthorEmailCommand(
            id,
            request.Email);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToAuthorResult(result);
    }

    [HttpPut("{id:long}/phone-number", Name = ApiRouteNames.Authors.ChangePhoneNumber)]
    public async Task<IActionResult> ChangePhoneNumber(
        long id,
        [FromBody] ChangeAuthorPhoneNumberRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeAuthorPhoneNumberCommand(
            id,
            request.PhoneNumber);

        var result = await _sender.Send(command, cancellationToken);

        return this.ToAuthorResult(result);
    }
}