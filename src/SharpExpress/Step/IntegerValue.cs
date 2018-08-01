namespace SharpExpress.Step
{
    public class IntegerValue : StepValue<int>
    {
        public IntegerValue(int value) : base(StepValueKind.Integer, value)
        {
        }
    }
}