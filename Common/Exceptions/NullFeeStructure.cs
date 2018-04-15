using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    public class NullFeeStructure : LepadException
    {
        public NullFeeStructure()
            :base("The fee structure you requested is not ready yet. Please come back soon.")
        {
            
        }
    }
}
