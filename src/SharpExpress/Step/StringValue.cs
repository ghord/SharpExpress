using SharpExpress.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Step
{
    public class StringValue : StepValue<string>
    {
        public StringValue(string value) : base(StepValueKind.String, 
            value ?? throw new ArgumentNullException(nameof(value)))
        {
        }

        public StringValue(FastStringBuilder builder) : base(StepValueKind.String, 
            builder?.ToString() ?? throw new ArgumentNullException(nameof(builder)))
        {
           
        }

    }
}
