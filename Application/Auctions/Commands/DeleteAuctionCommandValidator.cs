using FluentValidation;

namespace Application.Auctions.Commands;

public class DeleteAuctionCommandValidator : AbstractValidator<DeleteAuctionCommand>
{
    public DeleteAuctionCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}