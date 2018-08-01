using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Step
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class StepAttributeAttribute : Attribute
    {
        public StepAttributeAttribute([CallerLineNumber] int order = 0)
        {
            Order = order;
        }

        public int Order { get; }
    }
}
