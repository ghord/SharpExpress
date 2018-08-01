using SharpExpress.Builders;
using SharpExpress.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    abstract class ParserPass
    {
        private Stack<ErrorRecoveryScope> recoveryScopes_;
        private IList<ParsingError> errors_;
        private ArrayPeekEnumerator<Token> enumerator_;

        protected ArrayPeekEnumerator<Token> Enumerator => enumerator_;

        protected IList<ParsingError> GetErrorList() => errors_;

        protected ParserPass(Token[] tokens, IList<ParsingError> errors)
        {
            errors_ = errors;
            recoveryScopes_ = new Stack<ErrorRecoveryScope>();
            enumerator_ = new ArrayPeekEnumerator<Token>(tokens);
        }

        protected Token CurrentToken => enumerator_.Current;

        public abstract void Run(SyntaxTreeBuilder builder);

        protected void EmitError(string message)
        {
            errors_?.Add(new ParsingError(ErrorSource.Parser, message, CurrentToken.Span));
        }

        protected void Recover(Action code, TokenKind syncTokenKind, string syncTokenText = null)
        {
            recoveryScopes_.Push(new ErrorRecoveryScope(syncTokenKind, syncTokenText));

            try
            {
                code();
            }
            catch (ErrorRecoveryException e) when (e.Scope == recoveryScopes_.Peek())
            {

            }
            catch (NotImplementedException e)
            {
                EmitError(string.Format(Errors.NotImplemented, e.Message));

                while (enumerator_.TryPeek(out var token))
                {
                    if (CanRecoverInParentScope(token.Kind, token.Text, out var scope))
                    {
                        if (scope == recoveryScopes_.Peek())
                        {
                            return;
                        }
                        else
                        {
                            throw new ErrorRecoveryException(scope);
                        }
                    }

                    enumerator_.MoveNext();
                }

                throw new ParsingException(Errors.UnexpectedEndOfFile, Span.Empty);
            }
            finally
            {
                recoveryScopes_.Pop();
            }
        }

        protected void Expect(TokenKind kind, string text = null)
        {
            bool emitError = true;

            while (enumerator_.TryPeek(out var next))
            {
                if (kind == next.Kind && (text == null || text.Equals(next.Text, StringComparison.OrdinalIgnoreCase)))
                {
                    enumerator_.MoveNext();
                    return;
                }

                if (emitError)
                {
                    if (text != null && kind == next.Kind)
                        EmitError(string.Format(Errors.UnexpectedTokenText, text, next.Text));
                    else
                        EmitError(string.Format(Errors.UnexpectedTokenKind,
                            text != null ? $"{kind}('{text}')" : kind.ToString(),
                            $"{next.Kind}('{next.Text}')"));

                    emitError = false;
                }

                if (CanRecoverInParentScope(next.Kind, next.Text, out var scope))
                {
                    throw new ErrorRecoveryException(scope);
                }

                enumerator_.MoveNext();
            }

            throw new ParsingException(Errors.UnexpectedEndOfFile, CurrentToken.Span);
        }

        private bool CanRecoverInParentScope(TokenKind kind, string text, out ErrorRecoveryScope recoveryScope)
        {
            foreach (var scope in recoveryScopes_)
            {
                if (scope.CanRecover(kind, text))
                {
                    recoveryScope = scope;
                    return true;
                }
            }

            recoveryScope = null;
            return false;
        }

        protected bool Accept(TokenKind kind, string text = null)
        {
            if (enumerator_.TryPeek(out var token))
            {
                if (token.Kind == kind)
                {
                    if (text == null || text.Equals(token.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        enumerator_.MoveNext();
                        return true;
                    }
                }
            }

            return false;
        }


        protected bool AcceptRelationalOperatorExtended()
        {
            if(Enumerator.TryPeek(out var token))
            {
                switch (token.Kind)
                {
                    case TokenKind.LessThan:
                    case TokenKind.GreaterThan:
                    case TokenKind.LessThanOrEqual:
                    case TokenKind.GreaterThanOrEqual:
                    case TokenKind.NotEqual:
                    case TokenKind.Equal:
                    case TokenKind.InstanceNotEqual:
                    case TokenKind.InstanceEqual:
                    case TokenKind.Keyword when
                        token.Text.Equals(Keywords.Like, StringComparison.OrdinalIgnoreCase) || 
                        token.Text.Equals(Keywords.In, StringComparison.OrdinalIgnoreCase):

                        Enumerator.MoveNext();
                        return true;
                }
            }

            return false;
        }

        protected bool AcceptAddLikeOperator()
        {
            if(Enumerator.TryPeek(out var token))
            {
                switch (token.Kind)
                {
                    case TokenKind.Plus:
                    case TokenKind.Minus:
                    case TokenKind.Keyword when
                        token.Text.Equals(Keywords.Or, StringComparison.OrdinalIgnoreCase) ||
                        token.Text.Equals(Keywords.Xor, StringComparison.OrdinalIgnoreCase):

                        Enumerator.MoveNext();
                        return true;
                }
            }

            return false;
      
        }

        protected bool AcceptMultiplicationLikeOperator()
        {
            return Accept(TokenKind.Multiply)
                || Accept(TokenKind.Slash)
                || Accept(TokenKind.Keyword, Keywords.Div)
                || Accept(TokenKind.Keyword, Keywords.Mod)
                || Accept(TokenKind.Keyword, Keywords.And)
                || Accept(TokenKind.ComplexEntityConstruction);
        }

        protected bool AcceptUnaryOpeartor()
        {
            return Accept(TokenKind.Plus)
                || Accept(TokenKind.Minus)
                || Accept(TokenKind.Keyword, Keywords.Not);
        }

    }
}
