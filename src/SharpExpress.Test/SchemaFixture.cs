using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpExpress.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Test
{
    [TestClass]
    public class SchemaFixture 
    {
        [TestMethod]
        public void SchemaParser_ShouldParseEmptySchema()
        {
            var input = @"
SCHEMA TEST;

END_SCHEMA;
";
            var parser = new ExpressParser();
            var result = parser.Parse(input);

            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(1, result.Schemas.Length);
            Assert.AreEqual("test", result.Schemas[0].Name, true);
        }

        [TestMethod]
        public void SchemaParser_ShouldParseSchemaVersionId()
        {
            var input = @"
SCHEMA TEST 'Test version id';

END_SCHEMA;";

            var parser = new ExpressParser();
            var result = parser.Parse(input);

            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(1, result.Schemas.Length);
            Assert.AreEqual("Test version id", result.Schemas[0].SchemaVersionId);
        }

        [TestMethod]
        public void SchenaParser_ShouldNotThrowOnInvalidInput()
        {
            var intput = @"..\..\..\..\test\IFC4_ADD2.exp";
        }
    }
}
