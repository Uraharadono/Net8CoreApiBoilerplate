using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net7CoreApiBoilerplate.Api.Infrastructure;
using Net7CoreApiBoilerplate.Api.Models;
using Net7CoreApiBoilerplate.Api.Utility.Extensions;
using Net7CoreApiBoilerplate.DbContext.Entities.Identity;
using Net7CoreApiBoilerplate.Infrastructure.Settings;
using Net7CoreApiBoilerplate.Services.VueBoilerplate;
using Net7CoreApiBoilerplate.Services.VueBoilerplate.Dto;
using System.Linq;
using System.Threading.Tasks;

namespace Net7CoreApiBoilerplate.Api.Controllers
{
    /* ============================================================================
     * YOU DO NOT NEED THIS CONTROLLER
     * It was created only for the purposes of showing off different functionalities
     * for my other boilerplate project: "Vue 3 Webpack Boilerplate V2"
     * https://github.com/Uraharadono/Vue3WebpackBoilerplateV2/
     * ============================================================================ */

    [Route("api/[controller]")]
    [ApiController]
    public class VueBoilerplateController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IClientAppSettings _clientAppSettings;
        private readonly IVueBoilerplateService _vueBoilerplateService;


        public VueBoilerplateController(UserManager<ApplicationUser> userManager, IClientAppSettings clientAppSettings, IVueBoilerplateService vueBoilerplateService)
        {
            _userManager = userManager;
            _clientAppSettings = clientAppSettings;
            _vueBoilerplateService = vueBoilerplateService;
        }

        [HttpPost("RegisterCircumvent")]
        public async Task<IActionResult> RegisterCircumvent([FromBody] RegisterViewModel model)
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
                // await _emailService.SendEmailConfirmationAsync(model.Email, callbackUrl).ConfigureAwait(false);

                return Ok(callbackUrl);
            }

            return BadRequest(result.Errors.Select(x => x.Description));
        }

        [HttpPost("GetClients")]
        public async Task<IActionResult> GetClients(ClientsOverviewFilterDto filter)
        {
            var clients = await _vueBoilerplateService.GetClients(filter);
            return Ok(clients);
        }

        [HttpPost, Route("UploadMockDocuments")]
        public async Task<IActionResult> UploadMockDocuments([FromForm] MockDocumentViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelStateExtensions.GetErrorMessage(ModelState));

            var dto = new MockDocumentUploadDto
            {
                ArticleId = model.ArticleId,
                DocumentType = model.DocumentTypeId,
                FileData = await model.File.GetBytes(),
                FileName = model.File.FileName, // I need this property as well, since I have only byte array in my "services" project
                // CurrentPublisherId = CurrentPublisherId,
            };

            var result = await _vueBoilerplateService.AddArticleDocument(dto);
            return result ? Ok() : StatusCode(500);
        }
    }

    public class MockDocumentViewModel
    {
        public long ArticleId { get; set; }
        public string DocumentTypeId { get; set; }
        public IFormFile File { get; set; }

        // public long CurrentPublisherId { get; set; }
    }
}
