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
    
    [Route("api/fileUpload")]
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
        //Rename of parameters.
        public async Task<IActionResult> upload(IList<IFormFile> fileData, string fileId, string fileName, string totalfilesize, string sourceId, string uploadType)
        {
            _log.LogInformation("Starting File Upload procedure");
            //We only take one file... lets not muddy the waters yet).
            var file = fileData.FirstOrDefault();
            if (file == null)
            {
                _log.LogError("File not found...it is null.");
                return NotFound();
            }

            _log.LogInformation("Got a file ok.");

            try
            {
                //unique name for the entry.
                string name = string.Concat(sourceId, "-", DateTime.UtcNow.ToString("yyyy-MM-dd"), "-", fileId);
                _log.LogInformation("Got a name of ["+name+"]");
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



