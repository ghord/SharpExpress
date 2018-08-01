using SharpExpress.Parsing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace SharpExpress.Builders
{
    class SyntaxTreeBuilder 
    {
        public List<SchemaDeclarationBuilder> Schemas { get; } = new List<SchemaDeclarationBuilder>();

        public ImmutableArray<SchemaDeclaration> Build()
        {
            return Schemas.Select(s => s.Build()).Cast<SchemaDeclaration>().ToImmutableArray();
        }
    }
}
