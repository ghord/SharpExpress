using System;

namespace SharpExpress.Step
{
    public class EntityValue : StepValue<int>
    {

        public EntityValue(int id) : base(StepValueKind.EntityRef, id)
        {

            
        }
    }
}
