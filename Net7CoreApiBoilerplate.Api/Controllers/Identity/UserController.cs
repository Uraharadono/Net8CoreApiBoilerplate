using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net7CoreApiBoilerplate.Api.Models;
using Net7CoreApiBoilerplate.DbContext.Entities.Identity;

namespace Net7CoreApiBoilerplate.Api.Controllers.Identity
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager
            )
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IdentityUser>), 200)]
        [Route("Get")]
        public IActionResult Get() => Ok(
            _userManager.Users.Select(user => new
            {
                user.Id,
                user.Email,
                user.PhoneNumber,
                user.EmailConfirmed,
                user.LockoutEnabled,
                user.TwoFactorEnabled
            }));

        [HttpGet("Get/{Id}")]
        public IActionResult Get(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
                return BadRequest(new string[] { "Empty parameter!" });

            return Ok(_userManager.Users
                .Where(user => user.Id == id)
                .Select(user => new
                {
                    user.Id,
                    user.Email,
                    user.PhoneNumber,
                    user.EmailConfirmed,
                    user.LockoutEnabled,
                    user.TwoFactorEnabled
                })
                .FirstOrDefault());
        }

        [HttpPost("InsertWithRole")]
        public async Task<IActionResult> InsertWithRole([FromBody] UserViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            ApplicationUser user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = model.EmailConfirmed,
                PhoneNumber = model.PhoneNumber
            };

            ApplicationRole role = await _roleManager.FindByIdAsync(model.RoleId).ConfigureAwait(false);
            if (role == null)
                return BadRequest(new string[] { "Could not find role!" });

            IdentityResult result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                IdentityResult result2 = await _userManager.AddToRoleAsync(user, role.Name).ConfigureAwait(false);
                if (result2.Succeeded)
                {
                    return Ok(new
                    {
                        user.Id,
                        user.Email,
                        user.PhoneNumber,
                        user.EmailConfirmed,
                        user.LockoutEnabled,
                        user.TwoFactorEnabled
                    });
                }
            }
            return BadRequest(result.Errors.Select(x => x.Description));
        }

        [HttpPut("Update/{Id}")]
        public async Task<IActionResult> Put(int id, [FromBody] EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            ApplicationUser user = await _userManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new[] { "Could not find user!" });

            // Add more fields to update
            user.Email = model.Email;
            user.UserName = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;
            user.PhoneNumber = model.PhoneNumber;
            user.LockoutEnabled = model.LockoutEnabled;
            user.TwoFactorEnabled = model.TwoFactorEnabled;

            IdentityResult result = await _userManager.UpdateAsync(user).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors.Select(x => x.Description));
        }

        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (String.IsNullOrEmpty(id.ToString()))
                return BadRequest(new[] { "Empty parameter!" });

            ApplicationUser user = await _userManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new[] { "Could not find user!" });

            IdentityResult result = await _userManager.DeleteAsync(user).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors.Select(x => x.Description));
        }
    }
}
