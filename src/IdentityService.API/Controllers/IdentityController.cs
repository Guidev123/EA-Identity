using IdentityService.API.Application.UseCases.ChangePassword;
using IdentityService.API.Application.UseCases.Delete;
using IdentityService.API.Application.UseCases.ForgotPassword;
using IdentityService.API.Application.UseCases.Login;
using IdentityService.API.Application.UseCases.RefreshToken;
using IdentityService.API.Application.UseCases.Register;
using IdentityService.API.Application.UseCases.ResetPassword;
using IdentityService.API.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Domain.Responses;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class IdentityController(IMediator mediator) : MainController
{
    private readonly IMediator _mediator = mediator;

    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Response<LoginResponseDTO>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<LoginResponseDTO>))]
    [HttpPost]
    public async Task<IResult> RegisterAsync(RegisterUserCommand command) =>
        CustomResponse(await _mediator.Send(command));


    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<LoginResponseDTO>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<LoginResponseDTO>))]
    [HttpPost("login")]
    public async Task<IResult> LoginAsync(LoginUserCommand command) =>
        CustomResponse(await _mediator.Send(command));


    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<object>))]
    [Authorize]
    [HttpPatch("change-password")]
    public async Task<IResult> ChangePasswordAsync(ChangeUserPasswordCommand command) =>
        CustomResponse(await _mediator.Send(command));


    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<object>))]
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteAsync(Guid id) =>
        CustomResponse(await _mediator.Send(new DeleteUserCommand(id)));


    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<object>))]
    [HttpPost("refresh-token")]
    public async Task<IResult> RefreshTokenAsync(string refreshToken) =>
        CustomResponse(await _mediator.Send(new RefreshTokenCommand(refreshToken)));


    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<object>))]
    [HttpPost("forgot-password")]
    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordCommand command) =>
        CustomResponse(await _mediator.Send(command));


    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<object>))]
    [HttpPost("reset-password")]
    public async Task<IResult> ResetPasswordAsync(ResetPasswordCommand command) =>
        CustomResponse(await _mediator.Send(command));
}
