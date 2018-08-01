using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress
{
    partial class SyntaxNodeVisitor
    {
        public virtual void VisitSyntaxList(SyntaxNode node)
        {
            DefaultVisit(node);
        }

        protected virtual void DefaultVisit(SyntaxNode node)
        {

        }
    }

    partial class SyntaxNodeVisitor<TResult>
    {
        public virtual TResult VisitSyntaxList(SyntaxNode node)
        {
            return DefaultVisit(node);
        }

        protected virtual TResult DefaultVisit(SyntaxNode node)
        {
            return default;
        }
    }
}
