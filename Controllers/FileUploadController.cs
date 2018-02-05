using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Safecheck.Services;

namespace Safecheck.Controllers
{
    
    [Route("api/fileUupload")]
    public class FileUploadController : Controller
    {
        private IAzureBlobStorageService _azureBlobStorageService;
        private ILogger<FileUploadController> _log;

        public FileUploadController(IAzureBlobStorageService abs, ILogger<FileUploadController> l)
        {
            _azureBlobStorageService = abs;
            _log = l;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Index(IList<IFormFile> qqfile, string qquuid, string qqfilename, string qqtotalfilesize, string resourceId, string uploadType)
        {
            //We only take one file... lets not muddy the waters yet).
            var file = qqfile.FirstOrDefault();
            if (file == null)
                return NotFound();
            try
            {
                //unique name for the entry.
                string name = string.Concat(resourceId, "-", DateTime.UtcNow.ToString("yyyy-MM-dd"), "-", qquuid);
                await _azureBlobStorageService.UploadBlobAsync(file, "incomingfiles", name);
                
                return new ObjectResult(new { success = true, newUuid = name });
            }
            catch(Exception e)
            {
                _log.LogError("Error uploading document",e);
                return new ObjectResult(new { success = false });
            }
        }
    }
}



