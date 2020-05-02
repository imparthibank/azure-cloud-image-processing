using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Profile.ImageProcessing.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Profile.ImageProcessing.Services
{
    public class ImageStoreServices
    {
        private readonly AzureStorageConfig storageConfig;

        CloudBlobClient blobClient;

        public ImageStoreServices(IOptions<AzureStorageConfig> config)
        {
            storageConfig = config.Value;
            var credentials = new StorageCredentials(storageConfig.AccountName, storageConfig.AccountKey);
            blobClient = new CloudBlobClient(new Uri(storageConfig.BaseUri), credentials);
        }

        public async Task<string> SaveImage(Stream imageStream)
        {
            var imageId = Guid.NewGuid().ToString();
            var container = blobClient.GetContainerReference(storageConfig.ImageContainer);
            var blob = container.GetBlockBlobReference(imageId);
            await blob.UploadFromStreamAsync(imageStream);
            return imageId;
        }

        public string UriFor(string imageId)
        {
            var sasPolicy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(15)
            };

            var container = blobClient.GetContainerReference(storageConfig.ImageContainer);
            var blob = container.GetBlockBlobReference(imageId);
            var sas = blob.GetSharedAccessSignature(sasPolicy);
            return $"{storageConfig.BaseUri}{storageConfig.ImageContainer}/{imageId}{sas}";
        }
    }
}
