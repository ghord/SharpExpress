using SharpExpress.Parsing;
using System;

namespace SharpExpress
{
    partial class ConstantReference
    {
        public static ConstantReference ConstE { get; } = new ConstantReference(Keywords.ConstE);
        public static ConstantReference Indeterminate { get; } = new ConstantReference("?");
        public static ConstantReference False { get; } = new ConstantReference(Keywords.False);
        public static ConstantReference Pi { get; } = new ConstantReference(Keywords.Pi);
        public static ConstantReference Self { get; } = new ConstantReference(Keywords.Self);
        public static ConstantReference True { get; } = new ConstantReference(Keywords.True);
        public static ConstantReference Unknown { get; } = new ConstantReference(Keywords.Unknown);
    }
}
