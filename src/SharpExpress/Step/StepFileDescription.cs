using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Step
{
    public class StepFileDescription 
    {
        [StepAttribute]
        public IList<string> Description { get; set; } 

        [StepAttribute]
        public string ImplementationLevel { get; set; }
    }

}
