using EA.CommonLib.Controllers;
using IdentityService.API.DTOs;
using IdentityService.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/v1/authentication")]
public class IdentityController(IAuthenticationService authenticationService) : MainController
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    [HttpPost]
    public async Task<IResult> RegisterAsync(RegisterUserDTO registerUser)
    {
        var result = await _authenticationService.RegisterAsync(registerUser);

        return CustomResponse(result);
    }

    [HttpPost("login")]
    public async Task<IResult> LoginAsync(LoginUserDTO loginUser)
    {
        var result = await _authenticationService.LoginAsync(loginUser);

        return CustomResponse(result);
    }

    [Authorize]
    [HttpPatch("change-password")]
    public async Task<IResult> ChangePasswordAsync(ChangeUserPasswordDTO changeUserPassword)
    {
        var result = await _authenticationService.ChangePasswordAsync(changeUserPassword);

        return CustomResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteAsync(Guid id)
    {
        var result = await _authenticationService.DeleteAsync(id);

        return CustomResponse(result);
    }
}
