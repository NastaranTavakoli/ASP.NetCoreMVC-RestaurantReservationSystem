using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using ReservationSystemDraft_v1.Helpers;

namespace ReservationSystemDraft_v1.Services
{
    public class PhotoStorageService
    {
        public async Task<List<string>> StorePhotos(List<IFormFile> files, AzureStorageConfig storageConfig)
        {
            var images = files.Select(file => new {File = file , Name = Guid.NewGuid() + Path.GetExtension(file.FileName)}).ToList();
            var fileNames = new List<string>();

            foreach (var image in images)
            {
                var imageURL = $"https://{storageConfig.AccountName}.blob.core.windows.net/{storageConfig.ImageContainer}/{image.Name}";
                // Create a URI to the blob
                Uri blobUri = new Uri(imageURL);

                fileNames.Add(imageURL);

                // Create StorageSharedKeyCredentials object by reading
                // the values from the configuration (appsettings.json)
                StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(storageConfig.AccountName, storageConfig.AccountKey);

                // Create the blob client.
                BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

                // Upload the file
                await blobClient.UploadAsync(image.File.OpenReadStream());

            }
            return fileNames;
        }
    }
}