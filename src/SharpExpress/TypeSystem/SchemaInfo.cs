using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.TypeSystem
{
    public class SchemaInfo : ISymbolInfo, IDeclaringSymbolInfo
    {
        private List<ISymbolInfo> declarations_ = new List<ISymbolInfo>();

        internal SchemaInfo(string name)
        {
            Name = name;
        }

        void IDeclaringSymbolInfo.AddDeclaration(ISymbolInfo declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            if (declaration.DeclaringSymbol != this)
                throw new InvalidOperationException();

            declarations_.Add(declaration);
        }

        public string Name { get; }
        public IReadOnlyList<ISymbolInfo> Declarations => declarations_;

        public ISymbolInfo DeclaringSymbol => null;

    }
}
