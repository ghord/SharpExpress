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
    public class TypeDeclarationFixture
    {
        [TestMethod]
        public void SchemaParser_ShouldParseSimpleType()
        {
            var schema = @"
SCHEMA IFC4;

TYPE IfcStrippedOptional = BOOLEAN;
END_TYPE;

END_SCHEMA;";

            var parser = new ExpressParser();
            var result = parser.Parse(schema);

            Assert.IsTrue(result.Success, result.GetSummary());

            var ifc = result.Schemas.Single();

            Assert.AreEqual(1, ifc.Declarations.Count);

            var decl = ifc.Declarations.OfType<TypeDeclaration>().Single();

            Assert.AreEqual("IfcStrippedOptional", decl.Name, true);
            Assert.AreEqual(SyntaxNodeKind.BooleanDataType, decl.UnderlyingType.Kind);
        }

        [TestMethod]
        public void SchemaParser_ShouldParseCustomType()
        {
            var schema = @"
SCHEMA IFC4;

TYPE IfcLabel = STRING;

END_TYPE;

TYPE IfcBoxAlignment = IfcLabel;
END_TYPE;

END_SCHEMA;";

            var parser = new ExpressParser();
            var result = parser.Parse(schema);

            Assert.IsTrue(result.Success, result.GetSummary());

            var ifc = result.Schemas.Single();

            Assert.AreEqual(1, ifc.Declarations.Count);

            var decl = ifc.Declarations.Single();

            Assert.Inconclusive("TODO");
        }

        [TestMethod]
        public void SchemaParser_ShouldParseAggregationType()
        {
            var schema = @" 
SCHEMA IFC4;

TYPE IfcPositiveInteger = integer;
END_TYPE;

TYPE IfcArcIndex = LIST [3:3] OF IfcPositiveInteger;
END_TYPE;

END_SCHEMA;
";

            var parser = new ExpressParser();
            var result = parser.Parse(schema);

            Assert.IsTrue(result.Success, result.GetSummary());

            var ifc = result.Schemas.Single();

            Assert.AreEqual(1, ifc.Declarations.Count);

            var decl = ifc.Declarations.Single();

            Assert.AreEqual("IfcArcIndex", decl.Name, true);
            Assert.Inconclusive("TODO");
            //TODO: aggregation types
            //Assert.AreEqual("LIST [3:3] OF IfcPositiveInteger", ((TypeDeclaration)decl).UnderlyingType.Value, true);
        }

        [TestMethod]
        public void SchemaParser_ShouldParseArrayDataType()
        {
            var schema = @"
SCHEMA IFC4;

TYPE IfcComplexNumber = ARRAY [1:2] OF REAL;
END_TYPE;

END_SCHEMA;";

            var parser = new ExpressParser();
            var result = parser.Parse(schema);

            Assert.IsTrue(result.Success, result.GetSummary());
        }

        [TestMethod]
        public void SchemaParser_ShouldNotThrowOnInvalidFile()
        {
            var schema = @"
SCHEMA IFC4;

TYPE IfcArcIndex = LIST [3:3] OF REAL;
END_TYPE;
";

            var parser = new ExpressParser();
            var result = parser.Parse(schema);

            Assert.IsFalse(result.Success);
        }
    }
}
