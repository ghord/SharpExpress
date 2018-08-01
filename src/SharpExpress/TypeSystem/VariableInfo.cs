using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.TypeSystem
{
    public class VariableInfo : ISymbolInfo
    {
        public VariableInfo(string name, ISymbolInfo declaringSymbol)
        {
            Name = name;
            DeclaringSymbol = declaringSymbol;
        }

        public string Name { get; }

        public TypeInfo Type { get; internal set; }

        public ISymbolInfo DeclaringSymbol { get; }
    }
}
