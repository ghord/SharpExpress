using System;
using System.Runtime.Serialization;

namespace SharpExpress.Parsing
{
    [Serializable]
    internal class ParsingException : Exception
    {
        public ParsingException(string message, Span span)
            : base(message)
        {
            Span = span;
        }

        public Span Span { get; }

        protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}