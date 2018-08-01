using SharpExpress.Parsing;
using System;

namespace SharpExpress
{
    partial class FunctionReference 
    {
        public static FunctionReference Abs { get; } = new FunctionReference(Keywords.Abs);
        public static FunctionReference Acos { get; } = new FunctionReference(Keywords.Acos);
        public static FunctionReference Asin { get; } = new FunctionReference(Keywords.Asin);
        public static FunctionReference Atan { get; } = new FunctionReference(Keywords.Atan);
        public static FunctionReference Blength { get; } = new FunctionReference(Keywords.Blength);
        public static FunctionReference Cos { get; } = new FunctionReference(Keywords.Cos);
        public static FunctionReference Exists { get; } = new FunctionReference(Keywords.Exists);
        public static FunctionReference Exp { get; } = new FunctionReference(Keywords.Exp);
        public static FunctionReference Format { get; } = new FunctionReference(Keywords.Format);
        public static FunctionReference Hibound { get; } = new FunctionReference(Keywords.Hibound);
        public static FunctionReference Hiindex { get; } = new FunctionReference(Keywords.Hiindex);
        public static FunctionReference Length { get; } = new FunctionReference(Keywords.Length);
        public static FunctionReference Lobound { get; } = new FunctionReference(Keywords.Lobound);
        public static FunctionReference Log { get; } = new FunctionReference(Keywords.Log);
        public static FunctionReference Log2 { get; } = new FunctionReference(Keywords.Log2);
        public static FunctionReference Log10 { get; } = new FunctionReference(Keywords.Log10);
        public static FunctionReference Loindex { get; } = new FunctionReference(Keywords.Loindex);
        public static FunctionReference Nvl { get; } = new FunctionReference(Keywords.Nvl);
        public static FunctionReference Odd { get; } = new FunctionReference(Keywords.Odd);
        public static FunctionReference Rolesof { get; } = new FunctionReference(Keywords.Rolesof);
        public static FunctionReference Sin { get; } = new FunctionReference(Keywords.Sin);
        public static FunctionReference Sizeof { get; } = new FunctionReference(Keywords.Sizeof);
        public static FunctionReference Sqrt { get; } = new FunctionReference(Keywords.Sqrt);
        public static FunctionReference Tan { get; } = new FunctionReference(Keywords.Tan);
        public static FunctionReference Typeof { get; } = new FunctionReference(Keywords.Typeof);
        public static FunctionReference Usedin { get; } = new FunctionReference(Keywords.Usedin);
        public static FunctionReference Value { get; } = new FunctionReference(Keywords.Value);
        public static FunctionReference ValueIn { get; } = new FunctionReference(Keywords.ValueIn);
        public static FunctionReference ValueUnique { get; } = new FunctionReference(Keywords.ValueUnique);

    }
}
