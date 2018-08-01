using System.Collections.Generic;

namespace SharpExpress.Step
{
    public class StepFilePopulation
    {
        [StepAttribute]
        public string GoverningSchema { get; set; }

        [StepAttribute]
        public string DeterminationMethod { get; set; }

        [StepAttribute]
        public IList<string> GovernedSections { get; set; }
    }

}
