using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress
{
    public class SyntaxList<T> : SyntaxNode, IReadOnlyList<T> where T : SyntaxNode
    {
        private T[] nodes_;

        internal SyntaxList(IEnumerable<T> nodes) : base(SyntaxNodeKind.SyntaxList)
        {
            nodes_ = nodes.ToArray();
        }

        public T this[int index] => nodes_[index];

        public override int SlotCount => nodes_.Length;

        public int Count => nodes_.Length;

        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitSyntaxList(this);
        }

        public override TResult Accept<TResult>(SyntaxNodeVisitor<TResult> visitor)
        {
            return visitor.VisitSyntaxList(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)nodes_).GetEnumerator();
        }

        public override SyntaxNode GetSlot(int slot)
        {
            return nodes_[slot];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return nodes_.GetEnumerator();
        }

        public static readonly SyntaxList<T> Empty = new SyntaxList<T>(Enumerable.Empty<T>());
    }

    public static class SyntaxList
    {
        public static SyntaxList<T> Create<T>(IEnumerable<T> nodes) where T : SyntaxNode
        {
            if (nodes.Any())
                return new SyntaxList<T>(nodes);
            else
                return SyntaxList<T>.Empty;
        }
    }
}
