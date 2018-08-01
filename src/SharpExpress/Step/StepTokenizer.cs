using SharpExpress.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Step
{
    public class DoubleBuffered<T> where T : class, new()
    {
        private T first_, second_;
        public DoubleBuffered()
        {
            first_ = new T();
            second_ = new T();
        }

        public void Swap()
        {
            var temp = second_;
            second_ = first_;
            first_ = temp;
        }

        public T Current => first_;
    }

    [Serializable]
    public class UnexpectedCharacterException : Exception
    {
        public UnexpectedCharacterException(char character) :
            base($"Unexpected character '{character}'")
        { }

        protected UnexpectedCharacterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException() { }
        public UnexpectedTokenException(string message) : base(message) { }
        public UnexpectedTokenException(string message, Exception inner) : base(message, inner) { }
        protected UnexpectedTokenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public enum StepTokenKind
    {
        UserDefinedKeyword,
        StandardKeyword,
        Integer,
        Real,
        String,
        EntityInstanceName,
        Enumeration,
        Binary,
        Dollar,
        Header,
        Data,
        EndSection,
        Iso,
        EndIso,
        Asterisk,
        Semicolon,
        LeftParen,
        RightParen,
        Comma,
        Solidus,
        Assignment,
        Invalid,
        Eof
    }

    [DebuggerDisplay("{Kind,nq}")]
    public struct StepToken
    {
        public StepToken(StepTokenKind kind, FastStringBuilder value)
        {
            Kind = kind;
            StringBuilder = value;
        }

        private StepToken(StepTokenKind kind)
        {
            Kind = kind;
            StringBuilder = null;
        }

        public StepTokenKind Kind { get; }

        public FastStringBuilder StringBuilder { get; }

        public override string ToString()
        {
            return StringBuilder?.ToString();
        }

        public string ToStringInterned()
        {
            return StringBuilder?.ToStringInterned();
        }

        public int ToInt()
        {
            return StringBuilder.ParseInt();
        }

        public unsafe float ToReal()
        {
            return (float)StringBuilder.ParseDouble();
        }

        public bool Equals(string value)
        {
            return StringBuilder.Equals(value);
        }

        public static StepToken Eof { get; } = new StepToken(StepTokenKind.Eof);
        public static StepToken Semicolon { get; } = new StepToken(StepTokenKind.Semicolon);
        public static StepToken Asterisk { get; } = new StepToken(StepTokenKind.Asterisk);
        public static StepToken LeftParen { get; } = new StepToken(StepTokenKind.LeftParen);
        public static StepToken RightParen { get; } = new StepToken(StepTokenKind.RightParen);
        public static StepToken Dollar { get; } = new StepToken(StepTokenKind.Dollar);
        public static StepToken Comma { get; } = new StepToken(StepTokenKind.Comma);
        public static StepToken Assignment { get; } = new StepToken(StepTokenKind.Assignment);
        public static StepToken Solidus { get; } = new StepToken(StepTokenKind.Solidus);
        public static StepToken Iso { get; } = new StepToken(StepTokenKind.Iso);
        public static StepToken EndIso { get; } = new StepToken(StepTokenKind.EndIso);
        public static StepToken EndSection { get; } = new StepToken(StepTokenKind.EndSection);
        public static StepToken Header { get; } = new StepToken(StepTokenKind.Header);
        public static StepToken Data { get; } = new StepToken(StepTokenKind.Data);


    }

    public static class StepTokenizer
    {
        private static Encoding[] encodings_ = {
            Encoding.GetEncoding("iso-8859-1"),
            Encoding.GetEncoding("iso-8859-2"),
            Encoding.GetEncoding("iso-8859-3"),
            Encoding.GetEncoding("iso-8859-4"),
            Encoding.GetEncoding("iso-8859-5"),
            Encoding.GetEncoding("iso-8859-6"),
            Encoding.GetEncoding("iso-8859-7"),
            Encoding.GetEncoding("iso-8859-8"),
            Encoding.GetEncoding("iso-8859-9")
        };

        private static Encoding unicode_ = Encoding.Unicode;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDigit(int character)
        {
            return character >= '0' && character <= '9';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsHex(int character)
        {
            return (character >= '0' && character <= '9') || (character >= 'A' && character <= 'F');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsUpper(int character)
        {
            return (character >= 'A' && character <= 'Z') || character == '_';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsUpperOrDigit(int character)
        {
            return IsUpper(character) || IsDigit(character);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsUpperOrDigitOrMinus(int character)
        {
            return IsUpper(character) || IsDigit(character) || character == '-';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsLower(int character)
        {
            return character >= 'a' && character <= 'z';
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsCharacter(int character)
        {
            return (character >= ' ' && character <= '~');
        }

        public static StepTokenKind ReadNext(FastBinaryReader reader, FastStringBuilder buffer)
        {

            int current;

            while ((current = reader.Read()) != -1)
            {
                var ch = (char)current;

                switch (ch)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            buffer.Clear();

                            buffer.Append(ch);
                            return ParseNumberKind(reader, buffer);
                        };

                    case '-':
                    case '+':
                        {
                            if (IsDigit(reader.Peek()))
                            {
                                buffer.Clear();

                                buffer.Append(ch);
                                return ParseNumberKind(reader, buffer);
                            }
                            else
                            {
                                throw new UnexpectedCharacterException(ch);
                            }
                        }
                    case '#':
                        {
                            if (!IsDigit(reader.Peek()))
                            {
                                throw new UnexpectedCharacterException(ch);
                            }
                            
                            buffer.Clear();

                            do
                            {
                                buffer.Append((char)reader.Read());
                            }
                            while (IsDigit(reader.Peek()));

                            return StepTokenKind.EntityInstanceName;
                        }
                    case '"':
                        {
                            throw new NotImplementedException("binary");

                        }

                    case '\'':
                        {
                            buffer.Clear();

                            return ParseStringKind(reader, buffer);
 
                        }
                    case '!':
                        {
                            throw new NotImplementedException("user defined keyword");

                        }
                    case '.':
                        {
                            if (!IsUpper(reader.Peek()))
                                throw new UnexpectedCharacterException(ch);

                            buffer.Clear();

                            do
                            {
                                buffer.Append((char)reader.Read());

                            } while (IsUpperOrDigit(reader.Peek()));

                            if (reader.Peek() == '.')
                            {
                                reader.Read();
                                return StepTokenKind.Enumeration;
                            }
                            else
                                throw new UnexpectedCharacterException((char)reader.Peek());
                        }
                    case '=':
                        return StepTokenKind.Assignment;
                    case '*':
                        return StepTokenKind.Asterisk;
                    case '$':
                        return StepTokenKind.Dollar;
                    case ';':
                        return StepTokenKind.Semicolon;
                    case '(':
                        return StepTokenKind.LeftParen;
                    case ')':
                        return StepTokenKind.RightParen;
                    case '/':
                        {
                            if (reader.Peek() == '*')
                            {
                                while ((current = reader.Read()) != -1)
                                {
                                    if (current == '*' && reader.Peek() == '/')
                                    {
                                        reader.Read();
                                        break;
                                    }

                                }
                            }
                            else
                                return StepTokenKind.Solidus;
                        }
                        break;
                    case ',':
                        return StepTokenKind.Comma;
                    case ' ':
                        break;
                    default:
                        {
                            if (IsUpper(ch))
                            {
                 
                                buffer.Clear();

                                buffer.Append(ch);
                                while (IsUpperOrDigitOrMinus(reader.Peek()))
                                {
                                    buffer.Append((char)reader.Read());
                                }

                                if (buffer.Equals("ISO-10303-21"))
                                    return StepTokenKind.Iso;
                                else if (buffer.Equals("END-ISO-10303-21"))
                                    return StepTokenKind.EndIso;
                                else if (buffer.Equals("HEADER"))
                                    return StepTokenKind.Header;
                                else if (buffer.Equals("ENDSEC"))
                                    return StepTokenKind.EndSection;
                                else if (buffer.Equals("DATA"))
                                    return StepTokenKind.Data;
                                else
                                    return (StepTokenKind.StandardKeyword);

                            }
                            else if (IsCharacter(ch))
                            {
                                throw new UnexpectedCharacterException(ch);
                            }

                            break;
                        }
                }
            }

            return StepTokenKind.Eof;
        }

        public static IEnumerable<StepToken> Tokenize(FastBinaryReader reader)
        {
            int current;

            var buffers = new DoubleBuffered<FastStringBuilder>();
            //var keywordBuilder = new KeywordStringBuilder();
            while ((current = reader.Read()) != -1)
            {
                var ch = (char)current;

                var buffer = buffers.Current;

                switch (ch)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            buffers.Swap();
                            buffer.Clear();

                            buffer.Append(ch);
                            yield return ParseNumber(reader, buffer);
                            break;
                        };

                    case '-':
                    case '+':
                        {
                            if (IsDigit(reader.Peek()))
                            {
                                buffers.Swap();
                                buffer.Clear();

                                buffer.Append(ch);
                                yield return ParseNumber(reader, buffer);
                                break;
                            }
                            else
                            {
                                throw new UnexpectedCharacterException(ch);
                            }
                        }
                    case '#':
                        {
                            if (!IsDigit(reader.Peek()))
                            {
                                throw new UnexpectedCharacterException(ch);
                            }

                            buffers.Swap();
                            buffer.Clear();

                            do
                            {
                                buffer.Append((char)reader.Read());
                            }
                            while (IsDigit(reader.Peek()));

                            yield return new StepToken(StepTokenKind.EntityInstanceName, buffer);
                            break;
                        }
                    case '"':
                        {
                            throw new NotImplementedException("binary");

                        }

                    case '\'':
                        {
                            buffers.Swap();
                            buffer.Clear();

                            yield return ParseString(reader, buffer);
                            break;
                        }
                    case '!':
                        {
                            throw new NotImplementedException("user defined keyword");

                        }
                    case '.':
                        {
                            if (!IsUpper(reader.Peek()))
                                throw new UnexpectedCharacterException(ch);

                            buffers.Swap();
                            buffer.Clear();

                            do
                            {
                                buffer.Append((char)reader.Read());

                            } while (IsUpperOrDigit(reader.Peek()));

                            if (reader.Peek() == '.')
                            {
                                reader.Read();
                                yield return new StepToken(StepTokenKind.Enumeration, buffer);
                                break;
                            }
                            else
                                throw new UnexpectedCharacterException((char)reader.Peek());
                        }
                    case '=':
                        yield return StepToken.Assignment;
                        break;
                    case '*':
                        yield return StepToken.Asterisk;
                        break;
                    case '$':
                        yield return StepToken.Dollar;
                        break;
                    case ';':
                        yield return StepToken.Semicolon;
                        break;
                    case '(':
                        yield return StepToken.LeftParen;
                        break;
                    case ')':
                        yield return StepToken.RightParen;
                        break;
                    case '/':
                        {
                            if (reader.Peek() == '*')
                            {
                                while ((current = reader.Read()) != -1)
                                {
                                    if (current == '*' && reader.Peek() == '/')
                                    {
                                        reader.Read();
                                        break;
                                    }

                                }
                            }
                            else
                                yield return StepToken.Solidus;
                        }
                        break;
                    case ',':
                        yield return StepToken.Comma;
                        break;
                    case ' ':
                        break;
                    default:
                        {
                            if (IsUpper(ch))
                            {
                                buffers.Swap();
                                buffer.Clear();

                                buffer.Append(ch);
                                while (IsUpperOrDigitOrMinus(reader.Peek()))
                                {
                                    buffer.Append((char)reader.Read());
                                }

                                if (buffer.Equals("ISO-10303-21"))
                                    yield return StepToken.Iso;
                                else if (buffer.Equals("END-ISO-10303-21"))
                                    yield return StepToken.EndIso;
                                else if (buffer.Equals("HEADER"))
                                    yield return StepToken.Header;
                                else if (buffer.Equals("ENDSEC"))
                                    yield return StepToken.EndSection;
                                else if (buffer.Equals("DATA"))
                                    yield return StepToken.Data;
                                else
                                    yield return new StepToken(StepTokenKind.StandardKeyword, buffer);

                                break;
                            }
                            else if (IsCharacter(ch))
                            {
                                throw new UnexpectedCharacterException(ch);
                            }

                            break;
                        }
                }
            }

            yield return StepToken.Eof;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FromHex(int character)
        {
            switch (character)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'A': return 10;
                case 'B': return 11;
                case 'C': return 12;
                case 'D': return 13;
                case 'E': return 14;
                case 'F': return 15;
                default:
                    throw new UnexpectedCharacterException((char)character);
            }
        }

        private unsafe static StepToken ParseString(FastBinaryReader reader, FastStringBuilder buffer)
        {
            int encoding = 0;
            int next;
            while ((next = reader.Read()) != -1)
            {
                if (next == '\'')
                {
                    if (reader.Peek() == '\'')
                    {
                        reader.Read();
                        buffer.Append('\'');
                    }
                    else
                    {
                        return new StepToken(StepTokenKind.String, buffer);
                    }
                }
                else if (next == '\\')
                {
                    switch (reader.Read())
                    {
                        case 'S':
                            {
                                next = reader.Read();

                                if (next != '\\')
                                    throw new UnexpectedCharacterException((char)next);

                                next = reader.Read();

                                if (IsCharacter(next))
                                {
                                    byte* ptr = stackalloc byte[1];
                                    char* result = stackalloc char[1];
                                    ptr[0] = (byte)(next + 128);
                                    encodings_[encoding].GetChars(ptr, 1, result, 1);
                                    buffer.Append(*result);
                                    break;
                                }
                                else
                                {
                                    throw new UnexpectedCharacterException((char)next);
                                }
                            }

                        case 'P':
                            {
                                next = reader.Read();
                                if (next >= 'A' && next <= 'I')
                                {
                                    encoding = next - 'A';
                                }
                                else
                                    throw new UnexpectedCharacterException((char)next);

                                next = reader.Read();
                                if (next != '\\')
                                    throw new UnexpectedCharacterException((char)next);

                                break;
                            }
                        case 'X':
                            {
                                next = reader.Read();

                                if (next == '\\')
                                {
                                    byte* ptr = stackalloc byte[1];
                                    char* result = stackalloc char[1];

                                    next = reader.Read();

                                    if (!IsHex(next))
                                        throw new UnexpectedCharacterException((char)next);

                                    ptr[0] = (byte)(FromHex(next) << 8);
                                    next = reader.Read();
                                    ptr[0] |= (byte)FromHex(next);
                                    encodings_[0].GetChars(ptr, 1, result, 1);

                                    buffer.Append(*result);
                                    break;
                                }
                                else if (next == '2')
                                {
                                    byte* ptr = stackalloc byte[20];
                                    char* chars = stackalloc char[10];

                                    Expect(reader, '\\');

                                    int offset = 0;

                                    while (IsHex(reader.Peek()))
                                    {
                                        next = reader.Read();
                                        ptr[offset] = (byte)(FromHex(next) << 24);
                                        next = reader.Read();
                                        ptr[offset] |= (byte)(FromHex(next) << 16);
                                        offset++;
                                        next = reader.Read();
                                        ptr[offset] = (byte)(FromHex(next) << 8);
                                        next = reader.Read();
                                        ptr[offset] |= (byte)(FromHex(next));
                                        offset++;

                                        if (offset == 20)
                                        {
                                            Encoding.Unicode.GetChars(ptr, offset, chars, 20);
                                            buffer.Append(chars, 10);
                                            offset = 0;
                                        }
                                    }

                                    var encoded = Encoding.Unicode.GetChars(ptr, offset, chars, 20);
                                    buffer.Append(chars, encoded);

                                    Expect(reader, '\\');
                                    Expect(reader, 'X');
                                    Expect(reader, '0');
                                    Expect(reader, '\\');

                                    break;
                                }
                                else if (next == '4')
                                {
                                    throw new NotImplementedException();
                                }
                                else
                                    throw new UnexpectedCharacterException((char)next);
                            }
                        case '\\':
                            buffer.Append('\\');
                            break;
                        case int ch:
                            throw new UnexpectedCharacterException((char)ch);

                    }
                }
                else if (IsCharacter(next))
                {
                    buffer.Append((char)next);
                }
                else
                {
                    throw new UnexpectedCharacterException((char)next);
                }
            }

            return new StepToken(StepTokenKind.String, buffer);
        }

        private unsafe static StepTokenKind ParseStringKind(FastBinaryReader reader, FastStringBuilder buffer)
        {
            int encoding = 0;
            int next;
            while ((next = reader.Read()) != -1)
            {
                if (next == '\'')
                {
                    if (reader.Peek() == '\'')
                    {
                        reader.Read();
                        buffer.Append('\'');
                    }
                    else
                    {
                        return StepTokenKind.String;
                    }
                }
                else if (next == '\\')
                {
                    switch (reader.Read())
                    {
                        case 'S':
                            {
                                next = reader.Read();

                                if (next != '\\')
                                    throw new UnexpectedCharacterException((char)next);

                                next = reader.Read();

                                if (IsCharacter(next))
                                {
                                    byte* ptr = stackalloc byte[1];
                                    char* result = stackalloc char[1];
                                    ptr[0] = (byte)(next + 128);
                                    encodings_[encoding].GetChars(ptr, 1, result, 1);
                                    buffer.Append(*result);
                                    break;
                                }
                                else
                                {
                                    throw new UnexpectedCharacterException((char)next);
                                }
                            }

                        case 'P':
                            {
                                next = reader.Read();
                                if (next >= 'A' && next <= 'I')
                                {
                                    encoding = next - 'A';
                                }
                                else
                                    throw new UnexpectedCharacterException((char)next);

                                next = reader.Read();
                                if (next != '\\')
                                    throw new UnexpectedCharacterException((char)next);

                                break;
                            }
                        case 'X':
                            {
                                next = reader.Read();

                                if (next == '\\')
                                {
                                    byte* ptr = stackalloc byte[1];
                                    char* result = stackalloc char[1];

                                    next = reader.Read();

                                    if (!IsHex(next))
                                        throw new UnexpectedCharacterException((char)next);

                                    ptr[0] = (byte)(FromHex(next) << 8);
                                    next = reader.Read();
                                    ptr[0] |= (byte)FromHex(next);
                                    encodings_[0].GetChars(ptr, 1, result, 1);

                                    buffer.Append(*result);
                                    break;
                                }
                                else if (next == '2')
                                {
                                    byte* ptr = stackalloc byte[20];
                                    char* chars = stackalloc char[10];

                                    Expect(reader, '\\');

                                    int offset = 0;

                                    while (IsHex(reader.Peek()))
                                    {
                                        next = reader.Read();
                                        ptr[offset] = (byte)(FromHex(next) << 24);
                                        next = reader.Read();
                                        ptr[offset] |= (byte)(FromHex(next) << 16);
                                        offset++;
                                        next = reader.Read();
                                        ptr[offset] = (byte)(FromHex(next) << 8);
                                        next = reader.Read();
                                        ptr[offset] |= (byte)(FromHex(next));
                                        offset++;

                                        if (offset == 20)
                                        {
                                            Encoding.Unicode.GetChars(ptr, offset, chars, 20);
                                            buffer.Append(chars, 10);
                                            offset = 0;
                                        }
                                    }

                                    var encoded = Encoding.Unicode.GetChars(ptr, offset, chars, 20);
                                    buffer.Append(chars, encoded);

                                    Expect(reader, '\\');
                                    Expect(reader, 'X');
                                    Expect(reader, '0');
                                    Expect(reader, '\\');

                                    break;
                                }
                                else if (next == '4')
                                {
                                    throw new NotImplementedException();
                                }
                                else
                                    throw new UnexpectedCharacterException((char)next);
                            }
                        case '\\':
                            buffer.Append('\\');
                            break;
                        case int ch:
                            throw new UnexpectedCharacterException((char)ch);

                    }
                }
                else if (IsCharacter(next))
                {
                    buffer.Append((char)next);
                }
                else
                {
                    throw new UnexpectedCharacterException((char)next);
                }
            }

            return StepTokenKind.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Expect(FastBinaryReader reader, char character)
        {
            var next = reader.Read();

            if (next != character)
                throw new UnexpectedCharacterException((char)next);
        }

        private static StepTokenKind ParseNumberKind(FastBinaryReader reader, FastStringBuilder buffer)
        {

            //integer or real
            while (IsDigit(reader.Peek()))
            {
                buffer.Append((char)reader.Read());
            }

            if (reader.Peek() == '.')
            {

                buffer.Append((char)reader.Read());

                while (IsDigit(reader.Peek()))
                {
                    //realValue += 
                    buffer.Append((char)reader.Read());
                }

                if (reader.Peek() == 'E')
                {
                    reader.Read();
                    buffer.Append('E');

                    var next = reader.Peek();

                    if (next == '+' || next == '-')
                    {
                        buffer.Append((char)next);
                        reader.Read();
                        next = reader.Peek();
                    }

                    if (IsDigit(next))
                    {
                        buffer.Append((char)reader.Read());
                    }
                    else
                    {
                        throw new UnexpectedCharacterException((char)next);
                    }

                    while (IsDigit(reader.Peek()))
                    {
                        buffer.Append((char)reader.Read());
                    }
                }

                return StepTokenKind.Real;
            }
            else
            {
                return StepTokenKind.Integer;
            }
        }

        private static StepToken ParseNumber(FastBinaryReader reader, FastStringBuilder buffer)
        {

            //integer or real
            while (IsDigit(reader.Peek()))
            {
                buffer.Append((char)reader.Read());
            }

            if (reader.Peek() == '.')
            {

                buffer.Append((char)reader.Read());

                while (IsDigit(reader.Peek()))
                {
                    //realValue += 
                    buffer.Append((char)reader.Read());
                }

                if (reader.Peek() == 'E')
                {
                    reader.Read();
                    buffer.Append('E');

                    var next = reader.Peek();

                    if (next == '+' || next == '-')
                    {
                        buffer.Append((char)next);
                        reader.Read();
                        next = reader.Peek();
                    }

                    if (IsDigit(next))
                    {
                        buffer.Append((char)reader.Read());
                    }
                    else
                    {
                        throw new UnexpectedCharacterException((char)next);
                    }

                    while (IsDigit(reader.Peek()))
                    {
                        buffer.Append((char)reader.Read());
                    }
                }

                return new StepToken(StepTokenKind.Real, buffer);
            }
            else
            {
                return new StepToken(StepTokenKind.Integer, buffer);
            }
        }
    }
}
