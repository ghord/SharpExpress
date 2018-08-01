using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.TypeSystem
{
    public class AttributeInfo : ISymbolInfo
    {
        internal AttributeInfo(string name, ISymbolInfo declaringSymbol)
        {
            Name = name;
            DeclaringType = (TypeInfo)declaringSymbol;
        }

        public string Name { get; }
        public TypeInfo Type { get; internal set; }

        public TypeInfo DeclaringType { get; }

        ISymbolInfo ISymbolInfo.DeclaringSymbol { get; }
    }
}
