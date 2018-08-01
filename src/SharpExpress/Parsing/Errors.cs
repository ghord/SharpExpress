using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    public static class Errors
    {
        public const string TokenizerUnrecognizedCharacter = "Unrecognized character {0}";
        public const string TokenizerIntegerLiteralIsTooLong = "Integer constant is too long";
        public const string TokenizerUnexpectedNewlineInStringLiteral = "Newline in string literal is not allowed";
        public const string TokenizerMissingExponent = "Missing exponent in real literal";
        public const string TokenizerRealLiteralOutOfRange = "Real literal is out of range";
        public const string TokenizerInvalidBinaryLiteral = "Invalid binary literal";
        public const string TokenizerUnbalancedOctets = "Octets must be supplied in groups of four";
        public const string TokenizerOnlyHex = "Only hexadecimal characters are allowed between \"\"";

        public const string ParserIdentifierAlreadyDeclared = "Identifier {0} is already declared in this scope";
        public const string ParserMissingUnderlyingType = "Missing underlying type for type declaration";
        public const string ParserMissingElementType = "Missing element type for aggregation type";
        

        public const string UnexpectedEndOfFile = "Unexpected end of file";
        public const string UnexpectedTokenKind = "Expected token of kind {0}, found {1} instead";
        public const string UnexpectedTokenText = "Expected token {0}, found {1} instead";
        public const string ExpectedDataType = "Expected data type";
        public const string NotImplemented = "{0} is not implemented";
        public const string MissingRuleOrDeclaration = "Missing rule or declaration";

        public const string ParserArrayRequiresBoundsSpecification = "Bounds specification are required for arrays";
    }
}
