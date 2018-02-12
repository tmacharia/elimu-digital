using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using DAL.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Services
{
    public class UploaderService : IUploader
    {
        private readonly string ST_Connection_string = "";

        public UploaderService(string blobAccount)
        {
            ST_Connection_string = blobAccount;
        }

        public async Task<string> Upload(IFile file)
        {
            // Upload to azure
            return await Upload(file, file.FileName.Split('.').Last());
        }
        private async Task<string> Upload(IFile file, string extension)
        {
            //retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ST_Connection_string);

            //create a blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //retreive reference to a container
            CloudBlobContainer container = null;

            switch (file.Format)
            {
                case FormatType.Audio:
                    container = blobClient.GetContainerReference("audios");
                    break;
                case FormatType.Video:
                    container = blobClient.GetContainerReference("videos");
                    break;
                case FormatType.Document:
                    container = blobClient.GetContainerReference("documents");
                    break;
                case FormatType.Image:
                    container = blobClient.GetContainerReference("images");
                    break;
                default:
                    break;
            }

            //block blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{Guid.NewGuid()}.{extension}");

            await blockBlob.UploadFromStreamAsync(file.Stream);

            if (!string.IsNullOrWhiteSpace(blockBlob.Uri.AbsoluteUri))
            {
                return blockBlob.Uri.AbsoluteUri;
            }
            else
            {
                return "";
            }
        }
    }
}
