using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress
{
    partial class IntegerLiteral
    {
        public static IntegerLiteral FromString(string text)
        {
            return new IntegerLiteral(int.Parse(text, NumberStyles.None));
        }
    }
}
