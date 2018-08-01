using SharpExpress.Analysis;
using System;
using System.Collections.Generic;

namespace SharpExpress.TypeSystem
{
    class SymbolTable
    {
        private IReadOnlyDictionary<Declaration, ISymbolInfo> symbols_;

        public SymbolTable(IReadOnlyDictionary<Declaration, ISymbolInfo> symbols)
        {
            symbols_ = symbols;
        }

        public ISymbolInfo GetSymbolInfo(Declaration declaration)
        {
            return symbols_.TryGetValue(declaration, out var result) ? result : null;
        }

        public TypeInfo GetType(TypeDeclaration typeDeclaration)
        {
            throw new NotImplementedException();
        }

        public EntityInfo GetEntity(EntityDeclaration entityDeclaration)
        {
            throw new NotImplementedException();
        }

        public AttributeInfo GetAttribute(AttributeDeclaration attributeDeclaration)
        {
            throw new NotImplementedException();
        }
    }
}
