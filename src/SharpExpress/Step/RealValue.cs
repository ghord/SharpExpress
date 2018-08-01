namespace SharpExpress.Step
{
    public class RealValue : StepValue<float>
    {
        public RealValue(float value) : base(StepValueKind.Real, value)
        {
        }
    }
}