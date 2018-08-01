using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class TokenizerFixture
    {
        private void Verify(string input, params Token[] tokens)
        {
            var errors = new List<ParsingError>();
            var result = Tokenizer.Tokenize(new StringReader(input), errors).ToArray();

            for (int i = 0; i < Math.Max(tokens.Length, result.Length); i++)
            {
                if (tokens.Length > i && result.Length > i)
                {
                    Assert.AreEqual(tokens[i].Kind, result[i].Kind, "at " + result[i].Span.Start.ToString());
                    Assert.AreEqual(tokens[i].Text, result[i].Text, true);

                    if (!tokens[i].Span.Start.IsUnknown)
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    Assert.Fail("Expected different number of tokens");
                }
            }

            if (errors.Count > 0)
                Assert.Fail("Too much errors");
        }

        [TestMethod]
        public void Tokenizer_ShouldGetTokens()
        {
            var input = @"
SCHEMA TEST;

END_SCHEMA;
";
            Verify(input,
                Token.FromKeyword(Keywords.Schema),
                new Token("TEST", TokenKind.SimpleId),
                Token.Semicolon,
                Token.FromKeyword(Keywords.EndSchema),
                Token.Semicolon,
                Token.Eof);
        }

        [TestMethod]
        public void Tokenizer_ShouldSkipEmbeddedRemarks()
        {
            var input = @"(* test *)
SCHEMA TEST(* test *);
(* test *)
END_SCHEMA;(* test *)
";
            Verify(input,
               Token.FromKeyword(Keywords.Schema),
               new Token("TEST", TokenKind.SimpleId),
               Token.Semicolon,
               Token.FromKeyword(Keywords.EndSchema),
               Token.Semicolon,
               Token.Eof);
        }

        [TestMethod]
        public void Tokenizer_ShouldSkipNestedRemarks()
        {
            var input = @"(* (* (*test *) *) *)
SCHEMA TEST(* test *);

(* test *)
END_SCHEMA;(* test *)
";
            Verify(input,
               Token.FromKeyword(Keywords.Schema),
               new Token("TEST", TokenKind.SimpleId),
               Token.Semicolon,
               Token.FromKeyword(Keywords.EndSchema),
               Token.Semicolon,
               Token.Eof);
        }

        [TestMethod]
        public void Tokenizer_ShouldRecognizerReals()
        {
            var input = @"
1.2
0.012e+2
120.0e-2
120.e-2
";
            var token = new Token("1.2", TokenKind.RealLiteral);

            Verify(input,
                token,
                token,
                token,
                token,
                Token.Eof);
        }

        [TestMethod]
        public void Tokenizer_ShouldTokenizeCompositeTokensWithColon()
        {
            var input = @"::<>::=::= :<> :<";

            Verify(input,
                Token.Colon,
                Token.InstanceNotEqual,
                Token.InstanceEqual,
                Token.Assignment,
                Token.Colon,
                Token.NotEqual,
                Token.Colon,
                Token.LessThan,
                Token.Eof);
        }

        [TestMethod]
        public void Tokenizer_ShouldSkipTailRemarks()
        {
            var input = @"
+
-- this is a tail remark
-
-- this is a tail remark";
            Verify(input,
                Token.Plus,
                Token.Minus,
                Token.Eof);
        }

        [TestMethod]
        public void Tokenizer_ShouldTokenizeBinaryLiterals()
        {
            var input = @"%1011101010111";

            Verify(input,
                new Token("1011101010111", TokenKind.BinaryLiteral),
                Token.Eof);
        }

        [TestMethod]
        public void Tokenizer_ShouldTokenizeEncodedStringLiteral()
        {
            var input = @"""000000c5""";

            Verify(input,
                new Token("Å", TokenKind.EncodedStringLiteral),
                Token.Eof);
        }

        [DataTestMethod]
        [DataRow("ifc4_add2")]
        [DataRow("10303-203-aim-long")]
        [DataRow("10303-214e3-aim-long")]
        [DataRow("stepnc_arm_merged")]
        [DataRow("stepnc_e2_aim_20111206")]
        [DataRow("mim")]
        [DataRow("wg12n3251")]
        public void Tokenizer_ShouldTokenizeExpWithoutErrors(string file)
        {
            using (var stream = File.Open($@"..\..\..\..\test\{file}.exp", FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                var errors = new List<ParsingError>();
                var tokens = Tokenizer.Tokenize(reader, errors).ToArray();

                foreach (var error in errors)
                {
                    Console.WriteLine(error.ToString());
                }

                Span? lastSpan = null;
                foreach (var token in tokens)
                {
                    if (token == Token.Eof)
                        break;

                    Assert.IsTrue(token.Span.Start < token.Span.End);
                    if (lastSpan.HasValue)
                    {
                        Assert.IsTrue(token.Span.Start >= lastSpan.Value.End);
                    }

                    lastSpan = token.Span;
                }

                Assert.AreEqual(0, errors.Count);
            }
        }
    }
}
