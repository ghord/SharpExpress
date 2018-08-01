using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpExpress.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Test
{
    [TestClass]
    public class BufferedPeekEnumeratorFixture
    {
        [TestMethod]
        public void BufferedPeekEnumerator_ShouldReadBuffer()
        {
            const int n = 997;
            IEnumerator<ArraySegment<int>> Yielder()
            {
                int[] buffer = new int[10];

                for(int i=0; i< n; i++)
                {
                    buffer[i % 10] = i;

                    if (i % 10 == 9)
                        yield return new ArraySegment<int>(buffer, 0, 10);
                }
            }

            var enumerator = new BufferedPeekEnumerator<int>(Yielder());

            int result = 0;
            while(enumerator.MoveNext())
            {
                Assert.AreEqual(result, enumerator.Current);
                result++;
            }

            Assert.AreEqual(n, result);
            
        }
    }
}
