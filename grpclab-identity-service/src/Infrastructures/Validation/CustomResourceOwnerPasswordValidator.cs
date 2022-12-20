using GRPCLab.IdentityService.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using GRPCLab.IdentityService.Infrastructures.Constants;

namespace GRPCLab.IdentityService.Infrastructures.Validation
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<CustomResourceOwnerPasswordValidator> _logger;

        public CustomResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager,
                                                        SignInManager<ApplicationUser> signInManager,
                                                        ILogger<CustomResourceOwnerPasswordValidator> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            _logger.LogInformation($"Start authenticating user with grant type 'ResourceOwner'...");
            try
            {
                var user = await _userManager.FindByNameAsync(context.UserName);
                if (user == null)
                {
                    _logger.LogWarning($"Found no user '{context.UserName}' to authenticate.");
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ResourceOwnerValidatorError.InvalidEmailOrPassword);
                    return;
                }

                bool loggedIn;
                // Logging in using individual password
                loggedIn = await _userManager.CheckPasswordAsync(user, context.Password);

                if (loggedIn)
                {
                    await _signInManager.SignInAsync(user, false);
                    context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.Password, null);
                }
                else
                {
                    _logger.LogWarning($"Fail to authenticate user '{context.UserName}' because the credential is invalid");
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ResourceOwnerValidatorError.InvalidEmailOrPassword);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error on authenticated user '{context.UserName}'");
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ResourceOwnerValidatorError.InvalidEmailOrPassword);
            }

            _logger.LogInformation($"End authenticating user with grant type 'ResourceOwner'!");
            return;
        }
    }
}
