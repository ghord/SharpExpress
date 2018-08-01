using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Builders
{
    abstract class SyntaxNodeBuilder<TNode> where TNode: SyntaxNode
    {
        public abstract TNode Build();
    }
}
