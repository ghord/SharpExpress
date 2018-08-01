using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.TypeSystem
{
    public class ParameterInfo : ISymbolInfo
    {
        internal ParameterInfo(string name, ISymbolInfo declaringSymbol)
        {
            Name = name;
            DeclaringSymbol = declaringSymbol;
        }

        public string Name { get; }

        public ISymbolInfo DeclaringSymbol { get; }

        public TypeInfo Type { get; internal set; }
    }
}
