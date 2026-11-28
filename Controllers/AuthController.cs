using FluentValidation;
using Hoofd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hoofd.Controllers;

public record RegisterRequest(string Email, string Password, DateOnly Birthday);

public record LoginRequest(string Email, string Password);

[ApiController]
[Route("auth")]
public class AuthController(
    UserManager<Horse> userManager,
    SignInManager<Horse> signInManager
) : ControllerBase
{

    [Route("login")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        LoginRequest request,
        IValidator<LoginRequest> validator
    )
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));


        // Cookie.
        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

        var result = await signInManager.PasswordSignInAsync(
            request.Email,
            request.Password,
            isPersistent: true,
            lockoutOnFailure: true
        );

        if (result.Succeeded)
            return NoContent();

        if (result.IsLockedOut)
            return Problem(
                "Too many failed attempt, please try again later",
                statusCode: StatusCodes.Status423Locked
            );

        
        return Problem("Invalid credentials", statusCode: StatusCodes.Status401Unauthorized);
    }

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

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var horse = await userManager.GetUserAsync(User);
        return horse is null
            ? Unauthorized()
            : Ok(new { horse.Id, horse.Email, horse.Birthday });
    }

    [Authorize]
    [HttpDelete("logout")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
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
