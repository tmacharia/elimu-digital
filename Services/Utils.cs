using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public static class Utils
    {
        public static byte[] ToByteArray(object obj)
        {
            if(obj == null)
            {
                return new byte[0];
            }

            IEnumerable en = (IEnumerable)obj;

            return en.OfType<byte>().ToArray();
        }
    }
}
