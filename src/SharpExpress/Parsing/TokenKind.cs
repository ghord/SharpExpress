using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    /// <summary>
    /// List of tokens 
    /// </summary>
    enum TokenKind
    { 
        //literals
        BinaryLiteral,
        EncodedStringLiteral,
        SimpleStringLiteral,
        IntegerLiteral,
        RealLiteral,
        SimpleId,
        Keyword,

        //sybmols
        Semicolon,
        Period,
        Exclamation,
        DoubleQuote,
        Pound,
        Dollar,
        Percent,
        Ampersand,
        Plus,
        Minus,
        Slash,
        Colon,
        LessThan,
        Equal,
        GreaterThan,
        QuestionMark,
        At,
        LeftBracket,
        Backslash,
        RightBracket,
        Caret,
        Underscore,
        Backtick,
        LeftBrace,
        Pipe,
        RightBrace,
        Tilde,
        LeftParen,
        RightParen,
        SingleQuote,
        Comma,
        Multiply,

        //composite operators
        NotEqual,
        GreaterThanOrEqual,
        LessThanOrEqual,
        InstanceEqual,
        InstanceNotEqual,
        Assignment,
        Query,
        ComplexEntityConstruction,
        Exponent,

        //eof
        Eof,
    }
}
