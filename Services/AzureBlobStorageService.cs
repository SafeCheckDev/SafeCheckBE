using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Safecheck.Constants;

namespace Safecheck.Services
{
    public interface IAzureBlobStorageService
    {
        Task UploadBlobAsync(IFormFile file, string containerName, string name);
    }

    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private IConfiguration _config;
        private CloudBlobClient _blobClient;
        private ILogger<AzureBlobStorageService> _log;

        public AzureBlobStorageService(ILogger<AzureBlobStorageService> l, IConfiguration c)
        {
            _log = l;
            _config = c;

            // Get storage account
            var storageAccount = CloudStorageAccount.Parse(_config["ConnectionStrings:AzureStorageConnectionString"]);

            // Create client
            _blobClient = storageAccount.CreateCloudBlobClient();
            _blobClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

        }

        public async Task UploadBlobAsync(IFormFile file, string containerName, string name)
        {
            
            // Get container
            var container = _blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            var blockBlob = container.GetBlockBlobReference(name);
            blockBlob.Properties.ContentType = file.ContentType;

            try
            {
                _log.LogInformation("Uploading blob: " + name);

                using (var stream = file.OpenReadStream())
                {
                    await blockBlob.UploadFromStreamAsync(stream);
                }
                _log.LogInformation("Uploaded blob: " + name);

            }
            catch (Exception e)
            {
                _log.LogError("Error uploading blob",e);
                throw;
            }
        }

        

    }
}
