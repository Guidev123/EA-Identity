using EA.IntegrationEvents.Integration;
using EA.IntegrationEvents.Integration.DeletedUser;
using EA.IntegrationEvents.Integration.RegisteredUser;
using FluentValidation;
using FluentValidation.Results;
using IdentityService.API.DTOs;
using IdentityService.API.DTOs.Validations;
using IdentityService.API.Extensions;
using IdentityService.API.Helpers;
using IdentityService.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using SharedLib.Domain.Responses;
using SharedLib.MessageBus;

namespace IdentityService.API.Services
{
    public class AuthenticationService(SignInManager<IdentityUser> signInManager,
                                       UserManager<IdentityUser> userManager,
                                       ITokenGeneratorService jwt,
                                       IMessageBus messageBus)
                                     : IAuthenticationService
    {
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly ITokenGeneratorService _jwt = jwt;
        private readonly IMessageBus _messageBus = messageBus;

        public async Task<Response<LoginResponseDTO>> LoginAsync(LoginUserDTO dto)
        {
            var validationResult = ValidateEntity(new LoginUserValidation(), dto);

            if (!validationResult.IsValid)
                return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));

            var user = LoginUserDTO.MapToIdentity(dto);

            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, true);

            if (result.Succeeded)
                return new(await _jwt.JwtGenerator(user), 200, ErrorsMessage.SUCCESS.GetDescription());

            if (result.IsLockedOut)
            {
                AddError(validationResult, ErrorsMessage.LOCKED_ACCOUNT.GetDescription());
                return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
            }

            AddError(validationResult, ErrorsMessage.WRONG_CREDENTIALS.GetDescription());
            return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
        }

        public async Task<Response<LoginResponseDTO>> RegisterAsync(RegisterUserDTO dto)
        {
            var validationResult = ValidateEntity(new RegisterUserValidation(), dto);

            if (!validationResult.IsValid)
                return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));

            var user = RegisterUserDTO.MapToIdentity(dto);

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                var customerResult = await RegisterCustomer(dto);

                if (!customerResult.ValidationResult.IsValid)
                {
                    await _userManager.DeleteAsync(user);
                    return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(customerResult.ValidationResult));
                }

                return new(await _jwt.JwtGenerator(user), 201, ErrorsMessage.SUCCESS.GetDescription());
            }

            return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrorsIdentity(result));
        }

        public async Task<Response<ChangeUserPasswordDTO>> ChangePasswordAsync(ChangeUserPasswordDTO dto)
        {
            var validationResult = ValidateEntity(new ChangeUserPasswordValidation(), dto);

            if (!validationResult.IsValid)
            {
                return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
            {
                AddError(validationResult, ErrorsMessage.USER_NOT_FOUND.GetDescription());
                return new(null, 404, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
            }

            var checkPasswordResult = await _userManager.CheckPasswordAsync(user, dto.OldPassword);
            if (!checkPasswordResult)
            {
                AddError(validationResult, ErrorsMessage.WRONG_CREDENTIALS.GetDescription());
                return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                AddError(validationResult, ErrorsMessage.CANT_CHANGE_PASSWORD.GetDescription());
                return new(null, 400, ErrorsMessage.ERROR.GetDescription(), GetAllErrors(validationResult));
            }

            return new(null, 204, ErrorsMessage.SUCCESS.GetDescription());
        }

        public async Task<Response<DeletedUserIntegrationEvent>> DeleteAsync(Guid id)
        {
            var deleteEvent = new DeletedUserIntegrationEvent(id);

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user is null) return new(null, 404, ErrorsMessage.USER_NOT_FOUND.GetDescription());

            var result = await _messageBus.RequestAsync<DeletedUserIntegrationEvent, ResponseMessage>(deleteEvent);

            if (result.ValidationResult.IsValid)
            {
                await _userManager.DeleteAsync(user);
                return new(null, 204, ErrorsMessage.SUCCESS.GetDescription());
            }

            return new(null, 400, ErrorsMessage.CANT_DELETE_USER.GetDescription());
        }


        #region Helpers
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

        private static ValidationResult ValidateEntity<TV, TE>(TV validation, TE entity) where TV
        : AbstractValidator<TE> where TE : class => validation.Validate(entity);
        private static void AddError(ValidationResult validationResult, string message) =>
           validationResult.Errors.Add(new ValidationFailure(string.Empty, message));
        private static string[] GetAllErrors(ValidationResult validationResult) =>
            validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
        private static string[] GetAllErrorsIdentity(IdentityResult identityResult) =>
             identityResult.Errors.Select(e => e.Description).ToArray();
        #endregion
    }
}
