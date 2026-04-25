using FluentValidation;

namespace Application.Auctions.Commands;

public class CreateAuctionCommandValidator : AbstractValidator<CreateAuctionCommand>
{
    public CreateAuctionCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0);

        RuleFor(x => x.StartingPrice)
            .GreaterThan(0);

        RuleFor(x => x.OwnerId)
            .GreaterThan(0);

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("EndDate must be later than StartDate");
    }
}