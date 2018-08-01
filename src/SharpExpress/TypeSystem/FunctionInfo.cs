using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.TypeSystem
{
    public abstract class AlgorithmInfo : ISymbolInfo, IDeclaringSymbolInfo
    {
        private List<ParameterInfo> parameters_ = new List<ParameterInfo>();

        internal AlgorithmInfo(string name, ISymbolInfo declaringSymbol)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (declaringSymbol == null)
                throw new ArgumentNullException(nameof(declaringSymbol));

            Name = name;
            DeclaringSymbol = declaringSymbol;
        }

        public string Name { get; }

        public ISymbolInfo DeclaringSymbol { get; }

        public IReadOnlyCollection<ParameterInfo> Parameters => parameters_;

        void IDeclaringSymbolInfo.AddDeclaration(ISymbolInfo declaration)
        {
            if (declaration.DeclaringSymbol != this)
                throw new InvalidOperationException();

            if (declaration is ParameterInfo parameterInfo)
            {
                parameters_.Add(parameterInfo);
            }
            else
                throw new NotSupportedException();
        }
    }

    public sealed class FunctionInfo : AlgorithmInfo
    {
        internal FunctionInfo(string name, ISymbolInfo declaringSymbol) : base(name, declaringSymbol)
        {
        }


        public TypeInfo ReturnType { get; internal set; }
    }
}
