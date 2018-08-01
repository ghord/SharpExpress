using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.TypeSystem
{
    sealed class EntityInfo : TypeInfo, IDeclaringSymbolInfo
    {
        private List<AttributeInfo> attributes_ = new List<AttributeInfo>();
        private List<TypeInfo> baseTypes_ = new List<TypeInfo>();

        internal EntityInfo(string fullName, ISymbolInfo declaringSymbol) : base(fullName, declaringSymbol)
        {
        }

        internal void AddAttribute(AttributeInfo attribute)
        {
            if (attribute.DeclaringType != this)
                throw new InvalidOperationException();

            attributes_.Add(attribute);
        }

        internal void SetBaseTypeCount(int count)
        {
            if(count > baseTypes_.Count)
            {
                baseTypes_.AddRange(new TypeInfo[count - baseTypes_.Count]);
            }
        }

        internal void SetBaseType(int index, TypeInfo typeInfo)
        {
            baseTypes_[index] = typeInfo;
        }

        public override IReadOnlyList<TypeInfo> BaseTypes => baseTypes_;
    
        public override bool IsSpecializationOf(TypeInfo type)
        {
            throw new NotImplementedException();
        }

        void IDeclaringSymbolInfo.AddDeclaration(ISymbolInfo declaration)
        {
            switch (declaration)
            {
                case AttributeInfo attributeInfo:
                    AddAttribute(attributeInfo);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
