using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress
{
    public abstract class SyntaxNode
    {
        public SyntaxNode(SyntaxNodeKind kind)
        {
            Kind = kind;
        }

        public SyntaxNodeKind Kind { get; }

        public abstract int SlotCount { get; }

        public abstract SyntaxNode GetSlot(int slot);

        public abstract void Accept(SyntaxNodeVisitor visitor);

        public abstract TResult Accept<TResult>(SyntaxNodeVisitor<TResult> visitor);
    }
}
