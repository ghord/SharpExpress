using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress
{
    partial class RealLiteral
    {
        public static RealLiteral FromString(string text)
        {
            return new RealLiteral(double.Parse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent));
        }
    }
}
