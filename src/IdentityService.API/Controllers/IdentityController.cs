using IdentityService.API.DTOs;
using IdentityService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/v1/authentication")]
public class IdentityController(IAuthenticationService authenticationService) : ControllerBase
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    [HttpPost]
    public async Task<IResult> RegisterAsync(RegisterUserDTO registerUser)
    {
        var result = await _authenticationService.RegisterAsync(registerUser);

        return result.IsSuccess ? TypedResults.Created() : TypedResults.BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IResult> LoginAsync(LoginUserDTO loginUser)
    {
        var result = await _authenticationService.LoginAsync(loginUser);

        return result.IsSuccess ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }

    [Authorize]
    [HttpPatch("change-password")]
    public async Task<IResult> ChangePasswordAsync(ChangeUserPasswordDTO changeUserPassword)
    {
        var result = await _authenticationService.ChangePasswordAsync(changeUserPassword);

        return result.IsSuccess ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteAsync(Guid id)
    {
        var result = await _authenticationService.DeleteAsync(id);

        return result.IsSuccess ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }
}
