using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.TypeSystem
{
    public sealed class RuleInfo : AlgorithmInfo
    {
        internal RuleInfo(string name, ISymbolInfo declaringSymbol) : base(name, declaringSymbol)
        {
           
        }
    }
}
