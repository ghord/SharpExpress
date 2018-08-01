using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.TypeSystem
{
    public class EnumerationInfo : ISymbolInfo
    {
        internal EnumerationInfo(string name, TypeInfo declaringType)
        {
            DeclaringType = declaringType;
            Name = name;
        }

        public string Name { get; }

        public TypeInfo DeclaringType { get; }

        

        public ISymbolInfo DeclaringSymbol => DeclaringType;
    }
}
