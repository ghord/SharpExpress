using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpExpress.Analysis;
using SharpExpress.Builders;
using SharpExpress.Parsing;
using SharpExpress.Test.Utilities;
using SharpExpress.TypeSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Test
{
    [TestClass]
    public class IfcFixture
    {
        private const string IfcFilePath = @"..\..\..\..\test\ifc4_add2.exp";

        [TestMethod]
        public void Ifc_Tokenizer_ShouldTokenizeIfcFile()
        {
            using (var stream = File.Open(IfcFilePath, FileMode.Open, FileAccess.Read))
            {
                var errors = new List<ParsingError>();
                var tokens = Tokenizer.Tokenize(new StreamReader(stream, Encoding.ASCII), errors);

                foreach(var error in errors)
                {
                    Console.WriteLine(error.ToString());
                }

                Assert.AreEqual(0, errors.Count, errors.FirstOrDefault()?.ToString());
            }
        }

        [TestMethod]
        public void Ifc_DeclarationPass_ShouldParseIfcFile()
        {
            using (var stream = File.Open(@"..\..\..\..\test\ifc4_add2.exp", FileMode.Open, FileAccess.Read))
            {
                var tokens = Tokenizer.Tokenize(new StreamReader(stream, Encoding.ASCII)).ToArray();
                var builder = new SyntaxTreeBuilder();
                var errors = new List<ParsingError>();
                var pass = new DeclarationPass(tokens, errors);
                pass.Run(builder);

                foreach (var error in errors)
                {
                    Console.WriteLine(error.ToString());
                }

                Assert.AreEqual(0, errors.Count, errors.FirstOrDefault()?.ToString());
            }
        }

        [TestMethod]
        public void Ifc_SyntaxAnalysisPass_ShouldParseIfcFile()
        {
            using (var stream = File.Open(@"..\..\..\..\test\ifc4_add2.exp", FileMode.Open, FileAccess.Read))
            {
                var tokens = Tokenizer.Tokenize(new StreamReader(stream, Encoding.ASCII)).ToArray();
                var builder = new SyntaxTreeBuilder();
                var declarationPass = new DeclarationPass(tokens);
                declarationPass.Run(builder);

                var errors = new List<ParsingError>();
                var pass = new SyntaxAnalysisPass(tokens, errors);
                pass.Run(builder);

                foreach (var error in errors)
                {
                    Console.WriteLine(error.ToString());
                }

                Assert.AreEqual(0, errors.Count, errors.FirstOrDefault()?.ToString());
            }
        }


        [DataTestMethod]
        [DataRow(@"..\..\..\..\test\IFC4_ADD2.exp")]
        [DataRow(@"..\..\..\..\test\IFC2X3_TC1.exp")]
        public void Ifc_SchemaParser_ShouldParseIfcFile(string path)
        {
            var parser = new ExpressParser();

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                var result = parser.Parse(stream);

                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.ToString());
                }

                Assert.AreEqual(0, result.Errors.Length, result.Errors.FirstOrDefault()?.ToString());
            }
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\..\test\IFC4_ADD2.exp")]
        [DataRow(@"..\..\..\..\test\IFC2X3_TC1.exp")]
        public void Ifc_SemanticModel_ShouldCreateSymbolTable(string path)
        {
            var parser = new ExpressParser();

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                var result = parser.Parse(stream);

                Assert.IsTrue(result.Success);

                var builder = new SymbolTableBuilder(result.Schemas);
                var symbolTable = builder.Build();

                var declarations = new DeclarationSyntaxWalker();
                foreach (var schema in result.Schemas)
                    schema.Accept(declarations);

                foreach(var declaration in declarations.Declarations)
                {
                    if (declaration is LocalRuleDeclaration)
                        continue;

                    var symbolInfo = symbolTable.GetSymbolInfo(declaration);

                    Assert.IsNotNull(symbolInfo, 
                        $"Missing ISymbolInfo for {declaration.Name}({declaration.Kind})");

                    ValidateSymbolInfo(symbolInfo);
                }
            }
        }

        private void ValidateSymbolInfo(ISymbolInfo symbolInfo)
        {
            switch (symbolInfo)
            {
                case AttributeInfo attributeInfo:
                    ValidateAttributeInfo(attributeInfo);
                    break;
                case TypeInfo typeInfo:
                    ValidateTypeInfo(typeInfo);
                    break;
                case FunctionInfo functionInfo:
                    ValidateFunctionInfo(functionInfo);
                    break;
                case ParameterInfo parameterInfo:
                    ValidateParameterInfo(parameterInfo);
                    break;
               
                    
                default:
                    break;
            }
        }

        private void ValidateParameterInfo(ParameterInfo parameterInfo)
        {
            Assert.IsNotNull(parameterInfo.Type, $"Parameter {parameterInfo.DeclaringSymbol.Name}.{parameterInfo.Name} has no type");
        }

        private void ValidateFunctionInfo(FunctionInfo functionInfo)
        {
            Assert.IsNotNull(functionInfo.ReturnType, $"Function {functionInfo.Name} has no return type");
                
        }

        private void ValidateTypeInfo(TypeInfo typeInfo)
        {
            if(typeInfo.IsAggregateType)
            {
                Assert.IsNotNull(typeInfo.ElementType, $"Aggregate type {typeInfo.Name} has no elementType");
            }

            if(typeInfo.IsDefinedType)
            {
                Assert.IsNotNull(typeInfo.UnderlyingType, $"Defined type {typeInfo.Name} has no underlyingType");
            }
        }

        private void ValidateAttributeInfo(AttributeInfo attributeInfo)
        {
            Assert.IsNotNull(attributeInfo.Type, $"Attribute {attributeInfo.DeclaringType.Name}.{attributeInfo.Name} has no type");
        }
    }
}
