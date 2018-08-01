using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress
{
    public struct SimpleId
    {
        public SimpleId(string value)
        {
            if (value == null)
                throw new ArgumentNullException();

            Value = value.ToLowerInvariant();
        }

        public string Value { get; }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            if (obj is SimpleId sid)
                return sid.Value.Equals(Value, StringComparison.Ordinal);

            return false;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public static explicit operator string(SimpleId simpleId)
        {
            return simpleId.Value;
        }

        public static implicit operator SimpleId(string simpleId)
        {
            return new SimpleId(simpleId);
        }

        public static SimpleId Invalid { get; } = default;

        public bool IsInvalid => Value == null;
    }
}
