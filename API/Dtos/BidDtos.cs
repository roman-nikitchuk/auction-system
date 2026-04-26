using Domain.Entities;

namespace Api.Dtos;

public record BidDto(
    int Id,
    int AuctionId,
    int UserId,
    string UserName,
    decimal Amount,
    DateTime CreatedAt)
{
    public static BidDto FromDomainModel(Bid model)
        => new(
            model.Id,
            model.AuctionId,
            model.UserId,
            model.User?.UserName ?? string.Empty,
            model.Amount,
            model.CreatedAt);
}

public record CreateBidDto(
    int UserId,
    decimal Amount);