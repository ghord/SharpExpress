using System;

namespace SharpExpress.Step
{
    public enum StepValueKind
    {
        Integer,
        String,
        Logical,
        Real,
        Binary,
        List,
        Typed,
        Enumeration,
        Select,
        EntityRef,
        Undefined,
        Missing,
        Derived
    }

    public class StepValue
    {
        protected StepValue(StepValueKind kind)
        {
            Kind = kind;
        }

        public StepValueKind Kind { get; }

        public override string ToString()
        {
            switch (Kind)
            {
                case StepValueKind.Missing: return "$";
                case StepValueKind.Derived: return "*";
               
                default: return Kind.ToString();
            }
        }

        public static StepValue Missing { get; } = new StepValue(StepValueKind.Missing);
        public static StepValue Undefined { get; } = new StepValue(StepValueKind.Undefined);
        public static StepValue Derived { get; } = new StepValue(StepValueKind.Derived);
    }

    public abstract class StepValue<T> : StepValue
    {
        public StepValue(StepValueKind kind, T value) : base(kind)
        {
            Value = value;
        }

        public override string ToString() => $"{Kind}({Value})";

        public T Value { get; }
    }
}
