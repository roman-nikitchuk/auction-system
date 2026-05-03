using Application.Bids.Commands;
using FluentValidation.TestHelper;
using Xunit;

namespace Application.Tests.Bids.Commands;

public class CreateBidCommandValidatorTests
{
    private readonly CreateBidCommandValidator _validator = new();

    private static CreateBidCommand ValidCommand(
        int auctionId = 1,
        int userId = 1,
        decimal amount = 100) => new()
    {
        AuctionId = auctionId,
        UserId = userId,
        Amount = amount
    };

    [Fact]
    public void Should_Have_Error_When_AuctionId_Is_Invalid()
    {
        var command = ValidCommand(auctionId: 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AuctionId);
    }

    [Fact]
    public void Should_Have_Error_When_Amount_Is_Invalid()
    {
        var command = ValidCommand(amount: 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = ValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}