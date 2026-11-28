using FluentValidation;
using Hoofd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hoofd.Controllers;

public record RegisterRequest(string Email, string Password, DateOnly Birthday);

[ApiController]
[Route("auth")]
public class AuthController(UserManager<Horse> userManager) : ControllerBase
{
    [Route("register")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        IValidator<RegisterRequest> validator
    )
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));
        }

        var horse = new Horse
        {
            UserName = request.Email,
            Email = request.Email,
            Birthday = request.Birthday
        };

        var result = await userManager.CreateAsync(horse, request.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created);
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Birthday).NotNull().LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now).AddYears(-18));
    }
}