namespace SharpExpress.Step
{
    public class TypedValue : StepValue<StepValue>
    {
        public TypedValue(string type, StepValue value) : base(StepValueKind.Typed, value)
        {
            Type = type;
        }

        public string Type { get; }
    }
}