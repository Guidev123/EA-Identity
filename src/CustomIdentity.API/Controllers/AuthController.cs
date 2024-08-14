using CustomIdentity.API.Controllers.FreelanceCoders.Core.Controllers;
using CustomIdentity.API.DTOs;
using CustomIdentity.API.Extensions;
using CustomIdentity.API.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using static CustomIdentity.API.DTOs.UserDTO;

namespace CustomIdentity.API.Controllers;

[Route("api/auth")]
public class AuthController : MainController
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJasonWebToken _jwt;
    public AuthController(SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager,
                          IJasonWebToken jwt)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwt = jwt;
    }
    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterUserDTO registerUser)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var user = new IdentityUser
        {
            UserName = registerUser.Email,
            Email = registerUser.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, registerUser.Password);

        if (result.Succeeded) return CustomResponse(await _jwt.JwtGenerator(user.Email));

        foreach (var error in result.Errors) AddProcessError(error.Description);

        return CustomResponse(registerUser);
    }

    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync(LoginUserDTO loginUser)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

        if (result.Succeeded) return CustomResponse(await _jwt.JwtGenerator(loginUser.Email));

        if (result.IsLockedOut)
        {
            AddProcessError("O usuario foi temporariamente bloqueado por tentativas inválidas ");
            return CustomResponse(loginUser);
        }

        AddProcessError("Usuario ou senha incorretos");
        return CustomResponse(loginUser);
    }
    [HttpPut("change-password")]
    public async Task<ActionResult> ChangePasswordAsync(ChangeUserPasswordDTO changeUserPassword)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var user = await _userManager.FindByEmailAsync(changeUserPassword.Email);
        if (user == null) return NotFound("Usuário não encontrado");

        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, changeUserPassword.OldPassword);
        if (!checkPasswordResult)
        {
            AddProcessError("Senha antiga incorreta");
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

    [HttpPost]
    public async Task<ActionResult> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return CustomResponse();
    }

}
