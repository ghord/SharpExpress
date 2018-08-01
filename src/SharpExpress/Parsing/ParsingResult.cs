using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    /// <summary>
    /// Result of parsing
    /// </summary>
    public class ParsingResult
    {
        public ParsingResult(ImmutableArray<SchemaDeclaration> schemas, ImmutableArray<ParsingError> errors)
        {
            Schemas = schemas;
            Errors = errors;

            //TODO: warnings
            Success = Errors.Length == 0 && Schemas.Length > 0;
        }


        /// <summary>
        /// Whether parse was successfull
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Resulting schemas from parse
        /// </summary>
        public ImmutableArray<SchemaDeclaration> Schemas { get; }

        /// <summary>
        /// Errors during parsing
        /// </summary>
        public ImmutableArray<ParsingError> Errors { get; }

        public string GetSummary()
        {
            var sb = new StringBuilder();

            if (Success)
                sb.AppendLine("Parse succeeded without errors");
            else
            {
                sb.AppendLine($"Parsing failed with errors ({Errors.Length}):");
                foreach(var error in Errors)
                {
                    sb.AppendLine(error.ToString());
                }
            }

            return sb.ToString();
        }
    }
}
