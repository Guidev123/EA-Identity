using EA.CommonLib.MessageBus;
using EA.CommonLib.MessageBus.Integration.RegisteredCustomer;
using EA.CommonLib.MessageBus.Integration;
using EA.CommonLib.Responses;
using IdentityService.API.DTOs;
using Microsoft.AspNetCore.Identity;
using EA.CommonLib.MessageBus.Integration.DeleteCustomer;
using FluentValidation;
using FluentValidation.Results;
using IdentityService.API.DTOs.Validations;
using IdentityService.API.Interfaces;

namespace IdentityService.API.Services
{
    public class AuthenticationService(SignInManager<IdentityUser> signInManager,
                                       UserManager<IdentityUser> userManager,
                                       ISecurityService jwt,
                                       IMessageBus messageBus)
                                     : IAuthenticationService
    {
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly ISecurityService _jwt = jwt;
        private readonly IMessageBus _messageBus = messageBus;

        public async Task<Response<ChangeUserPasswordDTO>> ChangePasswordAsync(ChangeUserPasswordDTO dto)
        {
            var validationResult = ValidateEntity(new ChangeUserPasswordValidation(), dto);

            if (!validationResult.IsValid)
            {
                return new Response<ChangeUserPasswordDTO>(null, 400, "Error", GetAllErrors(validationResult));
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
            {
                AddError(validationResult, "User not found");
                return new Response<ChangeUserPasswordDTO>(null, 404, "Error", GetAllErrors(validationResult));
            }

            var checkPasswordResult = await _userManager.CheckPasswordAsync(user, dto.OldPassword);
            if (!checkPasswordResult)
            {
                AddError(validationResult, "Your old password is wrong");
                return new Response<ChangeUserPasswordDTO>(null, 400, "Error", GetAllErrors(validationResult));
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                AddError(validationResult, "You can not change your password");
                return new Response<ChangeUserPasswordDTO>(null, 400, "Error", GetAllErrors(validationResult));
            }

            return new Response<ChangeUserPasswordDTO>(null, 204);
        }

        public async Task<Response<DeleteCustomerIntegrationEvent>> DeleteAsync(Guid id)
        {
            var deleteEvent = new DeleteCustomerIntegrationEvent(id);

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user is null) return new Response<DeleteCustomerIntegrationEvent>(null, 404, "User not found");

            var result = await _messageBus.RequestAsync<DeleteCustomerIntegrationEvent, ResponseMessage>(deleteEvent);

            if (result.ValidationResult.IsValid)
            {
                await _userManager.DeleteAsync(user);
                return new Response<DeleteCustomerIntegrationEvent>(null, 204);
            }

            return new Response<DeleteCustomerIntegrationEvent>(null, 400, "You can not delete this user now");
        }

        public async Task<Response<LoginResponseDTO>> LoginAsync(LoginUserDTO dto)
        {
            var validationResult = ValidateEntity(new LoginUserValidation(), dto);

            if (!validationResult.IsValid)
            {
                return new Response<LoginResponseDTO>(null, 400, "Error", GetAllErrors(validationResult));
            }

            var user = LoginUserDTO.MapToIdentity(dto);

            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, true);

            if (result.Succeeded)
                return new Response<LoginResponseDTO>(await _jwt.JwtGenerator(user), 200, "Success!");

            if (result.IsLockedOut)
            {
                AddError(validationResult, "Your account is locked");
                return new Response<LoginResponseDTO>(null, 400, "Error", GetAllErrors(validationResult));
            }

            AddError(validationResult, "Your credentials are wrong");
            return new Response<LoginResponseDTO>(null, 400, "Error", GetAllErrors(validationResult));
        }

        public async Task<Response<LoginResponseDTO>> RegisterAsync(RegisterUserDTO dto)
        {
            var validationResult = ValidateEntity(new RegisterUserValidation(), dto);

            if (!validationResult.IsValid)
            {
                return new Response<LoginResponseDTO>(null, 400, "Error", GetAllErrors(validationResult));
            }

            var user = RegisterUserDTO.MapToIdentity(dto);

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                var customerResult = await RegisterCustomer(dto);

                if (!customerResult.ValidationResult.IsValid)
                {
                    await _userManager.DeleteAsync(user);
                    return new Response<LoginResponseDTO>(null, 400, "Error", GetAllErrors(validationResult));
                }

                return new Response<LoginResponseDTO>(await _jwt.JwtGenerator(user), 201, "Success");
            }

            return new Response<LoginResponseDTO>(null, 400, "Error", GetAllErrorsIdentity(result));
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

        private ValidationResult ValidateEntity<TV, TE>(TV validation, TE entity) where TV
        : AbstractValidator<TE> where TE : class => validation.Validate(entity);
        private void AddError(ValidationResult validationResult, string message) =>
           validationResult.Errors.Add(new ValidationFailure(string.Empty, message));
        private string[] GetAllErrors(ValidationResult validationResult) =>
            validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
        private string[] GetAllErrorsIdentity(IdentityResult identityResult) =>
             identityResult.Errors.Select(e => e.Description).ToArray();
        #endregion
    }
}
