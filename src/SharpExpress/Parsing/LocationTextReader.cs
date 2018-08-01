using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    /// <summary>
    /// TextReader wrapper that keeps track of current file position
    /// </summary>
    class LocationTextReader : TextReader
    {
        private TextReader reader_;
        private int column_;
        private int line_;

        public LocationTextReader(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            line_ = 1;
            column_ = 1;
            reader_ = reader;
        }

        public override int Peek()
        {
            return reader_.Peek();
        }

        public override int Read()
        {
            var ch = reader_.Read();

            if (ch == '\n')
            {
                column_ = 1;
                line_++;
            }
            else
            {
                column_++;
            }

            return ch;
        }

        /// <summary>
        /// Line info for current position 
        /// </summary>
        public Location Location => new Location(line_, column_);

    }
}
