using GRPCLab.IdentityService.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using System.Linq;
using IdentityModel;
using System.Collections.Generic;

namespace GRPCLab.IdentityService.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(UserManager<ApplicationUser> userManager,
                                IOptionsSnapshot<AppSettings> settings,
                                ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _settings = settings;
            _logger = loggerFactory.CreateLogger<ProfileService>();

        }

        async public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));
            var subjectId = subject.Claims.Where(x => x.Type == JwtClaimTypes.Subject).FirstOrDefault().Value;

            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
            {
                throw new ArgumentException("Invalid subject identifier");
            }

            context.IssuedClaims = GetApiClaims(user, subject.Claims.ToList()).ToList();
            return;
        }

        async public Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var id = subject.Claims.Where(x => x.Type == JwtClaimTypes.Subject).FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(id);

            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user);
                        if (db_security_stamp != security_stamp)
                            return;
                    }
                }

                context.IsActive =
                    !user.LockoutEnabled ||
                    !user.LockoutEnd.HasValue ||
                    user.LockoutEnd <= DateTime.Now;
            }
        }

        private IEnumerable<Claim> GetApiClaims(ApplicationUser user, List<Claim> defaultClaims)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.UpdatedAt, DateTime.UtcNow.ToEpochTime().ToString()),
                new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim("_sub", user.Id.ToString()),
                new Claim("display_name", user.DisplayName)
            };

            return claims;
        }
    }
}
