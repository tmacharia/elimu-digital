using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    /// <summary>
    /// Types of file format allowed in E-Learning Pad.
    /// </summary>
    public enum FormatType
    {
        /// <summary>
        /// A file format for storing digital audio data on a computer system.
        /// e.g mp3, wav
        /// </summary>
        Audio,
        /// <summary>
        /// A file format for storing digital video data on a computer system.
        /// e.g mp4, vob
        /// </summary>
        Video,
        /// <summary>
        /// A document file format is a text or binary file format for storing
        /// documents on a storage media
        /// </summary>
        Document,
        /// <summary>
        /// File format not recognized or supported by application.
        /// </summary>
        Unknown
    }
}
