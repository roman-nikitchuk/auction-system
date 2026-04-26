using Domain;
using Domain.Entities;

namespace Api.Dtos;

public record AuctionDto(
    int Id,
    string Title,
    string Description,
    int OwnerId,
    string OwnerName,
    int CategoryId,
    string CategoryName,
    AuctionStatus Status,
    decimal StartingPrice,
    decimal CurrentBid,
    DateTime StartDate,
    DateTime EndDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    public static AuctionDto FromDomainModel(Auction model)
        => new(
            model.Id,
            model.Title,
            model.Description,
            model.OwnerId,
            model.Owner?.UserName ?? string.Empty,
            model.CategoryId,
            model.Category?.Name ?? string.Empty,
            model.Status,
            model.StartingPrice,
            model.CurrentBid,
            model.StartDate,
            model.EndDate,
            model.CreatedAt,
            model.UpdatedAt);
}

public record CreateAuctionDto(
    string Title,
    string Description,
    int CategoryId,
    decimal StartingPrice,
    DateTime StartDate,
    DateTime EndDate,
    int OwnerId);

public record UpdateAuctionDto(
    string Title,
    string Description,
    int CategoryId,
    DateTime StartDate,
    DateTime EndDate);