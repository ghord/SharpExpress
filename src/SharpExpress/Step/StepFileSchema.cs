using System.Collections.Generic;

namespace SharpExpress.Step
{
    public class StepFileSchema
    {
        [StepAttribute]
        public IList<string> SchemaIdentifiers { get; set; }
    }

}
