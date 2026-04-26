using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Auction;

public class UpdateAuctionDtoValidator : AbstractValidator<UpdateAuctionDto>
{
    public UpdateAuctionDtoValidator()
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

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("EndDate must be later than StartDate");
    }
}