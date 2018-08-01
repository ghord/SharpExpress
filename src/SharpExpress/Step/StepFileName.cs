using System.Collections.Generic;

namespace SharpExpress.Step
{
    public class StepFileName
    {
        [StepAttribute]
        public string Name { get; set; }

        [StepAttribute]
        public string TimestampText { get; set; }
        
        [StepAttribute]
        public IList<string> Author { get; set;  }

        [StepAttribute]
        public IList<string> Organization { get; set; }

        [StepAttribute]
        public string PreprocessorVersion { get; set; } 

        [StepAttribute]
        public string OriginatingSystem { get; set; }

        [StepAttribute]
        public string Authorization { get; set; }
    }

}
