using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Net7CoreApiBoilerplate.Api.Models;
using Net7CoreApiBoilerplate.DbContext.Entities.Identity;
using Net7CoreApiBoilerplate.Infrastructure.Settings;
using Net7CoreApiBoilerplate.Services.Email;
using Net7CoreApiBoilerplate.Utility.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Net7CoreApiBoilerplate.Api.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IJwtSettings _jwt;
        private readonly IClientAppSettings _clientAppSettings;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration,
            IEmailService emailService,
            IJwtSettings jwt,
            SignInManager<ApplicationUser> signInManager,
            IClientAppSettings clientAppSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _jwt = jwt;
            _signInManager = signInManager;
            _clientAppSettings = clientAppSettings;
        }

        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailViewModel model)
        {
            if (model.UserId == null || model.Code == null)
            {
                return BadRequest(new string[] { "Error retrieving information!" });
            }

            ApplicationUser user = await _userManager.FindByIdAsync(model.UserId).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new string[] { "Could not find user!" });

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, model.Code).ConfigureAwait(false);
            if (!result.Succeeded)
                return BadRequest(new string[] { "Something went wront confirming user email." });

            var lockoutEnabledResult = await _userManager.SetLockoutEnabledAsync(user, false);
            if (!lockoutEnabledResult.Succeeded)
            {
                return BadRequest(new string[] { "Something went wront disabling user lockout." });
            }

            return Ok();
            // return BadRequest(result.Errors.Select(x => x.Description));
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            ApplicationUser user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,

                FirstName = model.FirstName,
                LastName = model.LastName,
                Title = model.Title,
                BirthDate = model.BirthDate,
                // Gender =  model.Gender,
            };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

            if (result.Succeeded)
            {
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

                var callbackUrl = $"{_clientAppSettings.ClientBaseUrl}{_clientAppSettings.EmailConfirmationPath}?uid={user.Id}&code={System.Net.WebUtility.UrlEncode(code)}";
                await _emailService.SendEmailConfirmationAsync(model.Email, callbackUrl).ConfigureAwait(false);

                return Ok();
            }

            return BadRequest(result.Errors.Select(x => x.Description));
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new string[] { "Invalid credentials." });

            TokenModel tokenModel = new TokenModel()
            {
                HasVerifiedEmail = false
            };

            // Only allow login if email is confirmed
            if (!user.EmailConfirmed)
            {
                return Ok(tokenModel);
            }

            // Used as user lock
            if (user.LockoutEnabled)
                return BadRequest(new string[] { "This account has been locked." });

            if (await _userManager.CheckPasswordAsync(user, model.Password).ConfigureAwait(false))
            {
                tokenModel.HasVerifiedEmail = true;

                if (user.TwoFactorEnabled)
                {
                    tokenModel.TFAEnabled = true;
                    return Ok(tokenModel);
                }
                else
                {
                    JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user, model.RememberMe).ConfigureAwait(false);
                    tokenModel.TFAEnabled = false;
                    tokenModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                    return Ok(tokenModel);
                }
            }

            return BadRequest(new string[] { "Invalid login attempt." });
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok(); // LOL
        }

        [HttpPost("LoginWith2fa")]
        public async Task<IActionResult> LoginWith2fa([FromBody] LoginWith2faViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            ApplicationUser user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new string[] { "Invalid credentials." });

            if (await _userManager.VerifyTwoFactorTokenAsync(user, "Authenticator", model.TwoFactorCode).ConfigureAwait(false))
            {
                JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user, model.RememberMe).ConfigureAwait(false);

                TokenModel tokenModel = new TokenModel()
                {
                    HasVerifiedEmail = true,
                    TFAEnabled = false,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
                };

                return Ok(tokenModel);

            }
            return BadRequest(new string[] { "Unable to verify Authenticator Code!" });
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            ApplicationUser user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false)))
                return BadRequest(new string[] { "Please verify your email address." });

            string code = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

            var callbackUrl = $"{_clientAppSettings.ClientBaseUrl}{_clientAppSettings.ResetPasswordPath}?uid={user.Id}&code={System.Net.WebUtility.UrlEncode(code)}";
            await _emailService.SendPasswordResetAsync(model.Email, callbackUrl).ConfigureAwait(false);

            return Ok();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            ApplicationUser user = await _userManager.FindByIdAsync(model.UserId).ConfigureAwait(false);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest(new string[] { "Invalid credentials." });
            }
            IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors.Select(x => x.Description));
        }

        [HttpPost("ResendVerificationEmail")]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] UserViewModel model)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new string[] { "Could not find user!" });

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

            var callbackUrl = $"{_clientAppSettings.ClientBaseUrl}{_clientAppSettings.EmailConfirmationPath}?uid={user.Id}&code={System.Net.WebUtility.UrlEncode(code)}";
            await _emailService.SendEmailConfirmationAsync(user.Email, callbackUrl).ConfigureAwait(false);

            return Ok();
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user, bool rememberMe)
        {
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            IList<string> roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            string ipAddress = IpHelper.GetIpAddress();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString()),
                new Claim("aid", user.Author.ApplicationUserId.ToString()),
                new Claim("ip", ipAddress)
            }
            .Union(userClaims)
            .Union(roleClaims);

            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes + (rememberMe ? _jwt.RememberMeDurationInHours * 60 : 0)),
            signingCredentials: signingCredentials
            );
            return jwtSecurityToken;
        }
    }
}
