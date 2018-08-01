namespace SharpExpress.TypeSystem
{
    public interface ISymbolInfo
    {
        string Name { get; }
        ISymbolInfo DeclaringSymbol { get; }
    }

    internal interface IDeclaringSymbolInfo : ISymbolInfo
    {
        void AddDeclaration(ISymbolInfo declaration);
    }

    public static class SymbolInfoExtensions
    {
        public static string GetFullName(this ISymbolInfo symbolInfo)
        {
            if (symbolInfo.DeclaringSymbol != null)
                return symbolInfo.DeclaringSymbol.GetFullName() + "." + symbolInfo.Name;

            return symbolInfo.Name;
        }
    }

}