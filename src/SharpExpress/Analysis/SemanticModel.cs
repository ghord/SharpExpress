using SharpExpress.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Analysis
{
    public class SemanticModel
    {
        private SymbolTable symbolTable_;

        internal SemanticModel(SymbolTable symbolTable)
        {
            symbolTable_ = symbolTable;
        }

        public TypeInfo GetTypeInfo(TypeReference reference)
        {
            throw new NotImplementedException();
        }

        public TypeInfo GetTypeInfo(EntityReference reference)
        {
            throw new NotImplementedException();
        }

        public TypeInfo GetTypeInfo(TypeDeclaration typeDeclaration)
        {
            return symbolTable_.GetSymbolInfo(typeDeclaration) as TypeInfo;
        }

        public TypeInfo GetTypeInfo(EntityDeclaration entityDeclaration)
        {
            return symbolTable_.GetSymbolInfo(entityDeclaration) as TypeInfo;
        }

        public TypeInfo GetTypeInfo(Expression expression)
        {
            throw new NotImplementedException();
        }

        public SchemaInfo GetSchemaInfo(SchemaDeclaration schemaDeclaration)
        {
            return symbolTable_.GetSymbolInfo(schemaDeclaration) as SchemaInfo;
        }

        public AttributeInfo GetAttributeInfo(AttributeReference reference)
        {
            throw new NotImplementedException();
        }

        public Declaration GetDeclaration(ISymbolInfo symbolInfo)
        {
            throw new NotImplementedException();
        }

        public bool TryGetConstantValue(Expression expression, out object value)
        {
            throw new NotImplementedException();
        }

        public static SemanticModel GetSemanticModel(IEnumerable<SchemaDeclaration> schemas)
        {
            var builder = new SemanticModelBuilder(schemas);
            return builder.Build();
        }
    }
}
