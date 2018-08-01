using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;

namespace SharpExpress
{
    /// <summary>
    /// Information about position in a file
    /// </summary>
    [Serializable]
    public struct Location : IComparable<Location>
    {
        public Location(int line, int column)
        {
            if (line <= 0)
                throw new ArgumentOutOfRangeException(nameof(line));

            if (column <= 0)
                throw new ArgumentOutOfRangeException(nameof(column));

            Line = line;
            Column = column;
        }

        public int Line { get; }
        public int Column { get; }

        public static Location Unknown { get; } = new Location();

        public bool IsUnknown => Line == 0 && Column == 0;

        public override string ToString()
        {
            if (IsUnknown)
                return "?";

            return $"{Line},{Column}";
        }

        public int CompareTo(Location other)
        {
            var comparison = Line.CompareTo(other.Line);

            if (comparison != 0)
                return comparison;

            return Column.CompareTo(other.Column);
        }

        public static bool operator <(Location x, Location y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator >(Location x, Location y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(Location x, Location y)
        {
            return x.CompareTo(y) >= 0;
        }

        public static bool operator <=(Location x, Location y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator ==(Location x, Location y)
        {
            return x.CompareTo(y) == 0;
        }

        public static bool operator !=(Location x, Location y)
        {
            return x.CompareTo(y) != 0;
        }

        public override bool Equals(object obj)
        {
            if(obj is Location loc)
            {
                return CompareTo(loc) == 0;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return unchecked((Line * (int)0xA5555529) + Column);
        }
    }
}