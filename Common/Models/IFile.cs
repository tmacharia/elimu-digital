using DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.Models
{
    /// <summary>
    /// Represents a file uploaded by the user.
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// Fiel format based on the extension in the filename.
        /// </summary>
        FormatType Format { get; }
        /// <summary>
        /// Size of the file in the input stream in bytes.
        /// </summary>
        long Length { get; }
        /// <summary>
        /// Gets the name of the Content-Disposition header.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the file name from the Content-Disposition header.
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// Opens the request stream for reading the uploaded file
        /// </summary>
        Stream Stream { get; }
    }
}
