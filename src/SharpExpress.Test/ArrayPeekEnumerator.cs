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
    public class ArrayPeekEnumeratorFixture
    {
        [TestMethod]
        public void ArrayPeekEnumerator_ShouldWorkForEmptyEnumerator()
        {
            var data = new int[0];
            var enumerator = new ArrayPeekEnumerator<int>(data);

            Assert.IsFalse(enumerator.TryPeek(out _));
            Assert.IsFalse(enumerator.TryPeek2(out _, out _));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.IsFalse(enumerator.TryPeek(out _));
            Assert.IsFalse(enumerator.TryPeek2(out _, out _));
        }

        [TestMethod]
        public void ArrayPeekEnumerator_ShouldWorkForOneElement()
        {
            var data = new int[] { 1 };
            var enumerator = new ArrayPeekEnumerator<int>(data);

            Assert.IsTrue(enumerator.TryPeek(out var first));
            Assert.AreEqual(1, first);

            Assert.IsFalse(enumerator.TryPeek2(out _, out _));

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);

            Assert.IsFalse(enumerator.TryPeek2(out _, out _));
            Assert.IsFalse(enumerator.TryPeek(out _));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void ArrayPeekEnumerator_ShouldWorkForTwoElements()
        {
            var data = new int[] { 1, 2 };
            var enumerator = new ArrayPeekEnumerator<int>(data);

            Assert.IsTrue(enumerator.TryPeek(out var first));
            Assert.AreEqual(1, first);

            Assert.IsTrue(enumerator.TryPeek2(out first, out var second));
            Assert.AreEqual(1, first);
            Assert.AreEqual(2, second);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);

            Assert.IsTrue(enumerator.TryPeek(out second));
            Assert.IsFalse(enumerator.TryPeek2(out _, out _));
            Assert.AreEqual(2, second);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);

            Assert.IsFalse(enumerator.TryPeek(out _));
            Assert.IsFalse(enumerator.MoveNext());
        }


    }
}
