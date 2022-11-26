using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net7CoreApiBoilerplate.Api.Models;
using Net7CoreApiBoilerplate.DbContext.Entities.Identity;

namespace Net7CoreApiBoilerplate.Api.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
{
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserRolesController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager
            )
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        [HttpGet("Get/{Id}")]
        public async Task<IActionResult> Get(long id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            return Ok(await _userManager.GetRolesAsync(user).ConfigureAwait(false));
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] UserRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            ApplicationUser user = await _userManager.FindByIdAsync(model.UserId.ToString()).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new string[] { "Could not find user!" });

            ApplicationRole role = await _roleManager.FindByIdAsync(model.RoleId.ToString()).ConfigureAwait(false);
            if (role == null)
                return BadRequest(new string[] { "Could not find role!" });

            IdentityResult result = await _userManager.AddToRoleAsync(user, role.Name).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok(role.Name);
            }
            return BadRequest(result.Errors.Select(x => x.Description));
        }

        [HttpDelete("Delete/{Id}/{RoleId}")]
        public async Task<IActionResult> Delete(string id, string roleId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            ApplicationUser user = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new string[] { "Could not find user!" });

            ApplicationRole role = await _roleManager.FindByIdAsync(roleId).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new string[] { "Could not find role!" });

            IdentityResult result = await _userManager.RemoveFromRoleAsync(user, role.Name).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors.Select(x => x.Description));
        }
    }
}
