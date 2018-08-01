using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Test.Utilities
{
    public class DeclarationSyntaxWalker : SyntaxNodeWalker
    {
        private List<Declaration> declarations_ = new List<Declaration>();

        protected override void DefaultVisit(SyntaxNode node)
        {
            if (node is Declaration declaration)
                declarations_.Add(declaration);

            base.DefaultVisit(node);
        }

        public IReadOnlyList<Declaration> Declarations => declarations_;
    }
}
