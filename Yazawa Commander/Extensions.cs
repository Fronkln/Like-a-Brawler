using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yazawa_Commander
{
    internal static class Extensions
    {
        public static string SplitByCase(this string str)
        {
            StringBuilder SB = new StringBuilder();

            foreach (Char c in str)
            {
                if (Char.IsUpper(c))
                    SB.Append(' ');
                SB.Append(c);
            }

            return SB.ToString();

        }
    }
}
