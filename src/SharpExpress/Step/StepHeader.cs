using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Step
{
    public class StepHeader<TEntity>
    {
        public StepFileDescription FileDescription { get; set; }
        public StepFileName FileName { get; set; }
        public StepFileSchema FileSchema { get; set; }
        public List<StepFilePopulation> FilePopulations { get; } = new List<StepFilePopulation>();
        public Dictionary<string, StepSectionLanguage> SectionLanguages { get; }
            = new Dictionary<string, StepSectionLanguage>(StringComparer.OrdinalIgnoreCase);
        public StepSectionLanguage DefaultSectionLanguage { get; set; }
        public Dictionary<string, StepSectionContext> SectionContexts { get; }
            = new Dictionary<string, StepSectionContext>(StringComparer.OrdinalIgnoreCase);
        public StepSectionContext DefaultSectionContext { get; set; }
        public List<TEntity> CustomEntities { get; } = new List<TEntity>();
    }
}
