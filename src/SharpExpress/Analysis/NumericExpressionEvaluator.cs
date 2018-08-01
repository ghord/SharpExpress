using SharpExpress.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Analysis
{
    public class NumericExpressionEvaluator : SyntaxNodeVisitor<int?>
    {
        public int? Evaluate(Expression expression)
        {
            return expression.Accept(this);
        }

        public int EvaluateNonIndeterminate(Expression expression)
        {
            var result = expression.Accept(this);

            if (result.HasValue)
                return result.Value;

            //non ideterminate expression evaluated to null
            throw new NotImplementedException();
        }

        protected override int? DefaultVisit(SyntaxNode node)
        {
            throw new NotSupportedException(node.Kind.ToString());
        }

        public override int? VisitIntegerLiteral(IntegerLiteral integerLiteral)
        {
            return integerLiteral.Value;
        }

        public override int? VisitUnaryMinusExpression(UnaryMinusExpression unaryMinusExpression)
        {
            return -unaryMinusExpression.Operand.Accept(this);
        }

        public override int? VisitUnaryPlusExpression(UnaryPlusExpression unaryPlusExpression)
        {
            return unaryPlusExpression.Operand.Accept(this);
        }

        public override int? VisitConstantReferenceExpression(ConstantReferenceExpression constantReferenceExpression)
        {
            if(Keywords.IsBuiltInConstant(constantReferenceExpression.Constant.ConstantName))
            {
                switch (constantReferenceExpression.Constant.ConstantName)
                {
                    case "?":
                        return null;

                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
