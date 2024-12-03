using EA.CommonLib.MessageBus;
using EA.CommonLib.MessageBus.Integration;
using EA.CommonLib.MessageBus.Integration.DeleteCustomer;
using EA.CommonLib.MessageBus.Integration.RegisteredCustomer;
using EA.CommonLib.Responses;
using IdentityService.API.DTOs;
using IdentityService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/v1/authentication")]
public class IdentityController(SignInManager<IdentityUser> signInManager,
                      UserManager<IdentityUser> userManager,
                      IAuthenticationService jwt,
                      IMessageBus messageBus) : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IAuthenticationService _jwt = jwt;
    private readonly IMessageBus _messageBus = messageBus;

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterUserDTO registerUser)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = RegisterUserDTO.MapToIdentity(registerUser);

        var result = await _userManager.CreateAsync(user, registerUser.Password);

        if (result.Succeeded)
        {
            var customerResult = await RegisterCustomer(registerUser);

            if (!customerResult.ValidationResult.IsValid)
            {
                await _userManager.DeleteAsync(user);
                var errors = string.Join(", ", customerResult.ValidationResult.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new Response<RegisterUserDTO>(null, 400, errors));
            }

            return Ok(new Response<LoginResponseDTO>(await _jwt.JwtGenerator(user), 200, "Success"));
        }

        return BadRequest(new Response<RegisterUserDTO>(null, 400, "Something has failed during your authentication"));
    }

    private async Task<ResponseMessage> RegisterCustomer(RegisterUserDTO userDTO)
    {
        var user = await _userManager.FindByEmailAsync(userDTO.Email);
        var registeredUser = new RegisteredUserIntegrationEvent(Guid.Parse(user.Id), userDTO.Name, userDTO.Email, userDTO.Cpf);

        try
        {
            return await _messageBus.RequestAsync<RegisteredUserIntegrationEvent, ResponseMessage>(registeredUser);
        }

        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync(LoginUserDTO loginUser)
    {
        if (!ModelState.IsValid) return BadRequest(new Response<ModelStateDictionary>(ModelState, 400, "Error"));

        var user = LoginUserDTO.MapToIdentity(loginUser);

        var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

        if (result.Succeeded)
            return Ok(new Response<LoginResponseDTO>(await _jwt.JwtGenerator(user), 200, "Success!"));

        if (result.IsLockedOut)
        {
            return BadRequest(new Response<LoginResponseDTO>(null, 400, "Your account is locked"));
        }

        return BadRequest(new Response<RegisterUserDTO>(null, 400, "Your credentials are wrong"));
    }

    [Authorize]
    [HttpPatch("change-password")]
    public async Task<ActionResult> ChangePasswordAsync(ChangeUserPasswordDTO changeUserPassword)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(changeUserPassword.Email);
        if (user is null) return NotFound(new Response<ChangeUserPasswordDTO>(null, 404));

        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, changeUserPassword.OldPassword);
        if (!checkPasswordResult)
        {
            return BadRequest(new Response<ChangeUserPasswordDTO>(null, 400, "Your old password is wrong"));
        }

        var result = await _userManager.ChangePasswordAsync(user, changeUserPassword.OldPassword, changeUserPassword.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new Response<ChangeUserPasswordDTO>(null, 400, "You can not change your password"));
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        var deleteEvent = new DeleteCustomerIntegrationEvent(id);

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound(new Response<DeleteCustomerIntegrationEvent>(null, 404));

        var result = await _messageBus.RequestAsync<DeleteCustomerIntegrationEvent, ResponseMessage>(deleteEvent);

        if (result.ValidationResult.IsValid)
        {
            await _userManager.DeleteAsync(user);
            return NoContent();
        }

        return BadRequest(new Response<DeleteCustomerIntegrationEvent>(null, 400, "You can not delete this user now"));
    }
}
