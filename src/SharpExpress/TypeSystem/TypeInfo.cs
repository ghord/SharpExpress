using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.TypeSystem
{
    public abstract class TypeInfo : ISymbolInfo
    {
        protected TypeInfo(string name, ISymbolInfo declaringSymbol)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (declaringSymbol == null)
                throw new ArgumentNullException(nameof(declaringSymbol));

            Name = name;
            DeclaringSymbol = declaringSymbol;
        }

        protected TypeInfo(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public string Name { get; }

        public virtual bool IsBuiltIn => false;
        public virtual bool IsDefinedType => false;
        public virtual bool IsEntityType => false;
        public virtual bool IsEnumerationType => false;
        public virtual bool IsSelectType => false;

        public virtual bool IsAggregateType => false;
        public virtual bool IsAggregateUnique=> false;
        public virtual bool IsAggregateOptional => false;
        public virtual int? LowerBound => null;
        public virtual int? UpperBound => null;

        public virtual TypeInfo UnderlyingType => null;
        public virtual TypeInfo ElementType => null;

        public virtual IReadOnlyList<EnumerationInfo> EnumerationItems => Array.Empty<EnumerationInfo>();
        public virtual IReadOnlyList<AttributeInfo> Attributes => Array.Empty<AttributeInfo>();
        public virtual IReadOnlyList<TypeInfo> BaseTypes => Array.Empty<TypeInfo>();

        public IEnumerable<AttributeInfo> GetAttributes(bool inhertied)
        {
            if (!IsEntityType)
                return Enumerable.Empty<AttributeInfo>();

            throw new NotImplementedException();
        }

        public AttributeInfo GetAttribute(string name, bool inherited)
        {
            if (!IsEntityType)
                return null;

            throw new NotImplementedException();
        }

        public ISymbolInfo DeclaringSymbol { get; }

        public static TypeInfo Null { get; } = new NullTypeInfo();
        public static TypeInfo Integer { get; } = new IntegerTypeInfo();
        public static TypeInfo String { get; } = new StringTypeInfo();
        public static TypeInfo Real { get; } = new RealTypeInfo();
        public static TypeInfo Binary { get; } = new BinaryTypeInfo();
        public static TypeInfo Boolean { get; } = new BooleanTypeInfo();
        public static TypeInfo Logical { get; } = new LogicalTypeInfo();
        public static TypeInfo Number { get; } = new NumberTypeInfo();
        public static TypeInfo Generic { get; } = new GenericTypeInfo();
        public static TypeInfo GenericEntity { get; } = new GenericEntityTypeInfo();

        public abstract bool IsSpecializationOf(TypeInfo type);

        public bool IsGeneralizationOf(TypeInfo type)
        {
            return type.IsSpecializationOf(this);
        }

        public bool IsCompatibleWith(TypeInfo type)
        {
            if (type == this)
                return true;

            if (IsSpecializationOf(type) || type.IsSpecializationOf(this))
                return true;

            if(type.ElementType != null)
            {
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
    }

    internal sealed class BooleanTypeInfo : TypeInfo
    {
        public BooleanTypeInfo() : base("boolean")
        {

        }

        public override bool IsBuiltIn => true;

        public override bool IsSpecializationOf(TypeInfo type)
        {
            if (type == Logical)
                return true;

            return false;
        }
    }

    internal sealed class LogicalTypeInfo : TypeInfo
    {
        public LogicalTypeInfo() : base("logical")
        {
        }

        public override bool IsSpecializationOf(TypeInfo type)
        {
            return false;
        }
    }

    internal sealed class GenericTypeInfo : TypeInfo
    {
        public GenericTypeInfo() : base("generic")
        {

        }

        public override bool IsSpecializationOf(TypeInfo type)
        {
            return false;
        }
    }

    internal sealed class GenericEntityTypeInfo : TypeInfo
    {
        public GenericEntityTypeInfo() : base("generic_entity")
        {
        }

        public override bool IsSpecializationOf(TypeInfo type)
        {
            if (type == Generic)
                return true;

            return false;
        }
    }

    internal sealed class BinaryTypeInfo : TypeInfo
    {
        public BinaryTypeInfo() : base("binary")
        {

        }

        public BinaryTypeInfo(int width, bool @fixed) : base("binary")
        {
            Width = width;
            IsFixed = @fixed;
        }

        public int? Width { get; }

        public bool IsFixed { get; }

        public override bool IsSpecializationOf(TypeInfo type)
        {
            if (type == Binary)
                return true;

            throw new NotImplementedException();
        }

        public override bool IsBuiltIn => true;
    }

    internal sealed class RealTypeInfo : TypeInfo
    {
        public RealTypeInfo() : base("real")
        {
        }

        public RealTypeInfo(int precision) : base("real")
        {
            Precision = precision;
        }

        public int? Precision { get; }

        public override bool IsSpecializationOf(TypeInfo type)
        {
            if (type == Number)
                return true;

            throw new NotImplementedException();
        }

        public override bool IsBuiltIn => true;
    }

    internal sealed class StringTypeInfo : TypeInfo
    {
        public StringTypeInfo() : base("string")
        {
        }

        public StringTypeInfo(int width, bool @fixed) : base("string")
        {
            Width = width;
            IsFixed = @fixed;
        }

        public int? Width { get; }

        public bool IsFixed { get; }

        public override bool IsBuiltIn => true;

        public override bool IsSpecializationOf(TypeInfo type)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class NullTypeInfo : TypeInfo
    {
        public NullTypeInfo() : base("null")
        {
        }

        public override bool IsBuiltIn => true;

        public override bool IsSpecializationOf(TypeInfo type)
        {
            return false;
        }
    }

    internal sealed class IntegerTypeInfo : TypeInfo
    {
        public IntegerTypeInfo() : base("integer")
        {
        }

        public override bool IsBuiltIn => true;

        public override bool IsSpecializationOf(TypeInfo type)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class NumberTypeInfo : TypeInfo
    {
        public NumberTypeInfo() : base("number")
        {
        }

        public override bool IsBuiltIn => true;

        public override bool IsSpecializationOf(TypeInfo type)
        {
            return false;
        }
    }

  

    internal abstract class AggregationBaseTypeInfo : TypeInfo
    {
        private TypeInfo elementType_;

        public AggregationBaseTypeInfo(string name, bool unique, bool optional) : base(name)
        {
            IsAggregateUnique = unique;
            IsAggregateOptional = optional;
        }

        public override bool IsSpecializationOf(TypeInfo type)
        {
            throw new NotImplementedException();
        }

        internal void SetElementType(TypeInfo elementType)
        {
            elementType_ = elementType;
        }

        public override bool IsAggregateType => true;
        public override bool IsAggregateOptional { get; }
        public override bool IsAggregateUnique { get; }
   
        public override bool IsBuiltIn => true;

        public override TypeInfo ElementType => elementType_;
    }

    internal sealed class AggregateTypeInfo : AggregationBaseTypeInfo
    {
        public AggregateTypeInfo() : base("aggregate", false, false)
        {

        }
    }

    internal sealed class ArrayTypeInfo : AggregationBaseTypeInfo
    {
        public ArrayTypeInfo(bool unique, bool optional) 
            : base("array", unique, optional)
        {

        }
    }

    internal sealed class ListTypeInfo : AggregationBaseTypeInfo
    {
        public ListTypeInfo(bool unique) 
            : base("list", unique, false)
        {

        }
    }

    internal sealed class BagTypeInfo : AggregationBaseTypeInfo
    {
        public BagTypeInfo()
            : base("bag", false, false)
        {

        }
    }

    internal sealed class SetTypeInfo : AggregationBaseTypeInfo
    {
        public SetTypeInfo()
            : base("set", false, false)
        {

        }
    }

    internal sealed class DefinedTypeInfo : TypeInfo
    {
        private TypeInfo underlyingType_;

        public DefinedTypeInfo(string fullName, ISymbolInfo declaringSymbol) : base(fullName, declaringSymbol)
        {
          
        }

        public override TypeInfo UnderlyingType => underlyingType_;

        public override TypeInfo ElementType => UnderlyingType.ElementType;
        public override bool IsEnumerationType => UnderlyingType.IsEnumerationType;
        public override bool IsSelectType => UnderlyingType.IsSelectType;
        public override bool IsDefinedType => true;

        public override IReadOnlyList<EnumerationInfo> EnumerationItems => UnderlyingType.EnumerationItems;

        public void SetUnderlyingType(TypeInfo underlyingType)
        {
            if (underlyingType == null)
                throw new ArgumentNullException(nameof(underlyingType));

            underlyingType_ = underlyingType;
        }

        public override bool IsSpecializationOf(TypeInfo type)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class SelectTypeInfo : TypeInfo
    {
        private List<TypeInfo> selectTypes_ = new List<TypeInfo>();

        public SelectTypeInfo(ISymbolInfo declaringSymbol) : base("select", declaringSymbol)
        {
        }

        public override bool IsSpecializationOf(TypeInfo type)
        {
            throw new NotImplementedException();
        }

        public void SetSelectTypesCount(int count)
        {
            if(selectTypes_.Count < count)
            {
                selectTypes_.AddRange(new TypeInfo[count - selectTypes_.Count]);
            }
        }

        public void AddSelectType(int index, TypeInfo selectType)
        {
            selectTypes_[index] = selectType;
        }
    }

    internal sealed class EnumerationTypeInfo : TypeInfo
    {
        private List<EnumerationInfo> items_ = new List<EnumerationInfo>();

        public EnumerationTypeInfo(ISymbolInfo declaringSymbol) : base("enumeration", declaringSymbol)
        {
            
        }

        public override IReadOnlyList<EnumerationInfo> EnumerationItems => items_;

        public override bool IsSpecializationOf(TypeInfo type)
        {
            throw new NotImplementedException();
        }

        public override bool IsEnumerationType => true;

        internal void AddEnumeration(EnumerationInfo enumerationInfo)
        {
            if (enumerationInfo.DeclaringType != this)
                throw new InvalidOperationException();

            items_.Add(enumerationInfo);
        }
    }
}
