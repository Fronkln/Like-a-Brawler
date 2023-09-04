using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    internal static class Utils
    {
        public static string ToLength(this string self, int length)
        {
            if (self == null)
                return null;

            if (self.Length == length)
                return self;

            if (self.Length > length)
                return self.Substring(0, length);


            StringBuilder str = new StringBuilder();
            str.Append(self);

            while (str.Length != length)
                str.Append('\0');

            return str.ToString();
        }
    }
}
