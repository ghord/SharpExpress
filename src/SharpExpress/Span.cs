using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SharpExpress
{


    [Serializable]
    public struct Span
    {

        public Span(Location start, Location end)
        {
            if (start.IsUnknown || end.IsUnknown)
            {
                Start = default(Location);
                End = default(Location);
            }
            else

                if (start > end)
                throw new ArgumentException("Start location should be before end location");

            Start = start;
            End = end;
        }


        public static Span Empty { get; } = default(Span);

        public Location Start { get; }
        public Location End { get; }

        public bool Contains(Location location)
        {
            if (location.IsUnknown)
                return false;

            return Start <= location && End >= location;
        }
    }
}
