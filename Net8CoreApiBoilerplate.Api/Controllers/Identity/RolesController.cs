using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net8CoreApiBoilerplate.Api.Infrastructure;
using Net8CoreApiBoilerplate.Api.Models;
using Net8CoreApiBoilerplate.DbContext.Entities.Identity;

namespace Net8CoreApiBoilerplate.Api.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : BaseController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesController(RoleManager<ApplicationRole> roleManager)
        {
            this._roleManager = roleManager;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApplicationRole>), 200)]
        [Route("get")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(
                    _roleManager.Roles
                        .Select(role => new
                        {
                            role.Id,
                            role.Name
                        }));
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }


        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole(RoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            var identityRole = new ApplicationRole { Name = model.Name };

            var result = await _roleManager.CreateAsync(identityRole).ConfigureAwait(false);
            
            if (!result.Succeeded) 
                return BadRequest(result.Errors.Select(x => x.Description));

            return Ok(new
            {
                identityRole.Id,
                identityRole.Name
            });
        }

        [HttpPut]
        [Route("UpdateRole/{Id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleModel model)
        {
            // Welcome to shit-show: https://stackoverflow.com/questions/36983656/identity-3-0-getting-a-user-by-id-when-the-id-is-int
            // var identityRole = _roleManager.Roles.FirstOrDefault(s => s.Id == Id);
            var identityRole = await _roleManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);

            if (identityRole == null)
                return NotFound("Could not find role!");

            identityRole.Name = model.Name;

            var result = await _roleManager.UpdateAsync(identityRole).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors.Select(x => x.Description));
        }

        [HttpDelete]
        [Route("Remove/{Id}")]
        public async Task<IActionResult> Remove(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
                return BadRequest(new[] { "Could not complete request!" });

            var identityRole = await _roleManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            if (identityRole == null)
                return BadRequest(new [] { "Could not find role!" });

            var result = await _roleManager.DeleteAsync(identityRole).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors.Select(x => x.Description));
        }
    }
}
