using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    static class Tokenizer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsLetter(int character)
        {
            return (character >= 'a' && character <= 'z') || (character >= 'A' && character <= 'Z');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsLetterOrDigitOrUnderscore(int character)
        {
            return IsLetter(character) || IsDigit(character) || character == '_';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDigit(int character)
        {
            return character >= '0' && character <= '9';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsWhiteSpace(int character)
        {
            return character == '\n' || character == '\t' || character == ' ' || character == '\r';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsBit(int character)
        {
            return character == '1' || character == '0';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsHexDigit(int character)
        {
            return IsDigit(character) || (character >= 'a' && character <= 'f') || (character >= 'A' && character <= 'F');
        }

        private static Span CreateSpan(Location endLocation, int length)
        {
            return new Span(new Location(endLocation.Line, endLocation.Column - length), endLocation);
        }

        public static IEnumerable<Token> Tokenize(TextReader textReader, IList<ParsingError> errors = null)
        {
            var reader = new LocationTextReader(textReader);
            void emitError(string message)
            {
                errors?.Add(new ParsingError(ErrorSource.Tokenizer, message,
                    new Span(reader.Location, new Location(reader.Location.Line, reader.Location.Column + 1))));
            }

            var buffer = new StringBuilder();
            var encoding = new UTF32Encoding(true, false, true);

            int current;

            while ((current = reader.Read()) != -1)
            {
                var ch = (char)current;

                switch (ch)
                {
                    case '!': yield return Token.Exclamation.WithEndLocation(reader.Location); break;
                    case '"':
                        {
                            buffer.Clear();

                            while ((current = reader.Read()) != -1)
                            {
                                if (IsHexDigit(current))
                                    buffer.Append((char)current);
                                else if (current == '"')
                                {
                                    if (buffer.Length % 8 != 0)
                                    {
                                        emitError(Errors.TokenizerUnbalancedOctets);
                                        break;
                                    }
                                    else
                                    {
                                        var result = encoding.GetString(SoapHexBinary.Parse(buffer.ToString()).Value);
                                        yield return new Token(result,
                                            TokenKind.EncodedStringLiteral, CreateSpan(reader.Location, buffer.Length + 2));
                                        break;
                                    }
                                }
                                else
                                {
                                    emitError(Errors.TokenizerOnlyHex);
                                    break;
                                }
                            }

                            break;
                        }
                    case '#': yield return Token.Pound.WithEndLocation(reader.Location); break;
                    case '$': yield return Token.Dollar.WithEndLocation(reader.Location); break;
                    case '%':
                        {
                            buffer.Clear();

                            var next = reader.Peek();

                            if (next == '1' || next == '0')
                            {
                                while (IsBit(reader.Peek()))
                                {
                                    buffer.Append((char)reader.Read());
                                }

                                yield return new Token(buffer.ToString(), TokenKind.BinaryLiteral,
                                    CreateSpan(reader.Location, buffer.Length));
                            }
                            else
                            {
                                emitError(Errors.TokenizerInvalidBinaryLiteral);
                            }

                            break;
                        }
                    case '&': yield return Token.Ampersand.WithEndLocation(reader.Location); break;
                    case '*':
                        {
                            if (reader.Peek() == '*')
                            {
                                reader.Read();
                                yield return Token.Exponent.WithEndLocation(reader.Location);
                            }
                            else

                                yield return Token.Multiply.WithEndLocation(reader.Location);

                            break;
                        }

                    case '+': yield return Token.Plus.WithEndLocation(reader.Location); break;
                    case ',': yield return Token.Comma.WithEndLocation(reader.Location); break;
                    case '-':
                        {
                            if (reader.Peek() == '-')
                            {
                                while ((current = reader.Read()) != -1)
                                {
                                    if (current == '\n')
                                        break;
                                }
                            }
                            else
                                yield return Token.Minus.WithEndLocation(reader.Location); break;
                        }
                    case '.': yield return Token.Period.WithEndLocation(reader.Location); break;
                    case '/': yield return Token.Slash.WithEndLocation(reader.Location); break;
                    case ':':
                        {
                            var next = reader.Peek();
                            if (next == '=')
                            {
                                reader.Read();

                                if (reader.Peek() == ':')
                                {
                                    reader.Read();
                                    yield return Token.InstanceEqual.WithEndLocation(reader.Location);
                                }
                                else
                                {
                                    yield return Token.Assignment.WithEndLocation(reader.Location);
                                }
                            }
                            else if (next == '<')
                            {
                                reader.Read();

                                if (reader.Peek() == '>')
                                {
                                    reader.Read();

                                    if (reader.Peek() == ':')
                                    {
                                        reader.Read();
                                        yield return Token.InstanceNotEqual.WithEndLocation(reader.Location);
                                    }
                                    else
                                    {
                                        yield return Token.Colon.WithEndLocation(new Location(reader.Location.Line, reader.Location.Column - 2));
                                        yield return Token.NotEqual.WithEndLocation(reader.Location);
                                    }
                                }
                                else
                                {
                                    yield return Token.Colon.WithEndLocation(new Location(reader.Location.Line, reader.Location.Column - 1));
                                    yield return Token.LessThan.WithEndLocation(reader.Location);
                                }
                            }
                            else
                                yield return Token.Colon.WithEndLocation(reader.Location);

                            break;
                        }
                    case ';': yield return Token.Semicolon.WithEndLocation(reader.Location); break;
                    case '<':
                        {
                            var next = reader.Peek();

                            if (next == '=')
                            {
                                reader.Read();
                                yield return Token.LessThanOrEqual.WithEndLocation(reader.Location);
                            }
                            else if (next == '>')
                            {
                                reader.Read();
                                yield return Token.NotEqual.WithEndLocation(reader.Location);
                            }
                            else if (next == '*')
                            {
                                reader.Read();
                                yield return Token.Query.WithEndLocation(reader.Location);
                            }
                            else
                                yield return Token.LessThan.WithEndLocation(reader.Location);

                            break;
                        }
                    case '=': yield return Token.Equal.WithEndLocation(reader.Location); break;
                    case '>':
                        {
                            if (reader.Peek() == '=')
                            {
                                reader.Read();
                                yield return Token.GreaterThanOrEqual.WithEndLocation(reader.Location);
                            }
                            else
                            {
                                yield return Token.GreaterThan.WithEndLocation(reader.Location);
                            }

                            break;
                        }
                    case '?': yield return Token.QuestionMark.WithEndLocation(reader.Location); break;
                    case '@': yield return Token.At.WithEndLocation(reader.Location); break;
                    case '[': yield return Token.LeftBracket.WithEndLocation(reader.Location); break;
                    case '\\': yield return Token.Backslash.WithEndLocation(reader.Location); break;
                    case ']': yield return Token.RightBracket.WithEndLocation(reader.Location); break;
                    case '^': yield return Token.Caret.WithEndLocation(reader.Location); break;
                    case '_': yield return Token.Underscore.WithEndLocation(reader.Location); break;
                    case '`': yield return Token.Backtick.WithEndLocation(reader.Location); break;
                    case '{': yield return Token.LeftBrace.WithEndLocation(reader.Location); break;
                    case '|':
                        {
                            if (reader.Peek() == '|')
                            {
                                reader.Read();

                                yield return Token.ComplexEntityConstruction.WithEndLocation(reader.Location);
                            }
                            else
                            {
                                yield return Token.Pipe.WithEndLocation(reader.Location);
                            }

                            break;
                        }
                    case '}': yield return Token.RightBrace.WithEndLocation(reader.Location); break;
                    case '~': yield return Token.Tilde.WithEndLocation(reader.Location); break;
                    case '(':
                        {
                            if (reader.Peek() == '*')
                            {
                                reader.Read();

                                int remarksNesting = 1;

                                while ((current = reader.Read()) != -1)
                                {
                                    if (current == '(' && reader.Peek() == '*')
                                    {
                                        reader.Read();
                                        remarksNesting++;
                                    }
                                    else if (current == '*' && reader.Peek() == ')')
                                    {
                                        reader.Read();
                                        remarksNesting--;
                                    }

                                    if (remarksNesting == 0)
                                        break;
                                }

                                break;
                            }
                            else
                                yield return Token.LeftParen.WithEndLocation(reader.Location);

                            break;
                        }

                    case ')': yield return Token.RightParen.WithEndLocation(reader.Location); break;

                    case '\'':
                        {
                            buffer.Clear();

                            while ((current = reader.Peek()) != -1)
                            {
                                if (current == '\n')
                                {
                                    emitError(Errors.TokenizerUnexpectedNewlineInStringLiteral);
                                    yield return new Token(buffer.ToString(), TokenKind.SimpleStringLiteral,
                                        CreateSpan(reader.Location, buffer.Length + 1));
                                    break;
                                }
                                else if (current == '\'')
                                {
                                    reader.Read();
                                    if (reader.Peek() == '\'')
                                        buffer.Append("'");
                                    else
                                    {
                                        yield return new Token(buffer.ToString(), TokenKind.SimpleStringLiteral,
                                            CreateSpan(reader.Location, buffer.Length + 2));
                                        break;
                                    }
                                }
                                else if (current == -1)
                                    yield return new Token(buffer.ToString(), TokenKind.SimpleStringLiteral,
                                        CreateSpan(reader.Location, buffer.Length + 1));
                                else
                                {
                                    buffer.Append((char)reader.Read());
                                }

                            }

                            break;
                        }

                    case char letter when IsLetter(letter):
                        {
                            buffer.Clear();
                            buffer.Append(letter);

                            while (IsLetterOrDigitOrUnderscore(reader.Peek()))
                            {
                                letter = (char)reader.Read();
                                buffer.Append(letter);
                            }

                            if (Keywords.IsKeyword(buffer.ToString()))
                                yield return Token.FromKeyword(buffer.ToString()).WithEndLocation(reader.Location);
                            else
                                yield return new Token(buffer.ToString(), TokenKind.SimpleId).WithEndLocation(reader.Location);

                            break;
                        }

                    case char digit when (digit >= '0' && digit <= '9'):
                        {
                            buffer.Clear();
                            buffer.Append(digit);

                            while (IsDigit(reader.Peek()))
                            {
                                buffer.Append((char)reader.Read());
                            }

                            if (reader.Peek() == '.')
                            {
                                reader.Read();
                                buffer.Append(".");

                                while (IsDigit(reader.Peek()))
                                {
                                    buffer.Append((char)reader.Read());
                                }

                                current = reader.Peek();
                                if (current == 'e' || current == 'E')
                                {
                                    reader.Read();
                                    buffer.Append('e');

                                    current = reader.Peek();

                                    if (current == '-')
                                    {
                                        reader.Read();
                                        buffer.Append('-');
                                    }
                                    else if (current == '+')
                                    {
                                        reader.Read();
                                    }

                                    if (IsDigit(reader.Peek()))
                                    {
                                        while (IsDigit(reader.Peek()))
                                        {
                                            buffer.Append((char)reader.Read());
                                        }
                                    }
                                    else
                                    {
                                        emitError(Errors.TokenizerMissingExponent);
                                        break;
                                    }
                                }

                                if (double.TryParse(buffer.ToString(),
                                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                                    CultureInfo.InvariantCulture,
                                    out var result))
                                    yield return new Token(result.ToString(CultureInfo.InvariantCulture),
                                        TokenKind.RealLiteral, CreateSpan(reader.Location, buffer.Length));
                                else
                                    emitError(Errors.TokenizerRealLiteralOutOfRange);
                            }
                            else
                            {
                                if (int.TryParse(buffer.ToString(), out int result))
                                    yield return new Token(result.ToString(), TokenKind.IntegerLiteral,
                                        CreateSpan(reader.Location, buffer.Length));
                                else
                                    emitError(Errors.TokenizerIntegerLiteralIsTooLong);
                            }


                            break;
                        }

                    case char whiteSpace when IsWhiteSpace(whiteSpace):
                        break;


                    default:
                        emitError(string.Format(Errors.TokenizerUnrecognizedCharacter, ch));
                        break;
                }
            }

            yield return Token.Eof;
        }

    }
}
