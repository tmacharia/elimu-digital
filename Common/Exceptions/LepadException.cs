using DAL.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    public class LepadException : Exception
    {
        public LepadException()
            : base()
        {
            
        }
        public LepadException(string message)
            : base(message)
        {

        }

        public LepadException(string message, Exception innerException)
            : base(message, innerException)
        {

        }


        #region Private Region
        private void LogError()
        {
            
        }
        #endregion
    }
}
