using EA.CommonLib.Controllers;
using EA.CommonLib.Responses;
using IdentityService.API.DTOs;
using IdentityService.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/v1/authentication")]
public class IdentityController(IAuthenticationService authenticationService) : MainController
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Response<LoginResponseDTO>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<LoginResponseDTO>))]
    [HttpPost]
    public async Task<IResult> RegisterAsync(RegisterUserDTO registerUser)
    {
        var result = await _authenticationService.RegisterAsync(registerUser);

        return CustomResponse(result);
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<LoginResponseDTO>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<LoginResponseDTO>))]
    [HttpPost("login")]
    public async Task<IResult> LoginAsync(LoginUserDTO loginUser)
    {
        var result = await _authenticationService.LoginAsync(loginUser);

        return CustomResponse(result);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<object>))]
    [Authorize]
    [HttpPatch("change-password")]
    public async Task<IResult> ChangePasswordAsync(ChangeUserPasswordDTO changeUserPassword)
    {
        var result = await _authenticationService.ChangePasswordAsync(changeUserPassword);

        return CustomResponse(result);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<object>))]
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteAsync(Guid id)
    {
        var result = await _authenticationService.DeleteAsync(id);

        return CustomResponse(result);
    }
}
