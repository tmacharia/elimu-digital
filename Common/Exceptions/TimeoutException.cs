using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    /// <summary>
    /// Custom exception thrown when an operation takes longer than anticipated
    /// of type <see cref="Exception"/>
    /// </summary>
    public class TimeoutException : Exception
    {
        // Private Variables
        private readonly string _msg;

        /// <summary>
        /// Throws an exception/error when an operation takes more time to process
        /// than anticipated with a related exception message. 
        /// </summary>
        public TimeoutException()
        {
            _msg = "Processing this request took longer than expected.";
        }

        public override string Message => _msg;
    }
}
