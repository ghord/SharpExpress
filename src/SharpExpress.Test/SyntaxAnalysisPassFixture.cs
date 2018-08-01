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
    public class SyntaxAnalysisPassFixture
    {
        [TestMethod]
        public void ExpressParser_ShouldParseNestedQueryScopes()
        {
            var input = @"
SCHEMA TEST;

ENTITY Test;
	Voids : SET [1:?] OF ARRAY [0:10] OF REAL;
 WHERE
	VoidsHaveAdvancedFaces : SIZEOF (QUERY (Vsh <* Voids |
  SIZEOF (QUERY (Afs <* Vsh |
  (NOT ('IFC4.IFCADVANCEDFACE' IN TYPEOF(Afs)))
  )) = 0
)) = 0;
END_ENTITY;

END_SCHEMA;
";

            var parser = new ExpressParser();

            var result = parser.Parse(input);

            Assert.IsTrue(result.Success, result.GetSummary());
        }
    }
}
