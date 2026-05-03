using Application.Users.Commands;
using FluentValidation.TestHelper;
using Xunit;

namespace Application.Tests.Users.Commands;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator = new();

    private static CreateUserCommand ValidCommand(
        string userName = "TestUser",
        string email = "test@test.com",
        string password = "password123") => new()
    {
        UserName = userName,
        Email = email,
        Password = password
    };

    [Fact]
    public void Should_Have_Error_When_UserName_Is_Empty()
    {
        var command = ValidCommand(userName: "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var command = ValidCommand(email: "invalid");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var command = ValidCommand(password: "123");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = ValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}