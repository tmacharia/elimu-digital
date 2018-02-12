using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.Exceptions;
using DAL.Models;

namespace Common.Models
{
    public class FormFile : IFile
    {
        #region Private Readonly variables
        private readonly string[] _videoTypes;
        private readonly string[] _audioTypes;
        private readonly string[] _docTypes;
        private readonly string[] _imageTypes;

        private readonly string _mime;
        private readonly long _length;
        private readonly string _name;
        private readonly string _fileName;
        private readonly FormatType _format;
        private readonly Stream _stream;
        #endregion

        /// <summary>
        /// Initializes a concrete implementation of <see cref="IFile"/> with all
        /// properties from an uploaded file from a user.
        /// </summary>
        /// <param name="file">File uploaded by user in the Form Request body.</param>
        /// <exception cref="UnSupportedFileException"></exception>
        public FormFile(dynamic file)
        {
            _mime = file.ContentType;
            _length = file.Length;
            _name = file.Name;
            _fileName = file.FileName;
            _stream = file.OpenReadStream();

            // Initialize supported mime types
            _videoTypes = new string[] { "video/x-flv", "video/mp4", "video/3gpp", "application/x-mpegURL", "video/MP2T", "video/quicktime", "video/x-msvideo", "video/x-ms-wmv" };
            _audioTypes = new string[] { "audio/basic", "auido/L24", "audio/mid", "audio/mpeg", "audio/mp4", "audio/x-aiff", "audio/x-mpegurl", "audio/vnd.rn-realaudio", "audio/vnd.rn-realaudio", "audio/ogg", "audio/vorbis", "audio/vnd.wav" };
            _docTypes = new string[] { "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation", "application/vnd.ms-access" };
            _imageTypes = new string[] { "image/gif", "image/jpeg", "image/png" };

            // Get file format
            _format = GetFileFormat();
        }
        public FormFile()
        {

        }
        public FormatType Format => _format;

        private FormatType GetFileFormat()
        {
            if(_videoTypes.Contains(_mime))
            {
                return FormatType.Video;
            }
            else if (_audioTypes.Contains(_mime))
            {
                return FormatType.Audio;
            }
            else if(_docTypes.Contains(_mime))
            {
                return FormatType.Document;
            }
            else if (_imageTypes.Contains(_mime))
            {
                return FormatType.Image;
            }
            else
            {
                throw new UnSupportedFileException(_fileName);
            }
        }

        public long Length => _length;

        public string Name => _name;

        public string FileName => _fileName;

        public Stream Stream => _stream;
    }
}
