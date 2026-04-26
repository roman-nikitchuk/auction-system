using Api.Dtos;
using Api.Modules.Errors;
using Application.Bids.Commands;
using Application.Common.Interfaces.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/auctions/{auctionId:int}/bids")]
[ApiController]
public class BidController(ISender sender, IBidQueries bidQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BidDto>>> GetByAuction(
        [FromRoute] int auctionId,
        CancellationToken cancellationToken)
    {
        var bids = await bidQueries.GetByAuctionIdAsync(auctionId, cancellationToken);
        return Ok(bids.Select(BidDto.FromDomainModel).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<BidDto>> Create(
        [FromRoute] int auctionId,
        [FromBody] CreateBidDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBidCommand
        {
            AuctionId = auctionId,
            UserId = request.UserId,
            Amount = request.Amount
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<BidDto>>(
            b => CreatedAtAction(nameof(GetByAuction),
                new { auctionId = b.AuctionId },
                BidDto.FromDomainModel(b)),
            e => e.ToObjectResult());
    }
}