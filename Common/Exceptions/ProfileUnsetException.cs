using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    public class ProfileUnsetException : Exception
    {
        // Private Variables
        private readonly string _msg;

        /// <summary>
        /// Throws when a user tries to perform an operation that requires their profile
        /// set up.
        /// </summary>
        public ProfileUnsetException()
        {
            _msg = "Can only perform this operation when account profile is set. Redirect back to home and create a profile.";
        }

        public override string Message => _msg;
    }
}
