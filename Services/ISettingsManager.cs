using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface ISettingsManager
    {
        /// <summary>
        /// Notify you by email everytime an exception is thrown by system immediately
        /// as it happens.
        /// </summary>
        bool SendEmailOnError { get; }
        /// <summary>
        /// Save error messages either in the server machine or in the database for every
        /// exception the system throws.
        /// </summary>
        bool LogError { get; }
        /// <summary>
        /// Developer email setting to use when sending developer related emails by
        /// the system.
        /// </summary>
        string DevEmail { get; }
    }
}
