using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace Profile.ImageProcessing.Functions
{
    public static class ImageProcess
    {
        [FunctionName("ImageProcess")]
        public static void Run([BlobTrigger("images/{name}", Connection = "AzureWebJobsStorage")]Stream image, string name, ILogger log)
        {
            try
            {
                log.LogInformation($"{name} photo moving process started...");
                // The variable sourceConnection is a string holding the connection string for the storage account
                string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                string destinationBlobContainer = Environment.GetEnvironmentVariable("DestinationBlobContainer");
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                // Create the destination blob client
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer destClient = blobClient.GetContainerReference(destinationBlobContainer);
                CloudBlockBlob destBlob = destClient.GetBlockBlobReference(name);
                destBlob.UploadFromStreamAsync(image);
                log.LogInformation($"{name} photo has been moved to userphoto container.");

            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                throw;
            }
        }
    }
}
