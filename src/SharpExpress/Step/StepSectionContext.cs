using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Step
{
    public class StepSectionContext
    {
        public string Section { get; set; }

        public List<string> ContextName { get; } = new List<string>();
    }
}
