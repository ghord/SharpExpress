using SharpExpress.Parsing;
using System;

namespace SharpExpress
{
    partial class ProcedureReference
    {
        public static ProcedureReference Insert { get; } = new ProcedureReference(Keywords.Insert);
        public static ProcedureReference Remove { get; } = new ProcedureReference(Keywords.Remove);
    }
}
