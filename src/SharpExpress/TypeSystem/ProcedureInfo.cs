using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.TypeSystem
{
    public sealed class ProcedureInfo : AlgorithmInfo
    {
        internal ProcedureInfo(string name, ISymbolInfo declaringSymbol) : base(name, declaringSymbol)
        {
        }
    }
}
