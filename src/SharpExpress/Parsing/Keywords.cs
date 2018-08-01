using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    /// <summary>
    /// Complete list of keyowrds in express schema file
    /// </summary>
    /// <remarks>
    /// TODO: separate keyword and builtin procedures/functions/constants as they can be redeclared freely
    /// </remarks>
    static class Keywords
    {
        public const string Abs = "abs";
        public const string Acos = "acos";
        public const string Abstract = "abstract";
        public const string Aggregate = "aggregate";
        public const string Alias = "alias";
        public const string And = "and";
        public const string AndOr = "andor";
        public const string Array = "array";
        public const string As = "as";
        public const string Asin = "asin";
        public const string Atan = "atan";
        public const string Bag = "bag";
        public const string BasedOn = "based_on";
        public const string Begin = "begin";
        public const string Binary = "binary";
        public const string Blength = "blength";
        public const string Boolean = "boolean";
        public const string By = "by";
        public const string Case = "case";
        public const string Constant = "constant";
        public const string ConstE = "const_e";
        public const string Cos = "cos";
        public const string Derive = "derive";
        public const string Div = "div";
        public const string Else = "else";
        public const string End = "end";
        public const string EndAlias = "end_alias";
        public const string EndCase = "end_case";
        public const string EndConstant = "end_constant";
        public const string EndEntity = "end_entity";
        public const string EndFunction = "end_function";
        public const string EndIf = "end_if";
        public const string EndLocal = "end_local";
        public const string EndProcedure = "end_procedure";
        public const string EndRepeat = "end_repeat";
        public const string EndRule = "end_rule";
        public const string EndSchema = "end_schema";
        public const string EndSubtypeConstraint = "end_subtype_constraint";
        public const string EndType = "end_type";
        public const string Entity = "entity";
        public const string Enumeration = "enumeration";
        public const string Escape = "escape";
        public const string Exists = "exists";
        public const string Extensible = "extensible";
        public const string Exp = "exp";
        public const string False = "false";
        public const string Fixed = "fixed";
        public const string For = "for";
        public const string Format = "format";
        public const string From = "from";
        public const string Function = "function";
        public const string Generic = "generic";
        public const string GenericEntity = "generic_entity";
        public const string Hibound = "hibound";
        public const string Hiindex = "hiindex";
        public const string If = "if";
        public const string In = "in";
        public const string Insert = "insert";
        public const string Integer = "integer";
        public const string Inverse = "inverse";
        public const string Length = "length";
        public const string Like = "like";
        public const string List = "list";
        public const string Lobound = "lobound";
        public const string Local = "local";
        public const string Log = "log";
        public const string Log10 = "log10";
        public const string Log2 = "log2";
        public const string Logical = "logical";
        public const string Loindex = "loindex";
        public const string Mod = "mod";
        public const string Not = "not";
        public const string Number = "number";
        public const string Nvl = "nvl";
        public const string Odd = "odd";
        public const string Of = "of";
        public const string Oneof = "oneof";
        public const string Optional = "optional";
        public const string Or = "or";
        public const string Otherwise = "otherwise";
        public const string Pi = "pi";
        public const string Procedure = "procedure";
        public const string Query = "query";
        public const string Real = "real";
        public const string Reference = "reference";
        public const string Remove = "remove";
        public const string Renamed = "renamed";
        public const string Repeat = "repeat";
        public const string Return = "return";
        public const string Rolesof = "rolesof";
        public const string Rule = "rule";
        public const string Schema = "schema";
        public const string Select = "select";
        public const string Self = "self";
        public const string Set = "set";
        public const string Sin = "sin";
        public const string Sizeof = "sizeof";
        public const string Skip = "skip";
        public const string Sqrt = "sqrt";
        public const string String = "string";
        public const string Subtype = "subtype";
        public const string SubtypeConstraint = "subtype_constraint";
        public const string Supertype = "supertype";
        public const string Tan = "tan";
        public const string Then = "then";
        public const string To = "to";
        public const string TotalOver = "total_over";
        public const string True = "true";
        public const string Type = "type";
        public const string Typeof = "typeof";
        public const string Unique = "unique";
        public const string Unknown = "unknown";
        public const string Until = "until";
        public const string Use = "use";
        public const string Usedin = "usedin";
        public const string Value = "value";
        public const string ValueIn = "value_in";
        public const string ValueUnique = "value_unique";
        public const string Var = "var";
        public const string Where = "where";
        public const string While = "while";
        public const string With = "with";
        public const string Xor = "xor";

        private static HashSet<string> builtinFunctions_ = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                Abs, Acos, Asin, Atan,
                Blength, Cos, Exists, Exp,
                Format, Hibound, Hiindex, Length,
                Lobound, Log, Log2, Log10,
                Loindex, Nvl, Odd, Rolesof,
                Sin, Sizeof, Sqrt, Tan,
                Typeof, Usedin, Value, ValueIn,
                ValueUnique
            };


        private static HashSet<string> builtinConstants_ = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "?",
                Self,
                ConstE,
                Pi,
                False,
                True,
                Unknown
            };

        private static Lazy<HashSet<string>> keywordLookup_;

        static Keywords()
        {
            keywordLookup_ = new Lazy<HashSet<string>>(CreateKeywordLookup);
        }

        /// <summary>
        /// Returns a collection of all keywords
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetKeywords()
        {
            return keywordLookup_.Value;
        }

        /// <summary>
        /// Returns a collection of all built-in functions
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetBuiltInFunctions()
        {
            return builtinFunctions_;
        }

        /// <summary>
        /// Returns a collection of all built-in contsants
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetBuiltInConstants()
        {
            return builtinConstants_;
        }

        public static IEnumerable<string> GetBuiltInProcedures()
        {
            return new[] { Insert, Remove };
        }

        private static HashSet<string> CreateKeywordLookup()
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var fields = typeof(Keywords).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var constField in fields.Where(f => f.IsLiteral && !f.IsInitOnly))
            {
                result.Add((string)constField.GetValue(null));
            }

            return result;
        }

        public static bool IsKeyword(string text)
        {
            return keywordLookup_.Value.Contains(text);
        }

        public static bool IsBuiltInFunction(string text)
        {
            return builtinFunctions_.Contains(text);
        }

        public static bool IsBuiltInConstant(string text)
        {
            return builtinConstants_.Contains(text);
        }

        public static bool IsBuiltInProcedure(string text)
        {
            return text.Equals(Insert, StringComparison.OrdinalIgnoreCase)
                || text.Equals(Remove, StringComparison.OrdinalIgnoreCase);
        }
    }
}
