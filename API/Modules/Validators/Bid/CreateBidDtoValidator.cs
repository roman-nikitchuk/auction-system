using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Bid;

public class CreateBidDtoValidator : AbstractValidator<CreateBidDto>
{
    public CreateBidDtoValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);

        RuleFor(x => x.Amount)
            .GreaterThan(0);
    }
}