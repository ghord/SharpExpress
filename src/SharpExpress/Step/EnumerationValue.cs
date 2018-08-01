using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Step
{
    public class EnumerationValue : StepValue<string>
    {
        public EnumerationValue(string value) : base(StepValueKind.Enumeration, value)
        {
        }
    }
}
