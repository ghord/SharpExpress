using System.Collections.Generic;
using System.Linq;

namespace SharpExpress.Step
{
    public class ListValue : StepValue
    {
        public ListValue(IReadOnlyList<StepValue> items) : base(StepValueKind.List)
        {
            Items = items;
        }

        public IReadOnlyList<StepValue> Items { get; }
    }
}
