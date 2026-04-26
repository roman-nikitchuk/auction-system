using Api.Dtos;
using Api.Modules.Errors;
using Application.Auctions.Commands;
using Application.Common.Interfaces.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/auctions")]
[ApiController]
public class AuctionController(ISender sender, IAuctionQueries auctionQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuctionDto>>> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] AuctionStatus? status,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Auction> auctions;

        if (categoryId.HasValue)
            auctions = await auctionQueries.GetByCategoryAsync(categoryId.Value, cancellationToken);
        else if (status.HasValue)
            auctions = await auctionQueries.GetByStatusAsync(status.Value, cancellationToken);
        else
            auctions = await auctionQueries.GetAllAsync(cancellationToken);

        return Ok(auctions.Select(AuctionDto.FromDomainModel).ToList());
    }

    [HttpGet("{auctionId:int}")]
    public async Task<ActionResult<AuctionDto>> GetById(
        [FromRoute] int auctionId,
        CancellationToken cancellationToken)
    {
        var result = await auctionQueries.GetByIdAsync(auctionId, cancellationToken);

        return result.Match<ActionResult<AuctionDto>>(
            a => Ok(AuctionDto.FromDomainModel(a)),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> Create(
        [FromBody] CreateAuctionDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAuctionCommand
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            StartingPrice = request.StartingPrice,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            OwnerId = request.OwnerId
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<AuctionDto>>(
            a => CreatedAtAction(nameof(GetById),
                new { auctionId = a.Id },
                AuctionDto.FromDomainModel(a)),
            e => e.ToObjectResult());
    }

    [HttpPut("{auctionId:int}")]
    public async Task<ActionResult<AuctionDto>> Update(
        [FromRoute] int auctionId,
        [FromBody] UpdateAuctionDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAuctionCommand
        {
            Id = auctionId,
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<AuctionDto>>(
            a => Ok(AuctionDto.FromDomainModel(a)),
            e => e.ToObjectResult());
    }

    [HttpDelete("{auctionId:int}")]
    public async Task<ActionResult> Delete(
        [FromRoute] int auctionId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteAuctionCommand { Id = auctionId };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult>(
            _ => NoContent(),
            e => e.ToObjectResult());
    }
}