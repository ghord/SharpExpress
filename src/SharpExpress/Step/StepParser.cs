using SharpExpress.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpExpress.Step
{
    public class StepEntityParser : StepParser
    {
        
    }


    public abstract class StepParser
    {
        private PeekEnumerator<StepToken> enumerator_;
        private StepDataSection currentSection_;
        protected virtual void ProcessEntity(StepDataSection section, string entityType, IReadOnlyList<StepValue> attributeValues) { }
        protected virtual void ProcessHeaderEntity(string entityType, IReadOnlyList<StepValue> attributeValues) { }
        protected virtual void ProcessFileDescription(StepFileDescription fileDescription) { }
        protected virtual void ProcessFileName(StepFileName fileName) { }
        protected virtual void ProcessFileSchema(StepFileSchema fileSchema) { }
        protected virtual void ProcessDataSection(StepDataSection dataSection) { }

        protected virtual StepEntity ConstructHeaderEntity(string entityType, IReadOnlyList<StepValue> attributeValues)
        {
            return new StepEntity(entityType, attributeValues);
        }

        public void Parse(string fileName, IProgress<int> progress = null, CancellationToken token = default)
        {
            using (var reader = new FastBinaryReader(fileName))
            {
                enumerator_ = new PeekEnumerator<StepToken>(StepTokenizer.Tokenize(reader));
                ParseStep();
            }
        }

        private void ParseStep()
        {
            Expect(StepTokenKind.Iso);
            Expect(StepTokenKind.Semicolon);

            ParseHeaderSection();

            while (TryParseDataSection()) ; 

            Expect(StepTokenKind.EndIso);
            Expect(StepTokenKind.Semicolon);
            Expect(StepTokenKind.Eof);
        }

        private bool TryParseDataSection()
        {
            if (Accept(StepTokenKind.Data))
            {
                currentSection_ = new StepDataSection();

                Expect(StepTokenKind.Semicolon);

                ProcessDataSection(currentSection_);

                while (TryParseEntity()) ;

                Expect(StepTokenKind.EndSection);
                Expect(StepTokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private StepToken Current => enumerator_.Current;

        private bool TryParseEntity()
        {
            if (Accept(StepTokenKind.EntityInstanceName))
            {
                Expect(StepTokenKind.Assignment);

                if (Accept(StepTokenKind.StandardKeyword))
                {
                    ProcessEntity(currentSection_, 
                        Current.ToStringInterned(), 
                        ParseParameterList());

                    Expect(StepTokenKind.Semicolon);
                }
                else
                {
                    Expect(StepTokenKind.UserDefinedKeyword);

                    throw new NotImplementedException();
                }

                return true;
            }

            return false;
        }

        private void ParseHeaderSection()
        {
            Expect(StepTokenKind.Header);
            Expect(StepTokenKind.Semicolon);

            ParseFileDescription();
            ParseFileName();
            ParseFileSchema();

            Expect(StepTokenKind.EndSection);
            Expect(StepTokenKind.Semicolon);
        }

        private IReadOnlyList<StepValue> ParseParameterList()
        {
            var result = new List<StepValue>();

            Expect(StepTokenKind.LeftParen);

            if (TryParseValue() is StepValue value)
            {
                result.Add(value);
            }

            while (Accept(StepTokenKind.Comma))
            {
                result.Add(ParseValue());
            }

            Expect(StepTokenKind.RightParen);

            return result;
        }


        private void ParseFileName()
        {
            var fileName = new StepFileName();

            Expect(StepTokenKind.StandardKeyword, "FILE_NAME");

            StepConvert.PopulateObject(fileName, ParseParameterList());

            Expect(StepTokenKind.Semicolon);

            ProcessFileName(fileName);
        }



        private void ParseFileSchema()
        {
            var fileSchema = new StepFileSchema();

            Expect(StepTokenKind.StandardKeyword, "FILE_SCHEMA");

            StepConvert.PopulateObject(fileSchema, ParseParameterList());

            Expect(StepTokenKind.Semicolon);

            ProcessFileSchema(fileSchema);
        }

        private void ParseFileDescription()
        {
            var fileDescription = new StepFileDescription();

            Expect(StepTokenKind.StandardKeyword, "FILE_DESCRIPTION");

            StepConvert.PopulateObject(fileDescription, ParseParameterList());

            Expect(StepTokenKind.Semicolon);

            ProcessFileDescription(fileDescription);
        }


        private ListValue TryParseListValue()
        {
            if (Accept(StepTokenKind.LeftParen))
            {
                var result = new List<StepValue>();

                if (TryParseValue() is StepValue value)
                {
                    result.Add(value);
                    while (Accept(StepTokenKind.Comma))
                    {
                        result.Add(ParseValue());
                    }
                }

                Expect(StepTokenKind.RightParen);

                return new ListValue(result);
            }

            return null;
        }

        private StepValue TryParseValue()
        {
            if (Accept(StepTokenKind.Dollar))
                return StepValue.Missing;
            else if (Accept(StepTokenKind.Asterisk))
                return StepValue.Derived;

            return TryParseListValue()
                ?? TryParseString()
                ?? TryParseEnumeration()
                ?? TryParseInt()
                ?? TryParseTypedValue()
                ?? TryParseReal()
                ?? TryParseEntityRef() as StepValue;
        }

        private TypedValue TryParseTypedValue()
        {
            if (Accept(StepTokenKind.StandardKeyword))
            {
                return new TypedValue(string.Intern(Current.ToString()), ParseValue());
            }

            return null;
        }

        private StepValue TryParseInt()
        {
            if (Accept(StepTokenKind.Integer))
            {
                return new IntegerValue(Current.ToInt());
            }

            return null;
        }

        private StepValue TryParseReal()
        {
            if (Accept(StepTokenKind.Real))
            {
                return new RealValue(Current.ToReal());
            }

            return null;
        }

        private EnumerationValue TryParseEnumeration()
        {
            if (Accept(StepTokenKind.Enumeration))
            {
                return new EnumerationValue(string.Intern(Current.ToString()));
            }

            return null;
        }

        private EntityValue TryParseEntityRef()
        {
            if (Accept(StepTokenKind.EntityInstanceName))
            {
                var id = Current.ToInt();
                return new EntityValue(id);
            }

            return null;
        }

        private StepValue ParseValue()
        {
            var result = TryParseValue();

            if (result == null)
            {
                if (enumerator_.TryPeek(out var token))
                {
                    throw new UnexpectedTokenException($"Expected value, actual {token.Kind}({token})");
                }
                else
                    throw new NotImplementedException($"Expected value, actual eof");
            }

            return result;
        }

        private StringValue TryParseString()
        {
            if (Accept(StepTokenKind.String))
            {
                return new StringValue(enumerator_.Current.StringBuilder);
            }

            return null;
        }


        private void Expect(StepTokenKind kind, string text = null)
        {
            if (enumerator_.TryPeek(out var token))
            {
                if (token.Kind == kind)
                {

                    if (text == null || token.StringBuilder.Equals(text))
                    {
                        enumerator_.MoveNext();
                        return;
                    }
                }

                throw new UnexpectedTokenException($"Expected token {kind}({text}), found {token.Kind}({token})");
            }

            throw new UnexpectedTokenException($"Expected token {kind}({text}), found end of file");
        }

        private bool Accept(StepTokenKind kind, string text = null)
        {
            if (enumerator_.TryPeek(out var token) && token.Kind == kind)
            {
                if (text == null || token.StringBuilder.Equals(text))
                {
                    enumerator_.MoveNext();
                    return true;
                }
            }

            return false;
        }
    }
}
