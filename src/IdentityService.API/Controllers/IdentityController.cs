using EA.CommonLib.Controllers;
using EA.CommonLib.MessageBus;
using EA.CommonLib.MessageBus.Integration;
using EA.CommonLib.MessageBus.Integration.RegisteredCustomer;
using IdentityService.API.DTOs;
using IdentityService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[Route("api/v1/authentication")]
public class IdentityController(SignInManager<IdentityUser> signInManager,
                      UserManager<IdentityUser> userManager,
                      IAuthenticationService jwt,
                      IMessageBus messageBus) : MainController
{
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IAuthenticationService _jwt = jwt;
    private readonly IMessageBus _messageBus = messageBus;

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterUserDTO registerUser)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var user = RegisterUserDTO.MapToIdentity(registerUser);

        var result = await _userManager.CreateAsync(user, registerUser.Password);

        if (result.Succeeded)
        {
            var customerResult = await RegisterCustomer(registerUser);

            if (!customerResult.ValidationResult.IsValid)
            {
                await _userManager.DeleteAsync(user);
                return CustomResponse(customerResult.ValidationResult);
            }

            return CustomResponse(await _jwt.JwtGenerator(user));
        }

        foreach (var error in result.Errors)
            AddProcessError(error.Description);

        return CustomResponse(registerUser);
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
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var user = LoginUserDTO.MapToIdentity(loginUser);

        var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

        if (result.Succeeded)
            return CustomResponse(await _jwt.JwtGenerator(user));

        if (result.IsLockedOut)
        {
            AddProcessError("The user has been temporarily locked out due to invalid attempts");
            return CustomResponse(loginUser);
        }

        AddProcessError("Incorrect username or password");
        return CustomResponse(loginUser);
    }

    [Authorize]
    [HttpPatch("change-password")]
    public async Task<ActionResult> ChangePasswordAsync(ChangeUserPasswordDTO changeUserPassword)
    {
        if (!ModelState.IsValid)
            return CustomResponse(ModelState);

        var user = await _userManager.FindByEmailAsync(changeUserPassword.Email);
        if (user is null)
            return NotFound("User not found");

        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, changeUserPassword.OldPassword);
        if (!checkPasswordResult)
        {
            AddProcessError("Incorrect password");
            return CustomResponse(changeUserPassword);
        }

        var result = await _userManager.ChangePasswordAsync(user, changeUserPassword.OldPassword, changeUserPassword.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) AddProcessError(error.Description);
            return CustomResponse(changeUserPassword);
        }

        return CustomResponse(changeUserPassword);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return CustomResponse();
    }
}
