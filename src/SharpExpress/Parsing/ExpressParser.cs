using SharpExpress.Builders;
using SharpExpress.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    /// <summary>
    /// Express schema parser
    /// </summary>
    public class ExpressParser
    {
        /// <summary>
        /// Parses schema from string
        /// </summary>
        /// <param name="text">Text used for parsing</param>
        /// <returns>Parsing result</returns>
        public ParsingResult Parse(string text)
        {
            using (var reader = new StringReader(text))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses schema from stream. Assumes stream contains only data in ASCII encoding (required by Express spec)
        /// </summary>
        /// <param name="stream">Stream used for parsing</param>
        /// <returns>Parsing result</returns>
        public ParsingResult Parse(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.ASCII))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses schema from text reader
        /// </summary>
        /// <param name="reader">Text reader used for parsing</param>
        /// <returns>Parsing result</returns>
        public ParsingResult Parse(TextReader reader)
        {
            var errors = new List<ParsingError>();
            var tokens = Tokenizer.Tokenize(reader, errors).ToArray();

            var passes = new ParserPass[]
            {
                new DeclarationPass(tokens, errors),
                new SyntaxAnalysisPass(tokens, errors)
            };

            var result = new SyntaxTreeBuilder();

            foreach(var pass in passes)
            {
                pass.Run(result);
            }

            if (errors.Count > 0)
                return new ParsingResult(ImmutableArray<SchemaDeclaration>.Empty, errors.ToImmutableArray());
            else
                return new ParsingResult(result.Build(), ImmutableArray<ParsingError>.Empty);
        }
    }
}
