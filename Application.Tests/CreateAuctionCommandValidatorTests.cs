using Application.Auctions.Commands;
using FluentValidation.TestHelper;
using Xunit;

namespace Application.Tests.Auctions.Commands;

public class CreateAuctionCommandValidatorTests
{
    private readonly CreateAuctionCommandValidator _validator = new();

    private static CreateAuctionCommand ValidCommand(
        string title = "Auction",
        string description = "Description",
        int categoryId = 1,
        decimal startingPrice = 100,
        int ownerId = 1,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow;
        var end = endDate ?? start.AddDays(1);

        return new CreateAuctionCommand
        {
            Title = title,
            Description = description,
            CategoryId = categoryId,
            StartingPrice = startingPrice,
            OwnerId = ownerId,
            StartDate = start,
            EndDate = end
        };
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var command = ValidCommand(title: "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_CategoryId_Is_Invalid()
    {
        var command = ValidCommand(categoryId: 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void Should_Have_Error_When_EndDate_Is_Before_StartDate()
    {
        var start = DateTime.UtcNow;

        var command = ValidCommand(
            startDate: start,
            endDate: start.AddHours(-1)
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = ValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}