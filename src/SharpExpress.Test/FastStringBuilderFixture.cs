using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpExpress.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Test
{
    [TestClass]
    public class FastStringBuilderFixture 
    {
        [TestMethod]
        public void FastStringBuilder_ShouldParseDouble()
        {
            var builder = new FastStringBuilder();

            builder.Append("1.2e-17");

            Assert.AreEqual(1.2e-17, builder.ParseDouble());
        }


        [TestMethod]
        public void FastStringBuilder_ShouldParseInt()
        {
            var builder = new FastStringBuilder();

            builder.Append("1233321");

            Assert.AreEqual(1233321, builder.ParseInt());

            builder.Clear();

            builder.Append("+1235321");

            Assert.AreEqual(1235321, builder.ParseInt());

            builder.Clear();

            builder.Append("-1234321");

            Assert.AreEqual(-1234321, builder.ParseInt());
        }

        [TestMethod]
        public void FastStirngBuilder_ShouldEqualsWork()
        {
            var builder = new FastStringBuilder();

            builder.Append("Test");

            Assert.IsTrue(builder.Equals("Test"));
            Assert.IsFalse(builder.Equals("Tes"));
            Assert.IsFalse(builder.Equals("Test1"));
        }
    }
}
