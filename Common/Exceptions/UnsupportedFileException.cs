using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a file that is not compatible with the
    /// application is uploaded by a user.
    /// </summary>
    public class UnSupportedFileException : Exception
    {
        private readonly string _msg;

        /// <summary>
        /// Throws an exception with a relevant error message when the user
        /// tries to upload a file that is not supported by the application.
        /// </summary>
        /// <param name="fileName">Full name of the file including its extension.</param>
        public UnSupportedFileException(string fileName)
        {
            _msg = $"File '{fileName}' is not supported by the application at the moment.";
            _msg += "Try uploading a file of different type.";
        }

        public override string Message => _msg;
    }
}
