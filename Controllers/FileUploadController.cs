using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Safecheck.Services;
using Safecheck.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.ServiceBus;


namespace Safecheck.Controllers
{
    
    [Route("api/fileUpload")]
    public class FileUploadController : Controller
    {
        private IAzureBlobStorageService _azureBlobStorageService;
        private ILogger<FileUploadController> _log;
        private IConfiguration  _config;

        public FileUploadController(IAzureBlobStorageService abs, IConfiguration c, ILogger<FileUploadController> l)
        {
            _azureBlobStorageService = abs;
            _log = l;
            _config = c;
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
                
                // Add message on add-transaction queue
                var client = new QueueClient(_config[SafeCheckConstants.CONFIG_SB_ACCOUNT_CONNECTION_STRING], SafeCheckConstants.REGISTER_QUEUE);
                var stream = new MemoryStream();
                file.CopyTo(stream); 
                byte[] fileBytes  =  stream.ToArray();
                var msg = new Message(fileBytes);
                
                msg.UserProperties.Add("fileName", fileName);
                msg.UserProperties.Add("fileId", fileId);
                msg.UserProperties.Add("sourceId", sourceId);

                await client.SendAsync(msg);


                return new ObjectResult(new { success = true, newUuid = name });
            }
            catch(Exception e)
            {
                _log.LogError(e,"Error uploading document",new string[0]);
                return new ObjectResult(new { success = false });
            }
        }
    }
}



