using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress
{
    public class SyntaxNodeWalker : SyntaxNodeVisitor
    {
        protected override void DefaultVisit(SyntaxNode node)
        {
            for (int i = 0; i < node.SlotCount; i++)
            {
                var child = node.GetSlot(i);

                if(child != null)
                    child.Accept(this);
            }
        }
    }
}
