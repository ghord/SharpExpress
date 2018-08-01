using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    public enum ErrorSource
    {
        Tokenizer,
        Parser,
    }
    
    public class ParsingError
    {
        public ParsingError(ErrorSource source, string message, Span span)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            Source = source;
            Message = message;
            Span = span;
        }

        public string Message { get; }
        public Span Span { get; }
        public ErrorSource Source { get; }

        public override string ToString()
        {
            return $"{Source}: {Message} at {Span.Start}.";
        }
    }
}
