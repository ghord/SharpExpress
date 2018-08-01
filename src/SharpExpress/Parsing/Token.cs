using SharpExpress.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    [DebuggerDisplay("{Kind,nq}({Text,nq})")]
    class Token
    {
        public Token(string text, TokenKind kind, Span span = default(Span))
        {
            Kind = kind;
            Text = text;
            Span = span;
        }

        public static Token Eof { get; } = new Token(null, TokenKind.Eof);

        public static Token Exclamation { get; } = new Token("!", TokenKind.Exclamation);
        public static Token DoubleQuote { get; } = new Token("\"", TokenKind.DoubleQuote);
        public static Token Pound { get; } = new Token("#", TokenKind.Pound);
        public static Token Dollar { get; } = new Token("$", TokenKind.Dollar);
        public static Token Ampersand { get; } = new Token("&", TokenKind.Ampersand);
        public static Token Plus { get; } = new Token("+", TokenKind.Plus);
        public static Token Comma { get; } = new Token(",", TokenKind.Comma);
        public static Token Multiply { get; } = new Token("*", TokenKind.Multiply);
        public static Token Minus { get; } = new Token("-", TokenKind.Minus);
        public static Token Period { get; } = new Token(".", TokenKind.Period);
        public static Token Slash { get; } = new Token("/", TokenKind.Slash);
        public static Token Colon { get; } = new Token(":", TokenKind.Colon);
        public static Token Semicolon { get; } = new Token(";", TokenKind.Semicolon);
        public static Token LessThan { get; } = new Token("<", TokenKind.LessThan);
        public static Token Equal { get; } = new Token("=", TokenKind.Equal);
        public static Token GreaterThan { get; } = new Token(">", TokenKind.GreaterThan);
        public static Token QuestionMark { get; } = new Token("?", TokenKind.QuestionMark);
        public static Token At { get; } = new Token("@", TokenKind.At);
        public static Token LeftBracket { get; } = new Token("[", TokenKind.LeftBracket);
        public static Token Backslash { get; } = new Token("\\", TokenKind.Backslash);
        public static Token RightBracket { get; } = new Token("]", TokenKind.RightBracket);
        public static Token Caret { get; } = new Token("^", TokenKind.Caret);
        public static Token Underscore { get; } = new Token("_", TokenKind.Underscore);
        public static Token Backtick { get; } = new Token("`", TokenKind.Backtick);
        public static Token LeftBrace { get; } = new Token("{", TokenKind.LeftBrace);
        public static Token Pipe { get; } = new Token("|", TokenKind.Pipe);
        public static Token RightBrace { get; } = new Token("}", TokenKind.RightBrace);
        public static Token Tilde { get; } = new Token("~", TokenKind.Tilde);
        public static Token LeftParen { get; } = new Token("(", TokenKind.LeftParen);
        public static Token RightParen { get; } = new Token(")", TokenKind.RightParen);
        public static Token SingleQuote { get; } = new Token("'", TokenKind.SingleQuote);

        public static Token NotEqual { get; } = new Token("<>", TokenKind.NotEqual);
        public static Token Assignment { get; } = new Token(":=", TokenKind.Assignment);
        public static Token ComplexEntityConstruction { get; } = new Token("||", TokenKind.ComplexEntityConstruction);
        public static Token InstanceEqual { get; } = new Token(":=:", TokenKind.InstanceEqual);
        public static Token InstanceNotEqual { get; } = new Token(":<>:", TokenKind.InstanceNotEqual);
        public static Token GreaterThanOrEqual { get; } = new Token(">=", TokenKind.GreaterThanOrEqual);
        public static Token LessThanOrEqual { get; } = new Token("<=", TokenKind.LessThanOrEqual);
        public static Token Exponent { get; } = new Token("**", TokenKind.Exponent);
        public static Token Query { get; } = new Token("<*", TokenKind.Query);

        public string Text { get; }
        public TokenKind Kind { get; }
        public Span Span { get; }
   
        public Token WithSpan(Span span)
        {
            return new Token(Text, Kind, span);
        }

        public Token WithEndLocation(Location location, int? length = null)
        {
            var span = new Span(
                    new Location(location.Line,
                        location.Column - (length ?? Text.Length)),
                        location);

            return new Token(Text, Kind, span);
        }

        public static Token FromKeyword(string keyword, Span span = default)
        {
            if (keyword == null)
                throw new ArgumentNullException(nameof(keyword));

            return new Token(keyword.ToLower(), TokenKind.Keyword, span);
        }
    }
}
