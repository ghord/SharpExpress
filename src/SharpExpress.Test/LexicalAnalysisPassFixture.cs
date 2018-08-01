using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpExpress.Builders;
using SharpExpress.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Test
{
    [TestClass]
    public class LexicalAnalysisPassFixture
    {
        [TestMethod]
        public void DeclarationCollectingPass_ShouldCollectSchemaDeclarations()
        {
//            var input = @"
//SCHEMA TEST;

//END_SCHEMA;

//SCHEMA TEST2;

//END_SCHEMA;
//";
//            var tokens = Tokenizer.Tokenize(new StringReader(input));
//            var pass = new DeclarationCollectingPass(tokens);

//            var scope = pass.Run();

//            Assert.AreEqual(2, scope.Identifiers.Count);
//            Assert.IsTrue(scope.Identifiers.ContainsKey("test"));
//            Assert.AreEqual(IdentifierKind.Schema, scope.Identifiers["test"]);
//            Assert.IsTrue(scope.Identifiers.ContainsKey("test2"));
//            Assert.AreEqual(IdentifierKind.Schema, scope.Identifiers["test2"]);

        }

        [DataTestMethod]
        [DataRow("ifc4_add2")]
        [DataRow("10303-203-aim-long")]
        [DataRow("10303-214e3-aim-long")]
        [DataRow("stepnc_arm_merged")]
        [DataRow("stepnc_e2_aim_20111206")]
        [DataRow("mim")]
        [DataRow("wg12n3251")]
        public void DeclarationCollectingPass_ShouldPassTestCasesWithoutErrors(string fileName)
        {
            var parser = new ExpressParser();

            using (var stream = File.Open($@"..\..\..\..\test\{fileName}.exp", FileMode.Open, FileAccess.Read))
            {
                var errors = new List<ParsingError>();

                var tokens = Tokenizer.Tokenize(new StreamReader(stream, Encoding.ASCII)).ToArray();
                var declarationPass = new DeclarationPass(tokens, errors);
                var syntaxPass = new SyntaxAnalysisPass(tokens, errors);
                var builder = new SyntaxTreeBuilder();
                declarationPass.Run(builder);
                syntaxPass.Run(builder);

                foreach(var error in errors)
                {
                    Console.WriteLine(error.ToString());
                }

                Assert.AreEqual(0, errors.Count);
            }
        }
    }
}
