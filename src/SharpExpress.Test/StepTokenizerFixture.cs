using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpExpress.Step;
using SharpExpress.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Test
{
    [TestClass]
    public class StepTokenizerFixture
    {
        [DataTestMethod]
        [DataRow(@"C:\ifc\Hote6005.ifc")]
        public void StepParser_ShouldTokenizeFileFast(string path)
        {
            var sw = Stopwatch.StartNew();

            using (var file = new FastBinaryReader(path))
            {
                var tokens = new int[Enum.GetValues(typeof(StepTokenKind)).Length];

                foreach (var token in StepTokenizer.Tokenize(file))
                {
                    tokens[(int)token.Kind]++;
                }

                Console.WriteLine(sw.ElapsedMilliseconds);

                foreach (StepTokenKind kind in Enum.GetValues(typeof(StepTokenKind)))
                {
                    Console.WriteLine($"{kind}: {tokens[(int)kind]}");
                }

                Console.WriteLine($"total: {tokens.Sum()}");
            }
        }

        [DataTestMethod]
        [DataRow(@"C:\ifc\Hote6005.ifc")]
        public void StepParser_ShouldTokenizeFileFast2(string path)
        {
            var sw = Stopwatch.StartNew();

            using (var file = new FastBinaryReader(path))
            {
                var tokens = new int[Enum.GetValues(typeof(StepTokenKind)).Length];

                StepTokenKind token;
                var buffer = new FastStringBuilder();
                while ((token = StepTokenizer.ReadNext(file, buffer)) != StepTokenKind.Eof)
                {
                    tokens[(int)token]++;
                }

                Console.WriteLine(sw.ElapsedMilliseconds);

                foreach (StepTokenKind kind in Enum.GetValues(typeof(StepTokenKind)))
                {
                    Console.WriteLine($"{kind}: {tokens[(int)kind]}");
                }

                Console.WriteLine($"total: {tokens.Sum()}");
            }
        }



        [TestMethod]
        public void StepParser_YieldOverhead()
        {
            IEnumerable<(StepTokenKind, int)> Yielder()
            {
                for (int i = 0; i < 238_443_191; i++)
                {
                    yield return (StepTokenKind.EntityInstanceName, i);
                }
            }

            var sw = Stopwatch.StartNew();

            int total = 0;
            foreach (var item in Yielder())
            {
                total++;
            }

            Console.WriteLine(total + " " + sw.ElapsedMilliseconds);
        }


        [TestMethod]
        public void BlockingCollection_Overhead()
        {
            void Yielder(BlockingCollection<(StepTokenKind, int)> tokens)
            {
                for (int i = 0; i < 238_443_191; i++)
                {
                    tokens.Add((StepTokenKind.EntityInstanceName, i));
                }

                tokens.CompleteAdding();
            }

            var collection = new BlockingCollection<(StepTokenKind, int)>(10);
            Task.Factory.StartNew(() => Yielder(collection));

            var sw = Stopwatch.StartNew();

            int total = 0;
            foreach (var item in collection.GetConsumingEnumerable())
            {
                total++;
            }

            Console.WriteLine(total + " " + sw.ElapsedMilliseconds);
        }

        [DataTestMethod]
        //[DataRow(@"C:\ifc\Hote6005.ifc")]
        [DataRow(@"C:\ifc\ESAPROBIMDEMO.ifc")]
        public void StepParser_ShouldTokenizeFile(string path)
        {
            var sw = Stopwatch.StartNew();

            using (var file = new FastBinaryReader(path))
            {
                FastStringBuilder buffer = new FastStringBuilder();
                var tokens = new int[Enum.GetValues(typeof(StepTokenKind)).Length];
                StepTokenKind token;

                while ((token = StepTokenizer.ReadNext(file, buffer)) != StepTokenKind.Eof)
                {
                    tokens[(int)token]++;

                    //if (token == StepTokenKind.Real)
                    //{
                    //    double val = buffer.ParseDouble();
                    //}
                    //else if (token == StepTokenKind.Integer)
                    //{
                    //    int val = buffer.ParseInt();
                    //}
                    //else if (token == StepTokenKind.EntityInstanceName)
                    //{
                    //    int val = buffer.ParseInt();
                    //}
                    //else if (token == StepTokenKind.Enumeration)
                    //{
                    //    string val = buffer.ToStringInterned();
                    //}
                    //else if (token == StepTokenKind.StandardKeyword)
                    //{
                    //    string val = buffer.ToStringInterned();
                    //}
                    //else if (token == StepTokenKind.String)
                    //{
                    //    string val = buffer.ToString();
                    //}
                }

                Console.WriteLine(sw.ElapsedMilliseconds);

                foreach (StepTokenKind kind in Enum.GetValues(typeof(StepTokenKind)))
                {
                    Console.WriteLine($"{kind}: {tokens[(int)kind]}");
                }

                Console.WriteLine($"total: {tokens.Sum()}");
            }

        }

        [DataTestMethod]
        [DataRow(@"C:\ifc\PRACA INZYNIERSKA.ifc")]
        public void StepParser_ShouldReadFile(string path)
        {
            int bytes = 0;
            using (var file = new FastBinaryReader(path))
            {
                while (file.Peek() != -1)
                {
                    file.Read();
                    bytes++;
                }
            }
        }



        [DataTestMethod]
        [DataRow(@"C:\ifc\ESAPROBIMDEMO.ifc")]
        public void StepParser_ShouldParseIfc(string path)
        {
            var parser = new StepEntityParser();

            parser.Parse(path);
        }
    }
}
