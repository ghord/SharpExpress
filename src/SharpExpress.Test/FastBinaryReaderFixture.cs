using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpExpress.Step;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Test
{
    [TestClass]
    public class FastBinaryReaderFixture
    {
        [TestMethod]
        public void FastBinaryReader_ShouldReadFile()
        {
            using (var reader = new FastBinaryReader(@"C:\ifc\Hote6005.ifc"))
            {
                long bytes = 0;
                int lines = 0;
                while(reader.Peek() != -1)
                {
                    if(reader.Read() == '\n')
                    lines++;

                    bytes++;
                }

                Console.WriteLine($"Bytes: {bytes}, Lines: {lines}");
            }
        }

        [TestMethod]
        public void FastBinaryReader_ShouldReadFileSingleThreaded()
        {
            using (var reader = new FastBinaryReader(@"C:\ifc\Hote6005.ifc", false))
            {
                long bytes = 0;
                int lines = 0;
                while (reader.Peek() != -1)
                {
                    if (reader.Read() == '\n')
                        lines++;

                    bytes++;
                }

                Console.WriteLine($"Bytes: {bytes}, Lines: {lines}");
            }
        }

        [TestMethod]
        public void FastBinaryReader_ShouldWorkForAllSizesMultiThreaded()
        {
            const int max = 10000;

            var bytes = Enumerable.Range(0, max).Select(i => (byte)((i + 17) % 251)).ToArray();
            for(int i=1; i< max; i++)
            {
                using (var stream = new MemoryStream(bytes, 0, i))
                {
                    using (var reader = new FastBinaryReader(stream))
                    {
                        int total = 0;
                        while (reader.Peek() != -1)
                        {
                            total++;

                            byte peek = (byte)reader.Peek();
                            byte read = (byte)reader.Read();

                            Assert.AreEqual(peek, read, "peek failed for byte " + total);
                            Assert.AreEqual(bytes[total-1], peek, "read failed for byte " + total);
                        }

                        Assert.AreEqual(i, total, "Failed for i=" + i);
                    }
                }
            }
        }

        [TestMethod]
        public void FastBinaryReader_ShouldWorkForAllSizesSingleThreaded()
        {
            const int max = 10000;

            var bytes = Enumerable.Range(0, max).Select(i => (byte)((i + 17) % 251)).ToArray();
            for (int i = 1; i < max; i++)
            {
                using (var stream = new MemoryStream(bytes, 0, i))
                {
                    using (var reader = new FastBinaryReader(stream, false))
                    {
                        int total = 0;
                        while (reader.Peek() != -1)
                        {
                            total++;

                            byte peek = (byte)reader.Peek();
                            byte read = (byte)reader.Read();

                            Assert.AreEqual(peek, read, "peek failed for byte " + total);
                            Assert.AreEqual(bytes[total - 1], peek, "read failed for byte " + total);
                        }

                        Assert.AreEqual(i, total, "Failed for i=" + i);
                    }
                }
            }
        }
    }
}
