using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Step
{
    public class StepEntity
    {
        public StepEntity(string entityType, IReadOnlyList<StepValue> attributeValues)
        {
            EntityType = entityType;
            AttributeValues = attributeValues;
        }

        public string EntityType { get; }
        public IReadOnlyList<StepValue> AttributeValues { get; }

        public T ToObject<T>() where T : class, new()
        {
            var result = new T();
            StepConvert.PopulateObject(result, AttributeValues);
            return result;
        }
    }
}
