using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Net7CoreApiBoilerplate.Infrastructure.DbUtility;

namespace Net7CoreApiBoilerplate.Api.Infrastructure
{
    public abstract class BaseController : ControllerBase
    {
        // Currently logged in Publisher
        // public long CurrentPublisherId => this.HttpContext.GetCurrentPublisherId();

        // Managing files (downloading & converting)
        [NonAction] // I don't want this to be called outside my controllers 
        internal async Task<Stream> GetFileStream(string filePath)
        {
            var fileContents = new FileStream(filePath, FileMode.Open);
            return fileContents;
        }

        [NonAction] // I don't want this to be called outside my controllers 
        internal async Task<FileStreamResult> DownloadFile(string filePath)
        {
            var fileContents = await GetFileStream(filePath);
            var fileName = filePath.Split('/').Last();
            return File(fileContents, MediaTypeNames.Application.Pdf, fileName);
        }

        // Database related stuff (unit of work & repository)
        public IUnitOfWork UnitOfWork { get; set; }

        [NonAction]
        protected void Commit()
        {
            UnitOfWork.Commit();
        }
    }
}
