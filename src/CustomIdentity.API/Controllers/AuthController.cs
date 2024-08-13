using CustomIdentity.API.Controllers.FreelanceCoders.Core.Controllers;
using CustomIdentity.API.Extensions;
using CustomIdentity.API.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static CustomIdentity.API.DTOs.UserDTO;

namespace CustomIdentity.API.Controllers;

[Route("api/auth")]
public class AuthController : MainController
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppSettings _appSettings;
    private readonly IJasonWebToken _jwt;
    public AuthController(IOptions<AppSettings> appSettings,
                          SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager,
                          IJasonWebToken jwt)
    {
        _appSettings = appSettings.Value;
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

        if (result.Succeeded)
        {
            return CustomResponse(await _jwt.JwtGenerator(user.Email));
        }

        foreach (var error in result.Errors)
        {
            AddProcessError(error.Description);
        }

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
}
