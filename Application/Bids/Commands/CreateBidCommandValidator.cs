using FluentValidation;

namespace Application.Bids.Commands;

public class CreateBidCommandValidator : AbstractValidator<CreateBidCommand>
{
    public CreateBidCommandValidator()
    {
        RuleFor(x => x.AuctionId).GreaterThan(0);
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}