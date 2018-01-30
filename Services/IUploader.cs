using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Services
{
    /// <summary>
    /// Service for uploading any kind of files to Azure <see cref="CloudBlob"/>
    /// including: Videos, Audios and documents to specific <see cref="CloudBlobContainer"/>
    /// for that file type.
    /// </summary>
    public interface IUploader
    {
        Task<string> Upload(IFile file);
    }
}
