using CustomIdentity.API.Controllers.FreelanceCoders.Core.Controllers;
using CustomIdentity.API.DTOs;
using CustomIdentity.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CustomIdentity.API.Controllers;

[Route("api/v1/authentication")]
public class IdentityController(SignInManager<IdentityUser> signInManager,
                      UserManager<IdentityUser> userManager,
                      IAuthenticationService jwt) : MainController
{
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IAuthenticationService _jwt = jwt;

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterUserDTO registerUser)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var user = RegisterUserDTO.MapToIdentity(registerUser);

        var result = await _userManager.CreateAsync(user, registerUser.Password);

        if (result.Succeeded)
            return CustomResponse(await _jwt.JwtGenerator(user));

        foreach (var error in result.Errors)
            AddProcessError(error.Description);

        return CustomResponse(registerUser);
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

    [HttpPut("change-password")]
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

    [HttpPost("logout")]
    public async Task<ActionResult> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return CustomResponse();
    }
}
