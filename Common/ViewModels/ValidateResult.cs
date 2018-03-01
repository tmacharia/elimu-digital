using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class ValidateResult
    {
        public bool IsValid
        {
            get
            {
                if(this.Error == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public Exception Error { get; set; }
    }
}
