using System;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace SharpExpress
{
    public enum SyntaxNodeKind
    {
        SyntaxList,
        AggregateDataType,
        ListDataType,
        ArrayDataType,
        BagDataType,
        SetDataType,
        EnumerationDataType,
        SelectDataType,
        ReferenceDataType,
        StringDataType,
        BinaryDataType,
        RealDataType,
        BooleanDataType,
        LogicalDataType,
        IntegerDataType,
        NumberDataType,
        GenericDataType,
        GenericEntityDataType,
        SchemaDeclaration,
        TypeDeclaration,
        ExplicitAttributeDeclaration,
        InverseAttributeDeclaration,
        DerivedAttributeDeclaration,
        EntityDeclaration,
        ConstantDeclaration,
        EnumerationDeclaration,
        ProcedureDeclaration,
        FunctionDeclaration,
        RuleDeclaration,
        DomainRuleDeclaration,
        UniqueRuleDeclaration,
        ParameterDeclaration,
        VariableDeclaration,
        SubtypeConstraintDeclaration,
        TypeLabelDeclaration,
        LessThanExpression,
        GreaterThanExpression,
        LessThanOrEqualExpression,
        GreaterThanOrEqualExpression,
        NotEqualExpression,
        EqualExpression,
        InstanceNotEqualExpression,
        InstanceEqualExpression,
        InExpression,
        AdditionExpression,
        SubtractionExpression,
        OrExpression,
        XorExpression,
        MultiplicationExpression,
        DivisionExpression,
        IntegerDivisionExpression,
        ModuloExpression,
        AndExpression,
        ExponentiationExpression,
        LikeExpression,
        ComplexEntityConstructionExpression,
        UnaryPlusExpression,
        UnaryMinusExpression,
        NotExpression,
        StringLiteral,
        IntegerLiteral,
        RealLiteral,
        BinaryLiteral,
        AggregateInitializerExpression,
        AggregateInitializerElement,
        QueryExpression,
        IntervalExpression,
        EntityConstructorExpression,
        EnumerationReferenceExpression,
        ConstantReferenceExpression,
        ParameterReferenceExpression,
        EntityReferenceExpression,
        VariableReferenceExpression,
        FunctionCallExpression,
        QualifiedExpression,
        AttributeReferenceExpression,
        NullStatement,
        AliasStatement,
        AssignmentStatement,
        CaseStatement,
        CaseAction,
        CompoundStatement,
        IfStatement,
        ProcedureCallStatement,
        RepeatStatement,
        ReturnStatement,
        SkipStatement,
        EscapeStatement,
        QualifiedReference,
        AttributeQualifier,
        GroupQualifier,
        IndexQualifier,
        SchemaReference,
        TypeReference,
        EntityReference,
        AttributeReference,
        UnresolvedReference,
        ParameterReference,
        VariableReference,
        EnumerationReference,
        ConstantReference,
        FunctionReference,
        ProcedureReference,
        TypeLabelReference,
        Bounds,
        AndTypeExpression,
        AndOrTypeExpression,
        OneOfTypeExpression,
        EntityReferenceTypeExpression,
        UseClause,
        ReferenceClause,
        RenamedReference,
    }

    public abstract partial class DataType : SyntaxNode, IDataType
    {
        internal DataType(SyntaxNodeKind kind)
            : base(kind)
        {
        }
    }

    public abstract partial class AggregationDataType : DataType, IAggregationDataType
    {
        internal AggregationDataType(SyntaxNodeKind kind, DataType elementType, Bounds bounds)
            : base(kind)
        {
            ElementType = elementType;
            Bounds = bounds;
        }
        public DataType ElementType { get; }

        public Bounds Bounds { get; }


        IDataType IAggregationDataType.ElementType => ElementType;

        IBounds IAggregationDataType.Bounds => Bounds;
    }

    public sealed partial class AggregateDataType : AggregationDataType, IAggregateDataType
    {
        public AggregateDataType(DataType elementType, TypeLabelReference typeLabel)
            : base(SyntaxNodeKind.AggregateDataType, elementType, null)
        {
            TypeLabel = typeLabel;
        }
        public TypeLabelReference TypeLabel { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return ElementType;
                case 1: return Bounds;
                case 2: return TypeLabel;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 3;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAggregateDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitAggregateDataType(this);
        }

        ITypeLabelReference IAggregateDataType.TypeLabel => TypeLabel;
    }

    public sealed partial class ListDataType : AggregationDataType, IListDataType
    {
        public ListDataType(DataType elementType, Bounds bounds, bool isUnique)
            : base(SyntaxNodeKind.ListDataType, elementType, bounds)
        {
            IsUnique = isUnique;
        }
        public bool IsUnique { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return ElementType;
                case 1: return Bounds;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitListDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitListDataType(this);
        }

        bool IListDataType.IsUnique => IsUnique;
    }

    public sealed partial class ArrayDataType : AggregationDataType, IArrayDataType
    {
        public ArrayDataType(DataType elementType, Bounds bounds, bool isOptional, bool isUnique)
            : base(SyntaxNodeKind.ArrayDataType, elementType, bounds)
        {
            IsOptional = isOptional;
            IsUnique = isUnique;
        }
        public bool IsOptional { get; }

        public bool IsUnique { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return ElementType;
                case 1: return Bounds;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitArrayDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitArrayDataType(this);
        }

        bool IArrayDataType.IsOptional => IsOptional;

        bool IArrayDataType.IsUnique => IsUnique;
    }

    public sealed partial class BagDataType : AggregationDataType, IBagDataType
    {
        public BagDataType(DataType elementType, Bounds bounds)
            : base(SyntaxNodeKind.BagDataType, elementType, bounds)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return ElementType;
                case 1: return Bounds;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitBagDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitBagDataType(this);
        }
    }

    public sealed partial class SetDataType : AggregationDataType, ISetDataType
    {
        public SetDataType(DataType elementType, Bounds bounds)
            : base(SyntaxNodeKind.SetDataType, elementType, bounds)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return ElementType;
                case 1: return Bounds;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitSetDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitSetDataType(this);
        }
    }

    public abstract partial class ConstructedDataType : DataType, IConstructedDataType
    {
        internal ConstructedDataType(SyntaxNodeKind kind, bool isExtensible)
            : base(kind)
        {
            IsExtensible = isExtensible;
        }
        public bool IsExtensible { get; }


        bool IConstructedDataType.IsExtensible => IsExtensible;
    }

    public sealed partial class EnumerationDataType : ConstructedDataType, IEnumerationDataType
    {
        public EnumerationDataType(bool isExtensible, SyntaxList<EnumerationDeclaration> items, TypeReference basedOn)
            : base(SyntaxNodeKind.EnumerationDataType, isExtensible)
        {
            Items = items;
            BasedOn = basedOn;
        }
        public SyntaxList<EnumerationDeclaration> Items { get; }

        public TypeReference BasedOn { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Items;
                case 1: return BasedOn;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEnumerationDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitEnumerationDataType(this);
        }

        IEnumerable<IEnumerationDeclaration> IEnumerationDataType.Items => Items;

        ITypeReference IEnumerationDataType.BasedOn => BasedOn;
    }

    public sealed partial class SelectDataType : ConstructedDataType, ISelectDataType
    {
        public SelectDataType(bool isExtensible, SyntaxList<ReferenceDataType> items, TypeReference basedOn, bool isGenericIdentity)
            : base(SyntaxNodeKind.SelectDataType, isExtensible)
        {
            Items = items;
            BasedOn = basedOn;
            IsGenericIdentity = isGenericIdentity;
        }
        public SyntaxList<ReferenceDataType> Items { get; }

        public TypeReference BasedOn { get; }

        public bool IsGenericIdentity { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Items;
                case 1: return BasedOn;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitSelectDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitSelectDataType(this);
        }

        IEnumerable<IReferenceDataType> ISelectDataType.Items => Items;

        ITypeReference ISelectDataType.BasedOn => BasedOn;

        bool ISelectDataType.IsGenericIdentity => IsGenericIdentity;
    }

    public sealed partial class ReferenceDataType : DataType, IReferenceDataType
    {
        public ReferenceDataType(Reference reference)
            : base(SyntaxNodeKind.ReferenceDataType)
        {
            Reference = reference;
        }
        public Reference Reference { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Reference;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitReferenceDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitReferenceDataType(this);
        }

        IReference IReferenceDataType.Reference => Reference;
    }

    public abstract partial class SimpleDataType : DataType, ISimpleDataType
    {
        internal SimpleDataType(SyntaxNodeKind kind)
            : base(kind)
        {
        }
    }

    public sealed partial class StringDataType : SimpleDataType, IStringDataType
    {
        public StringDataType(Expression width, bool isFixed)
            : base(SyntaxNodeKind.StringDataType)
        {
            Width = width;
            IsFixed = isFixed;
        }
        public Expression Width { get; }

        public bool IsFixed { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Width;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitStringDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitStringDataType(this);
        }

        IExpression IStringDataType.Width => Width;

        bool IStringDataType.IsFixed => IsFixed;
    }

    public sealed partial class BinaryDataType : SimpleDataType, IBinaryDataType
    {
        public BinaryDataType(Expression width, bool isFixed)
            : base(SyntaxNodeKind.BinaryDataType)
        {
            Width = width;
            IsFixed = isFixed;
        }
        public Expression Width { get; }

        public bool IsFixed { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Width;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitBinaryDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitBinaryDataType(this);
        }

        IExpression IBinaryDataType.Width => Width;

        bool IBinaryDataType.IsFixed => IsFixed;
    }

    public sealed partial class RealDataType : SimpleDataType, IRealDataType
    {
        public RealDataType(Expression precision)
            : base(SyntaxNodeKind.RealDataType)
        {
            Precision = precision;
        }
        public Expression Precision { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Precision;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitRealDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitRealDataType(this);
        }

        IExpression IRealDataType.Precision => Precision;
    }

    public sealed partial class BooleanDataType : SimpleDataType, IBooleanDataType
    {
        public BooleanDataType()
            : base(SyntaxNodeKind.BooleanDataType)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitBooleanDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitBooleanDataType(this);
        }
    }

    public sealed partial class LogicalDataType : SimpleDataType, ILogicalDataType
    {
        public LogicalDataType()
            : base(SyntaxNodeKind.LogicalDataType)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitLogicalDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitLogicalDataType(this);
        }
    }

    public sealed partial class IntegerDataType : SimpleDataType, IIntegerDataType
    {
        public IntegerDataType()
            : base(SyntaxNodeKind.IntegerDataType)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitIntegerDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitIntegerDataType(this);
        }
    }

    public sealed partial class NumberDataType : SimpleDataType, INumberDataType
    {
        public NumberDataType()
            : base(SyntaxNodeKind.NumberDataType)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitNumberDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitNumberDataType(this);
        }
    }

    public sealed partial class GenericDataType : DataType, IGenericDataType
    {
        public GenericDataType(TypeLabelReference typeLabel)
            : base(SyntaxNodeKind.GenericDataType)
        {
            TypeLabel = typeLabel;
        }
        public TypeLabelReference TypeLabel { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return TypeLabel;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitGenericDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitGenericDataType(this);
        }

        ITypeLabelReference IGenericDataType.TypeLabel => TypeLabel;
    }

    public sealed partial class GenericEntityDataType : DataType, IGenericEntityDataType
    {
        public GenericEntityDataType(TypeLabelReference typeLabel)
            : base(SyntaxNodeKind.GenericEntityDataType)
        {
            TypeLabel = typeLabel;
        }
        public TypeLabelReference TypeLabel { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return TypeLabel;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitGenericEntityDataType(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitGenericEntityDataType(this);
        }

        ITypeLabelReference IGenericEntityDataType.TypeLabel => TypeLabel;
    }

    public abstract partial class Declaration : SyntaxNode, IDeclaration
    {
        internal Declaration(SyntaxNodeKind kind, string name)
            : base(kind)
        {
            Name = name;
        }
        public string Name { get; }


        string IDeclaration.Name => Name;
    }

    public abstract partial class AlgorithmDeclaration : Declaration, IAlgorithmDeclaration
    {
        internal AlgorithmDeclaration(SyntaxNodeKind kind, string name, SyntaxList<Declaration> declarations, SyntaxList<ConstantDeclaration> constants, SyntaxList<VariableDeclaration> localVariables, SyntaxList<Statement> statements)
            : base(kind, name)
        {
            Declarations = declarations;
            Constants = constants;
            LocalVariables = localVariables;
            Statements = statements;
        }
        public SyntaxList<Declaration> Declarations { get; }

        public SyntaxList<ConstantDeclaration> Constants { get; }

        public SyntaxList<VariableDeclaration> LocalVariables { get; }

        public SyntaxList<Statement> Statements { get; }


        IEnumerable<IDeclaration> IAlgorithmDeclaration.Declarations => Declarations;

        IEnumerable<IConstantDeclaration> IAlgorithmDeclaration.Constants => Constants;

        IEnumerable<IVariableDeclaration> IAlgorithmDeclaration.LocalVariables => LocalVariables;

        IEnumerable<IStatement> IAlgorithmDeclaration.Statements => Statements;
    }

    public sealed partial class SchemaDeclaration : Declaration, ISchemaDeclaration
    {
        public SchemaDeclaration(string name, string schemaVersionId, SyntaxList<InterfaceSpecification> interfaceSpecifications, SyntaxList<ConstantDeclaration> constants, SyntaxList<Declaration> declarations)
            : base(SyntaxNodeKind.SchemaDeclaration, name)
        {
            SchemaVersionId = schemaVersionId;
            InterfaceSpecifications = interfaceSpecifications;
            Constants = constants;
            Declarations = declarations;
        }
        public string SchemaVersionId { get; }

        public SyntaxList<InterfaceSpecification> InterfaceSpecifications { get; }

        public SyntaxList<ConstantDeclaration> Constants { get; }

        public SyntaxList<Declaration> Declarations { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return InterfaceSpecifications;
                case 1: return Constants;
                case 2: return Declarations;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 3;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitSchemaDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitSchemaDeclaration(this);
        }

        string ISchemaDeclaration.SchemaVersionId => SchemaVersionId;

        IEnumerable<IInterfaceSpecification> ISchemaDeclaration.InterfaceSpecifications => InterfaceSpecifications;

        IEnumerable<IConstantDeclaration> ISchemaDeclaration.Constants => Constants;

        IEnumerable<IDeclaration> ISchemaDeclaration.Declarations => Declarations;
    }

    public sealed partial class TypeDeclaration : Declaration, ITypeDeclaration
    {
        public TypeDeclaration(string name, DataType underlyingType, SyntaxList<DomainRuleDeclaration> domainRules)
            : base(SyntaxNodeKind.TypeDeclaration, name)
        {
            UnderlyingType = underlyingType;
            DomainRules = domainRules;
        }
        public DataType UnderlyingType { get; }

        public SyntaxList<DomainRuleDeclaration> DomainRules { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return UnderlyingType;
                case 1: return DomainRules;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitTypeDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitTypeDeclaration(this);
        }

        IDataType ITypeDeclaration.UnderlyingType => UnderlyingType;

        IEnumerable<IDomainRuleDeclaration> ITypeDeclaration.DomainRules => DomainRules;
    }

    public abstract partial class AttributeDeclaration : Declaration, IAttributeDeclaration
    {
        internal AttributeDeclaration(SyntaxNodeKind kind, string name, QualifiedReference redeclaredAttribute, DataType type)
            : base(kind, name)
        {
            RedeclaredAttribute = redeclaredAttribute;
            Type = type;
        }
        public QualifiedReference RedeclaredAttribute { get; }

        public DataType Type { get; }


        IQualifiedReference IAttributeDeclaration.RedeclaredAttribute => RedeclaredAttribute;

        IDataType IAttributeDeclaration.Type => Type;
    }

    public sealed partial class ExplicitAttributeDeclaration : AttributeDeclaration, IExplicitAttributeDeclaration
    {
        public ExplicitAttributeDeclaration(string name, QualifiedReference redeclaredAttribute, DataType type, bool isOptional)
            : base(SyntaxNodeKind.ExplicitAttributeDeclaration, name, redeclaredAttribute, type)
        {
            IsOptional = isOptional;
        }
        public bool IsOptional { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return RedeclaredAttribute;
                case 1: return Type;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitExplicitAttributeDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitExplicitAttributeDeclaration(this);
        }

        bool IExplicitAttributeDeclaration.IsOptional => IsOptional;
    }

    public sealed partial class InverseAttributeDeclaration : AttributeDeclaration, IInverseAttributeDeclaration
    {
        public InverseAttributeDeclaration(string name, QualifiedReference redeclaredAttribute, DataType type, EntityReference forEntity, AttributeReference forAttribute)
            : base(SyntaxNodeKind.InverseAttributeDeclaration, name, redeclaredAttribute, type)
        {
            ForEntity = forEntity;
            ForAttribute = forAttribute;
        }
        public EntityReference ForEntity { get; }

        public AttributeReference ForAttribute { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return RedeclaredAttribute;
                case 1: return Type;
                case 2: return ForEntity;
                case 3: return ForAttribute;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 4;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitInverseAttributeDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitInverseAttributeDeclaration(this);
        }

        IEntityReference IInverseAttributeDeclaration.ForEntity => ForEntity;

        IAttributeReference IInverseAttributeDeclaration.ForAttribute => ForAttribute;
    }

    public sealed partial class DerivedAttributeDeclaration : AttributeDeclaration, IDerivedAttributeDeclaration
    {
        public DerivedAttributeDeclaration(string name, QualifiedReference redeclaredAttribute, DataType type, Expression value)
            : base(SyntaxNodeKind.DerivedAttributeDeclaration, name, redeclaredAttribute, type)
        {
            Value = value;
        }
        public Expression Value { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return RedeclaredAttribute;
                case 1: return Type;
                case 2: return Value;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 3;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitDerivedAttributeDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitDerivedAttributeDeclaration(this);
        }

        IExpression IDerivedAttributeDeclaration.Value => Value;
    }

    public sealed partial class EntityDeclaration : Declaration, IEntityDeclaration
    {
        public EntityDeclaration(string name, bool isAbstract, bool isAbstractSupertype, SyntaxList<EntityReference> supertypes, TypeExpression subtype, SyntaxList<ExplicitAttributeDeclaration> explicitAttributes, SyntaxList<DerivedAttributeDeclaration> derivedAttributes, SyntaxList<InverseAttributeDeclaration> inverseAttributes, SyntaxList<UniqueRuleDeclaration> uniqueRules, SyntaxList<DomainRuleDeclaration> domainRules)
            : base(SyntaxNodeKind.EntityDeclaration, name)
        {
            IsAbstract = isAbstract;
            IsAbstractSupertype = isAbstractSupertype;
            Supertypes = supertypes;
            Subtype = subtype;
            ExplicitAttributes = explicitAttributes;
            DerivedAttributes = derivedAttributes;
            InverseAttributes = inverseAttributes;
            UniqueRules = uniqueRules;
            DomainRules = domainRules;
        }
        public bool IsAbstract { get; }

        public bool IsAbstractSupertype { get; }

        public SyntaxList<EntityReference> Supertypes { get; }

        public TypeExpression Subtype { get; }

        public SyntaxList<ExplicitAttributeDeclaration> ExplicitAttributes { get; }

        public SyntaxList<DerivedAttributeDeclaration> DerivedAttributes { get; }

        public SyntaxList<InverseAttributeDeclaration> InverseAttributes { get; }

        public SyntaxList<UniqueRuleDeclaration> UniqueRules { get; }

        public SyntaxList<DomainRuleDeclaration> DomainRules { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Supertypes;
                case 1: return Subtype;
                case 2: return ExplicitAttributes;
                case 3: return DerivedAttributes;
                case 4: return InverseAttributes;
                case 5: return UniqueRules;
                case 6: return DomainRules;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 7;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEntityDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitEntityDeclaration(this);
        }

        bool IEntityDeclaration.IsAbstract => IsAbstract;

        bool IEntityDeclaration.IsAbstractSupertype => IsAbstractSupertype;

        IEnumerable<IEntityReference> IEntityDeclaration.Supertypes => Supertypes;

        ITypeExpression IEntityDeclaration.Subtype => Subtype;

        IEnumerable<IExplicitAttributeDeclaration> IEntityDeclaration.ExplicitAttributes => ExplicitAttributes;

        IEnumerable<IDerivedAttributeDeclaration> IEntityDeclaration.DerivedAttributes => DerivedAttributes;

        IEnumerable<IInverseAttributeDeclaration> IEntityDeclaration.InverseAttributes => InverseAttributes;

        IEnumerable<IUniqueRuleDeclaration> IEntityDeclaration.UniqueRules => UniqueRules;

        IEnumerable<IDomainRuleDeclaration> IEntityDeclaration.DomainRules => DomainRules;
    }

    public sealed partial class ConstantDeclaration : Declaration, IConstantDeclaration
    {
        public ConstantDeclaration(string name, DataType type, Expression value)
            : base(SyntaxNodeKind.ConstantDeclaration, name)
        {
            Type = type;
            Value = value;
        }
        public DataType Type { get; }

        public Expression Value { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Type;
                case 1: return Value;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitConstantDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitConstantDeclaration(this);
        }

        IDataType IConstantDeclaration.Type => Type;

        IExpression IConstantDeclaration.Value => Value;
    }

    public sealed partial class EnumerationDeclaration : Declaration, IEnumerationDeclaration
    {
        public EnumerationDeclaration(string name)
            : base(SyntaxNodeKind.EnumerationDeclaration, name)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEnumerationDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitEnumerationDeclaration(this);
        }
    }

    public sealed partial class ProcedureDeclaration : AlgorithmDeclaration, IProcedureDeclaration
    {
        public ProcedureDeclaration(string name, SyntaxList<Declaration> declarations, SyntaxList<ConstantDeclaration> constants, SyntaxList<VariableDeclaration> localVariables, SyntaxList<Statement> statements, SyntaxList<ParameterDeclaration> parameters)
            : base(SyntaxNodeKind.ProcedureDeclaration, name, declarations, constants, localVariables, statements)
        {
            Parameters = parameters;
        }
        public SyntaxList<ParameterDeclaration> Parameters { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Declarations;
                case 1: return Constants;
                case 2: return LocalVariables;
                case 3: return Statements;
                case 4: return Parameters;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 5;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitProcedureDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitProcedureDeclaration(this);
        }

        IEnumerable<IParameterDeclaration> IProcedureDeclaration.Parameters => Parameters;
    }

    public sealed partial class FunctionDeclaration : AlgorithmDeclaration, IFunctionDeclaration
    {
        public FunctionDeclaration(string name, SyntaxList<Declaration> declarations, SyntaxList<ConstantDeclaration> constants, SyntaxList<VariableDeclaration> localVariables, SyntaxList<Statement> statements, SyntaxList<ParameterDeclaration> parameters, DataType returnType)
            : base(SyntaxNodeKind.FunctionDeclaration, name, declarations, constants, localVariables, statements)
        {
            Parameters = parameters;
            ReturnType = returnType;
        }
        public SyntaxList<ParameterDeclaration> Parameters { get; }

        public DataType ReturnType { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Declarations;
                case 1: return Constants;
                case 2: return LocalVariables;
                case 3: return Statements;
                case 4: return Parameters;
                case 5: return ReturnType;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 6;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitFunctionDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitFunctionDeclaration(this);
        }

        IEnumerable<IParameterDeclaration> IFunctionDeclaration.Parameters => Parameters;

        IDataType IFunctionDeclaration.ReturnType => ReturnType;
    }

    public sealed partial class RuleDeclaration : AlgorithmDeclaration, IRuleDeclaration
    {
        public RuleDeclaration(string name, SyntaxList<Declaration> declarations, SyntaxList<ConstantDeclaration> constants, SyntaxList<VariableDeclaration> localVariables, SyntaxList<Statement> statements, SyntaxList<VariableDeclaration> populations, SyntaxList<DomainRuleDeclaration> domainRules)
            : base(SyntaxNodeKind.RuleDeclaration, name, declarations, constants, localVariables, statements)
        {
            Populations = populations;
            DomainRules = domainRules;
        }
        public SyntaxList<VariableDeclaration> Populations { get; }

        public SyntaxList<DomainRuleDeclaration> DomainRules { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Declarations;
                case 1: return Constants;
                case 2: return LocalVariables;
                case 3: return Statements;
                case 4: return Populations;
                case 5: return DomainRules;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 6;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitRuleDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitRuleDeclaration(this);
        }

        IEnumerable<IVariableDeclaration> IRuleDeclaration.Populations => Populations;

        IEnumerable<IDomainRuleDeclaration> IRuleDeclaration.DomainRules => DomainRules;
    }

    public abstract partial class LocalRuleDeclaration : Declaration, ILocalRuleDeclaration
    {
        internal LocalRuleDeclaration(SyntaxNodeKind kind, string name)
            : base(kind, name)
        {
        }
    }

    public sealed partial class DomainRuleDeclaration : LocalRuleDeclaration, IDomainRuleDeclaration
    {
        public DomainRuleDeclaration(string name, Expression expression)
            : base(SyntaxNodeKind.DomainRuleDeclaration, name)
        {
            Expression = expression;
        }
        public Expression Expression { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Expression;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitDomainRuleDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitDomainRuleDeclaration(this);
        }

        IExpression IDomainRuleDeclaration.Expression => Expression;
    }

    public sealed partial class UniqueRuleDeclaration : LocalRuleDeclaration, IUniqueRuleDeclaration
    {
        public UniqueRuleDeclaration(string name, SyntaxList<Expression> expressions)
            : base(SyntaxNodeKind.UniqueRuleDeclaration, name)
        {
            Expressions = expressions;
        }
        public SyntaxList<Expression> Expressions { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Expressions;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitUniqueRuleDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitUniqueRuleDeclaration(this);
        }

        IEnumerable<IExpression> IUniqueRuleDeclaration.Expressions => Expressions;
    }

    public sealed partial class ParameterDeclaration : Declaration, IParameterDeclaration
    {
        public ParameterDeclaration(string name, DataType type, bool isVariable)
            : base(SyntaxNodeKind.ParameterDeclaration, name)
        {
            Type = type;
            IsVariable = isVariable;
        }
        public DataType Type { get; }

        public bool IsVariable { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Type;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitParameterDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitParameterDeclaration(this);
        }

        IDataType IParameterDeclaration.Type => Type;

        bool IParameterDeclaration.IsVariable => IsVariable;
    }

    public sealed partial class VariableDeclaration : Declaration, IVariableDeclaration
    {
        public VariableDeclaration(string name, DataType type, Expression initialValue)
            : base(SyntaxNodeKind.VariableDeclaration, name)
        {
            Type = type;
            InitialValue = initialValue;
        }
        public DataType Type { get; }

        public Expression InitialValue { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Type;
                case 1: return InitialValue;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitVariableDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitVariableDeclaration(this);
        }

        IDataType IVariableDeclaration.Type => Type;

        IExpression IVariableDeclaration.InitialValue => InitialValue;
    }

    public sealed partial class SubtypeConstraintDeclaration : Declaration, ISubtypeConstraintDeclaration
    {
        public SubtypeConstraintDeclaration(string name)
            : base(SyntaxNodeKind.SubtypeConstraintDeclaration, name)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitSubtypeConstraintDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitSubtypeConstraintDeclaration(this);
        }
    }

    public sealed partial class TypeLabelDeclaration : Declaration, ITypeLabelDeclaration
    {
        public TypeLabelDeclaration(string name)
            : base(SyntaxNodeKind.TypeLabelDeclaration, name)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitTypeLabelDeclaration(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitTypeLabelDeclaration(this);
        }
    }

    public abstract partial class Expression : SyntaxNode, IExpression
    {
        internal Expression(SyntaxNodeKind kind)
            : base(kind)
        {
        }
    }

    public abstract partial class BinaryExpression : Expression, IBinaryExpression
    {
        internal BinaryExpression(SyntaxNodeKind kind, Expression left, Expression right)
            : base(kind)
        {
            Left = left;
            Right = right;
        }
        public Expression Left { get; }

        public Expression Right { get; }


        IExpression IBinaryExpression.Left => Left;

        IExpression IBinaryExpression.Right => Right;
    }

    public sealed partial class LessThanExpression : BinaryExpression, ILessThanExpression
    {
        public LessThanExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.LessThanExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitLessThanExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitLessThanExpression(this);
        }
    }

    public sealed partial class GreaterThanExpression : BinaryExpression, IGreaterThanExpression
    {
        public GreaterThanExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.GreaterThanExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitGreaterThanExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitGreaterThanExpression(this);
        }
    }

    public sealed partial class LessThanOrEqualExpression : BinaryExpression, ILessThanOrEqualExpression
    {
        public LessThanOrEqualExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.LessThanOrEqualExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitLessThanOrEqualExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitLessThanOrEqualExpression(this);
        }
    }

    public sealed partial class GreaterThanOrEqualExpression : BinaryExpression, IGreaterThanOrEqualExpression
    {
        public GreaterThanOrEqualExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.GreaterThanOrEqualExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitGreaterThanOrEqualExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitGreaterThanOrEqualExpression(this);
        }
    }

    public sealed partial class NotEqualExpression : BinaryExpression, INotEqualExpression
    {
        public NotEqualExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.NotEqualExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitNotEqualExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitNotEqualExpression(this);
        }
    }

    public sealed partial class EqualExpression : BinaryExpression, IEqualExpression
    {
        public EqualExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.EqualExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEqualExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitEqualExpression(this);
        }
    }

    public sealed partial class InstanceNotEqualExpression : BinaryExpression, IInstanceNotEqualExpression
    {
        public InstanceNotEqualExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.InstanceNotEqualExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitInstanceNotEqualExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitInstanceNotEqualExpression(this);
        }
    }

    public sealed partial class InstanceEqualExpression : BinaryExpression, IInstanceEqualExpression
    {
        public InstanceEqualExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.InstanceEqualExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitInstanceEqualExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitInstanceEqualExpression(this);
        }
    }

    public sealed partial class InExpression : BinaryExpression, IInExpression
    {
        public InExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.InExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitInExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitInExpression(this);
        }
    }

    public sealed partial class AdditionExpression : BinaryExpression, IAdditionExpression
    {
        public AdditionExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.AdditionExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAdditionExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitAdditionExpression(this);
        }
    }

    public sealed partial class SubtractionExpression : BinaryExpression, ISubtractionExpression
    {
        public SubtractionExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.SubtractionExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitSubtractionExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitSubtractionExpression(this);
        }
    }

    public sealed partial class OrExpression : BinaryExpression, IOrExpression
    {
        public OrExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.OrExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitOrExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitOrExpression(this);
        }
    }

    public sealed partial class XorExpression : BinaryExpression, IXorExpression
    {
        public XorExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.XorExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitXorExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitXorExpression(this);
        }
    }

    public sealed partial class MultiplicationExpression : BinaryExpression, IMultiplicationExpression
    {
        public MultiplicationExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.MultiplicationExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitMultiplicationExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitMultiplicationExpression(this);
        }
    }

    public sealed partial class DivisionExpression : BinaryExpression, IDivisionExpression
    {
        public DivisionExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.DivisionExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitDivisionExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitDivisionExpression(this);
        }
    }

    public sealed partial class IntegerDivisionExpression : BinaryExpression, IIntegerDivisionExpression
    {
        public IntegerDivisionExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.IntegerDivisionExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitIntegerDivisionExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitIntegerDivisionExpression(this);
        }
    }

    public sealed partial class ModuloExpression : BinaryExpression, IModuloExpression
    {
        public ModuloExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.ModuloExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitModuloExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitModuloExpression(this);
        }
    }

    public sealed partial class AndExpression : BinaryExpression, IAndExpression
    {
        public AndExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.AndExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAndExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitAndExpression(this);
        }
    }

    public sealed partial class ExponentiationExpression : BinaryExpression, IExponentiationExpression
    {
        public ExponentiationExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.ExponentiationExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitExponentiationExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitExponentiationExpression(this);
        }
    }

    public sealed partial class LikeExpression : BinaryExpression, ILikeExpression
    {
        public LikeExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.LikeExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitLikeExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitLikeExpression(this);
        }
    }

    public sealed partial class ComplexEntityConstructionExpression : BinaryExpression, IComplexEntityConstructionExpression
    {
        public ComplexEntityConstructionExpression(Expression left, Expression right)
            : base(SyntaxNodeKind.ComplexEntityConstructionExpression, left, right)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitComplexEntityConstructionExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitComplexEntityConstructionExpression(this);
        }
    }

    public abstract partial class UnaryExpression : Expression, IUnaryExpression
    {
        internal UnaryExpression(SyntaxNodeKind kind, Expression operand)
            : base(kind)
        {
            Operand = operand;
        }
        public Expression Operand { get; }


        IExpression IUnaryExpression.Operand => Operand;
    }

    public sealed partial class UnaryPlusExpression : UnaryExpression, IUnaryPlusExpression
    {
        public UnaryPlusExpression(Expression operand)
            : base(SyntaxNodeKind.UnaryPlusExpression, operand)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Operand;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitUnaryPlusExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitUnaryPlusExpression(this);
        }
    }

    public sealed partial class UnaryMinusExpression : UnaryExpression, IUnaryMinusExpression
    {
        public UnaryMinusExpression(Expression operand)
            : base(SyntaxNodeKind.UnaryMinusExpression, operand)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Operand;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitUnaryMinusExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitUnaryMinusExpression(this);
        }
    }

    public sealed partial class NotExpression : UnaryExpression, INotExpression
    {
        public NotExpression(Expression operand)
            : base(SyntaxNodeKind.NotExpression, operand)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Operand;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitNotExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitNotExpression(this);
        }
    }

    public abstract partial class LiteralExpression : Expression, ILiteralExpression
    {
        internal LiteralExpression(SyntaxNodeKind kind)
            : base(kind)
        {
        }
    }

    public sealed partial class StringLiteral : LiteralExpression, IStringLiteral
    {
        public StringLiteral(string value)
            : base(SyntaxNodeKind.StringLiteral)
        {
            Value = value;
        }
        public string Value { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitStringLiteral(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitStringLiteral(this);
        }

        string IStringLiteral.Value => Value;
    }

    public sealed partial class IntegerLiteral : LiteralExpression, IIntegerLiteral
    {
        public IntegerLiteral(int value)
            : base(SyntaxNodeKind.IntegerLiteral)
        {
            Value = value;
        }
        public int Value { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitIntegerLiteral(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitIntegerLiteral(this);
        }

        int IIntegerLiteral.Value => Value;
    }

    public sealed partial class RealLiteral : LiteralExpression, IRealLiteral
    {
        public RealLiteral(double value)
            : base(SyntaxNodeKind.RealLiteral)
        {
            Value = value;
        }
        public double Value { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitRealLiteral(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitRealLiteral(this);
        }

        double IRealLiteral.Value => Value;
    }

    public sealed partial class BinaryLiteral : LiteralExpression, IBinaryLiteral
    {
        public BinaryLiteral(byte[] value)
            : base(SyntaxNodeKind.BinaryLiteral)
        {
            Value = value;
        }
        public byte[] Value { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitBinaryLiteral(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitBinaryLiteral(this);
        }

        byte[] IBinaryLiteral.Value => Value;
    }

    public sealed partial class AggregateInitializerExpression : Expression, IAggregateInitializerExpression
    {
        public AggregateInitializerExpression(SyntaxList<AggregateInitializerElement> elements)
            : base(SyntaxNodeKind.AggregateInitializerExpression)
        {
            Elements = elements;
        }
        public SyntaxList<AggregateInitializerElement> Elements { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Elements;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAggregateInitializerExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitAggregateInitializerExpression(this);
        }

        IEnumerable<IAggregateInitializerElement> IAggregateInitializerExpression.Elements => Elements;
    }

    public sealed partial class AggregateInitializerElement : SyntaxNode, IAggregateInitializerElement
    {
        public AggregateInitializerElement(Expression expression, Expression repetition)
            : base(SyntaxNodeKind.AggregateInitializerElement)
        {
            Expression = expression;
            Repetition = repetition;
        }
        public Expression Expression { get; }

        public Expression Repetition { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Expression;
                case 1: return Repetition;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAggregateInitializerElement(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitAggregateInitializerElement(this);
        }

        IExpression IAggregateInitializerElement.Expression => Expression;

        IExpression IAggregateInitializerElement.Repetition => Repetition;
    }

    public sealed partial class QueryExpression : Expression, IQueryExpression
    {
        public QueryExpression(VariableDeclaration variableDeclaration, Expression aggregate, Expression condition)
            : base(SyntaxNodeKind.QueryExpression)
        {
            VariableDeclaration = variableDeclaration;
            Aggregate = aggregate;
            Condition = condition;
        }
        public VariableDeclaration VariableDeclaration { get; }

        public Expression Aggregate { get; }

        public Expression Condition { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return VariableDeclaration;
                case 1: return Aggregate;
                case 2: return Condition;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 3;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitQueryExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitQueryExpression(this);
        }

        IVariableDeclaration IQueryExpression.VariableDeclaration => VariableDeclaration;

        IExpression IQueryExpression.Aggregate => Aggregate;

        IExpression IQueryExpression.Condition => Condition;
    }

    public sealed partial class IntervalExpression : Expression, IIntervalExpression
    {
        public IntervalExpression(Expression low, IntervalComparison lowComparison, Expression item, IntervalComparison highComparison, Expression high)
            : base(SyntaxNodeKind.IntervalExpression)
        {
            Low = low;
            LowComparison = lowComparison;
            Item = item;
            HighComparison = highComparison;
            High = high;
        }
        public Expression Low { get; }

        public IntervalComparison LowComparison { get; }

        public Expression Item { get; }

        public IntervalComparison HighComparison { get; }

        public Expression High { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Low;
                case 1: return Item;
                case 2: return High;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 3;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitIntervalExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitIntervalExpression(this);
        }

        IExpression IIntervalExpression.Low => Low;

        IntervalComparison IIntervalExpression.LowComparison => LowComparison;

        IExpression IIntervalExpression.Item => Item;

        IntervalComparison IIntervalExpression.HighComparison => HighComparison;

        IExpression IIntervalExpression.High => High;
    }

    public sealed partial class EntityConstructorExpression : Expression, IEntityConstructorExpression
    {
        public EntityConstructorExpression(EntityReference entity, SyntaxList<Expression> paramaters)
            : base(SyntaxNodeKind.EntityConstructorExpression)
        {
            Entity = entity;
            Paramaters = paramaters;
        }
        public EntityReference Entity { get; }

        public SyntaxList<Expression> Paramaters { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Entity;
                case 1: return Paramaters;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEntityConstructorExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitEntityConstructorExpression(this);
        }

        IEntityReference IEntityConstructorExpression.Entity => Entity;

        IEnumerable<IExpression> IEntityConstructorExpression.Paramaters => Paramaters;
    }

    public sealed partial class EnumerationReferenceExpression : Expression, IEnumerationReferenceExpression
    {
        public EnumerationReferenceExpression(TypeReference type, EnumerationReference enumeration)
            : base(SyntaxNodeKind.EnumerationReferenceExpression)
        {
            Type = type;
            Enumeration = enumeration;
        }
        public TypeReference Type { get; }

        public EnumerationReference Enumeration { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Type;
                case 1: return Enumeration;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEnumerationReferenceExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitEnumerationReferenceExpression(this);
        }

        ITypeReference IEnumerationReferenceExpression.Type => Type;

        IEnumerationReference IEnumerationReferenceExpression.Enumeration => Enumeration;
    }

    public sealed partial class ConstantReferenceExpression : Expression, IConstantReferenceExpression
    {
        public ConstantReferenceExpression(ConstantReference constant)
            : base(SyntaxNodeKind.ConstantReferenceExpression)
        {
            Constant = constant;
        }
        public ConstantReference Constant { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Constant;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitConstantReferenceExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitConstantReferenceExpression(this);
        }

        IConstantReference IConstantReferenceExpression.Constant => Constant;
    }

    public sealed partial class ParameterReferenceExpression : Expression, IParameterReferenceExpression
    {
        public ParameterReferenceExpression(ParameterReference parameter)
            : base(SyntaxNodeKind.ParameterReferenceExpression)
        {
            Parameter = parameter;
        }
        public ParameterReference Parameter { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Parameter;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitParameterReferenceExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitParameterReferenceExpression(this);
        }

        IParameterReference IParameterReferenceExpression.Parameter => Parameter;
    }

    public sealed partial class EntityReferenceExpression : Expression, IEntityReferenceExpression
    {
        public EntityReferenceExpression(EntityReference entity)
            : base(SyntaxNodeKind.EntityReferenceExpression)
        {
            Entity = entity;
        }
        public EntityReference Entity { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Entity;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEntityReferenceExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitEntityReferenceExpression(this);
        }

        IEntityReference IEntityReferenceExpression.Entity => Entity;
    }

    public sealed partial class VariableReferenceExpression : Expression, IVariableReferenceExpression
    {
        public VariableReferenceExpression(VariableReference variable)
            : base(SyntaxNodeKind.VariableReferenceExpression)
        {
            Variable = variable;
        }
        public VariableReference Variable { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Variable;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitVariableReferenceExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitVariableReferenceExpression(this);
        }

        IVariableReference IVariableReferenceExpression.Variable => Variable;
    }

    public sealed partial class FunctionCallExpression : Expression, IFunctionCallExpression
    {
        public FunctionCallExpression(FunctionReference function, SyntaxList<Expression> parameters)
            : base(SyntaxNodeKind.FunctionCallExpression)
        {
            Function = function;
            Parameters = parameters;
        }
        public FunctionReference Function { get; }

        public SyntaxList<Expression> Parameters { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Function;
                case 1: return Parameters;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitFunctionCallExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitFunctionCallExpression(this);
        }

        IFunctionReference IFunctionCallExpression.Function => Function;

        IEnumerable<IExpression> IFunctionCallExpression.Parameters => Parameters;
    }

    public sealed partial class QualifiedExpression : Expression, IQualifiedExpression
    {
        public QualifiedExpression(Expression expression, SyntaxList<Qualifier> qualifiers)
            : base(SyntaxNodeKind.QualifiedExpression)
        {
            Expression = expression;
            Qualifiers = qualifiers;
        }
        public Expression Expression { get; }

        public SyntaxList<Qualifier> Qualifiers { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Expression;
                case 1: return Qualifiers;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitQualifiedExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitQualifiedExpression(this);
        }

        IExpression IQualifiedExpression.Expression => Expression;

        IEnumerable<IQualifier> IQualifiedExpression.Qualifiers => Qualifiers;
    }

    public sealed partial class AttributeReferenceExpression : Expression, IAttributeReferenceExpression
    {
        public AttributeReferenceExpression(AttributeReference attribute)
            : base(SyntaxNodeKind.AttributeReferenceExpression)
        {
            Attribute = attribute;
        }
        public AttributeReference Attribute { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Attribute;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAttributeReferenceExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitAttributeReferenceExpression(this);
        }

        IAttributeReference IAttributeReferenceExpression.Attribute => Attribute;
    }

    public abstract partial class Statement : SyntaxNode, IStatement
    {
        internal Statement(SyntaxNodeKind kind)
            : base(kind)
        {
        }
    }

    public sealed partial class NullStatement : Statement, INullStatement
    {
        public NullStatement()
            : base(SyntaxNodeKind.NullStatement)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitNullStatement(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitNullStatement(this);
        }
    }

    public sealed partial class AliasStatement : Statement, IAliasStatement
    {
        public AliasStatement(VariableDeclaration variable, QualifiedReference renamedReference)
            : base(SyntaxNodeKind.AliasStatement)
        {
            Variable = variable;
            RenamedReference = renamedReference;
        }
        public VariableDeclaration Variable { get; }

        public QualifiedReference RenamedReference { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Variable;
                case 1: return RenamedReference;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAliasStatement(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitAliasStatement(this);
        }

        IVariableDeclaration IAliasStatement.Variable => Variable;

        IQualifiedReference IAliasStatement.RenamedReference => RenamedReference;
    }

    public sealed partial class AssignmentStatement : Statement, IAssignmentStatement
    {
        public AssignmentStatement(QualifiedReference left, Expression right)
            : base(SyntaxNodeKind.AssignmentStatement)
        {
            Left = left;
            Right = right;
        }
        public QualifiedReference Left { get; }

        public Expression Right { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAssignmentStatement(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitAssignmentStatement(this);
        }

        IQualifiedReference IAssignmentStatement.Left => Left;

        IExpression IAssignmentStatement.Right => Right;
    }

    public sealed partial class CaseStatement : Statement, ICaseStatement
    {
        public CaseStatement(Expression selector, SyntaxList<CaseAction> actions, Statement defaultAction)
            : base(SyntaxNodeKind.CaseStatement)
        {
            Selector = selector;
            Actions = actions;
            DefaultAction = defaultAction;
        }
        public Expression Selector { get; }

        public SyntaxList<CaseAction> Actions { get; }

        public Statement DefaultAction { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Selector;
                case 1: return Actions;
                case 2: return DefaultAction;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 3;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitCaseStatement(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitCaseStatement(this);
        }

        IExpression ICaseStatement.Selector => Selector;

        IEnumerable<ICaseAction> ICaseStatement.Actions => Actions;

        IStatement ICaseStatement.DefaultAction => DefaultAction;
    }

    public sealed partial class CaseAction : Statement, ICaseAction
    {
        public CaseAction(SyntaxList<Expression> labels, Statement statement)
            : base(SyntaxNodeKind.CaseAction)
        {
            Labels = labels;
            Statement = statement;
        }
        public SyntaxList<Expression> Labels { get; }

        public Statement Statement { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Labels;
                case 1: return Statement;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitCaseAction(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitCaseAction(this);
        }

        IEnumerable<IExpression> ICaseAction.Labels => Labels;

        IStatement ICaseAction.Statement => Statement;
    }

    public sealed partial class CompoundStatement : Statement, ICompoundStatement
    {
        public CompoundStatement(SyntaxList<Statement> statements)
            : base(SyntaxNodeKind.CompoundStatement)
        {
            Statements = statements;
        }
        public SyntaxList<Statement> Statements { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Statements;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitCompoundStatement(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitCompoundStatement(this);
        }

        IEnumerable<IStatement> ICompoundStatement.Statements => Statements;
    }

    public sealed partial class IfStatement : Statement, IIfStatement
    {
        public IfStatement(Expression condition, SyntaxList<Statement> statements, SyntaxList<Statement> elseStatements)
            : base(SyntaxNodeKind.IfStatement)
        {
            Condition = condition;
            Statements = statements;
            ElseStatements = elseStatements;
        }
        public Expression Condition { get; }

        public SyntaxList<Statement> Statements { get; }

        public SyntaxList<Statement> ElseStatements { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Condition;
                case 1: return Statements;
                case 2: return ElseStatements;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 3;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitIfStatement(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitIfStatement(this);
        }

        IExpression IIfStatement.Condition => Condition;

        IEnumerable<IStatement> IIfStatement.Statements => Statements;

        IEnumerable<IStatement> IIfStatement.ElseStatements => ElseStatements;
    }

    public sealed partial class ProcedureCallStatement : Statement, IProcedureCallStatement
    {
        public ProcedureCallStatement(ProcedureReference procedure, SyntaxList<Expression> parameters)
            : base(SyntaxNodeKind.ProcedureCallStatement)
        {
            Procedure = procedure;
            Parameters = parameters;
        }
        public ProcedureReference Procedure { get; }

        public SyntaxList<Expression> Parameters { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Procedure;
                case 1: return Parameters;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitProcedureCallStatement(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitProcedureCallStatement(this);
        }

        IProcedureReference IProcedureCallStatement.Procedure => Procedure;

        IEnumerable<IExpression> IProcedureCallStatement.Parameters => Parameters;
    }

    public sealed partial class RepeatStatement : Statement, IRepeatStatement
    {
        public RepeatStatement(VariableDeclaration incrementVariable, Expression incrementFrom, Expression incrementTo, Expression incrementStep, Expression whileCondition, Expression untilCondition, SyntaxList<Statement> statements)
            : base(SyntaxNodeKind.RepeatStatement)
        {
            IncrementVariable = incrementVariable;
            IncrementFrom = incrementFrom;
            IncrementTo = incrementTo;
            IncrementStep = incrementStep;
            WhileCondition = whileCondition;
            UntilCondition = untilCondition;
            Statements = statements;
        }
        public VariableDeclaration IncrementVariable { get; }

        public Expression IncrementFrom { get; }

        public Expression IncrementTo { get; }

        public Expression IncrementStep { get; }

        public Expression WhileCondition { get; }

        public Expression UntilCondition { get; }

        public SyntaxList<Statement> Statements { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return IncrementVariable;
                case 1: return IncrementFrom;
                case 2: return IncrementTo;
                case 3: return IncrementStep;
                case 4: return WhileCondition;
                case 5: return UntilCondition;
                case 6: return Statements;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 7;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitRepeatStatement(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitRepeatStatement(this);
        }

        IVariableDeclaration IRepeatStatement.IncrementVariable => IncrementVariable;

        IExpression IRepeatStatement.IncrementFrom => IncrementFrom;

        IExpression IRepeatStatement.IncrementTo => IncrementTo;

        IExpression IRepeatStatement.IncrementStep => IncrementStep;

        IExpression IRepeatStatement.WhileCondition => WhileCondition;

        IExpression IRepeatStatement.UntilCondition => UntilCondition;

        IEnumerable<IStatement> IRepeatStatement.Statements => Statements;
    }

    public sealed partial class ReturnStatement : Statement, IReturnStatement
    {
        public ReturnStatement(Expression expression)
            : base(SyntaxNodeKind.ReturnStatement)
        {
            Expression = expression;
        }
        public Expression Expression { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Expression;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitReturnStatement(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitReturnStatement(this);
        }

        IExpression IReturnStatement.Expression => Expression;
    }

    public sealed partial class SkipStatement : Statement, ISkipStatement
    {
        public SkipStatement()
            : base(SyntaxNodeKind.SkipStatement)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitSkipStatement(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitSkipStatement(this);
        }
    }

    public sealed partial class EscapeStatement : Statement, IEscapeStatement
    {
        public EscapeStatement()
            : base(SyntaxNodeKind.EscapeStatement)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEscapeStatement(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitEscapeStatement(this);
        }
    }

    public sealed partial class QualifiedReference : SyntaxNode, IQualifiedReference
    {
        public QualifiedReference(Reference reference, SyntaxList<Qualifier> qualifiers)
            : base(SyntaxNodeKind.QualifiedReference)
        {
            Reference = reference;
            Qualifiers = qualifiers;
        }
        public Reference Reference { get; }

        public SyntaxList<Qualifier> Qualifiers { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Reference;
                case 1: return Qualifiers;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitQualifiedReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitQualifiedReference(this);
        }

        IReference IQualifiedReference.Reference => Reference;

        IEnumerable<IQualifier> IQualifiedReference.Qualifiers => Qualifiers;
    }

    public abstract partial class Qualifier : SyntaxNode, IQualifier
    {
        internal Qualifier(SyntaxNodeKind kind)
            : base(kind)
        {
        }
    }

    public sealed partial class AttributeQualifier : Qualifier, IAttributeQualifier
    {
        public AttributeQualifier(AttributeReference attribute)
            : base(SyntaxNodeKind.AttributeQualifier)
        {
            Attribute = attribute;
        }
        public AttributeReference Attribute { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Attribute;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAttributeQualifier(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitAttributeQualifier(this);
        }

        IAttributeReference IAttributeQualifier.Attribute => Attribute;
    }

    public sealed partial class GroupQualifier : Qualifier, IGroupQualifier
    {
        public GroupQualifier(EntityReference entity)
            : base(SyntaxNodeKind.GroupQualifier)
        {
            Entity = entity;
        }
        public EntityReference Entity { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Entity;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitGroupQualifier(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitGroupQualifier(this);
        }

        IEntityReference IGroupQualifier.Entity => Entity;
    }

    public sealed partial class IndexQualifier : Qualifier, IIndexQualifier
    {
        public IndexQualifier(Expression from, Expression to)
            : base(SyntaxNodeKind.IndexQualifier)
        {
            From = from;
            To = to;
        }
        public Expression From { get; }

        public Expression To { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return From;
                case 1: return To;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitIndexQualifier(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitIndexQualifier(this);
        }

        IExpression IIndexQualifier.From => From;

        IExpression IIndexQualifier.To => To;
    }

    public abstract partial class Reference : SyntaxNode, IReference
    {
        internal Reference(SyntaxNodeKind kind)
            : base(kind)
        {
        }
    }

    public sealed partial class SchemaReference : Reference, ISchemaReference
    {
        public SchemaReference(string schemaName)
            : base(SyntaxNodeKind.SchemaReference)
        {
            SchemaName = schemaName;
        }
        public string SchemaName { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitSchemaReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitSchemaReference(this);
        }

        string ISchemaReference.SchemaName => SchemaName;
    }

    public sealed partial class TypeReference : Reference, ITypeReference
    {
        public TypeReference(string typeName)
            : base(SyntaxNodeKind.TypeReference)
        {
            TypeName = typeName;
        }
        public string TypeName { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitTypeReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitTypeReference(this);
        }

        string ITypeReference.TypeName => TypeName;
    }

    public sealed partial class EntityReference : Reference, IEntityReference
    {
        public EntityReference(string entityName)
            : base(SyntaxNodeKind.EntityReference)
        {
            EntityName = entityName;
        }
        public string EntityName { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEntityReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitEntityReference(this);
        }

        string IEntityReference.EntityName => EntityName;
    }

    public sealed partial class AttributeReference : Reference, IAttributeReference
    {
        public AttributeReference(string attributeName)
            : base(SyntaxNodeKind.AttributeReference)
        {
            AttributeName = attributeName;
        }
        public string AttributeName { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAttributeReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitAttributeReference(this);
        }

        string IAttributeReference.AttributeName => AttributeName;
    }

    public sealed partial class UnresolvedReference : Reference, IUnresolvedReference
    {
        public UnresolvedReference(string unresolvedName)
            : base(SyntaxNodeKind.UnresolvedReference)
        {
            UnresolvedName = unresolvedName;
        }
        public string UnresolvedName { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitUnresolvedReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitUnresolvedReference(this);
        }

        string IUnresolvedReference.UnresolvedName => UnresolvedName;
    }

    public sealed partial class ParameterReference : Reference, IParameterReference
    {
        public ParameterReference(string parameterName)
            : base(SyntaxNodeKind.ParameterReference)
        {
            ParameterName = parameterName;
        }
        public string ParameterName { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitParameterReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitParameterReference(this);
        }

        string IParameterReference.ParameterName => ParameterName;
    }

    public sealed partial class VariableReference : Reference, IVariableReference
    {
        public VariableReference(string variableName)
            : base(SyntaxNodeKind.VariableReference)
        {
            VariableName = variableName;
        }
        public string VariableName { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitVariableReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitVariableReference(this);
        }

        string IVariableReference.VariableName => VariableName;
    }

    public sealed partial class EnumerationReference : Reference, IEnumerationReference
    {
        public EnumerationReference(string enumerationName)
            : base(SyntaxNodeKind.EnumerationReference)
        {
            EnumerationName = enumerationName;
        }
        public string EnumerationName { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEnumerationReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitEnumerationReference(this);
        }

        string IEnumerationReference.EnumerationName => EnumerationName;
    }

    public sealed partial class ConstantReference : Reference, IConstantReference
    {
        public ConstantReference(string constantName)
            : base(SyntaxNodeKind.ConstantReference)
        {
            ConstantName = constantName;
        }
        public string ConstantName { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitConstantReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitConstantReference(this);
        }

        string IConstantReference.ConstantName => ConstantName;
    }

    public sealed partial class FunctionReference : Reference, IFunctionReference
    {
        public FunctionReference(string functionName)
            : base(SyntaxNodeKind.FunctionReference)
        {
            FunctionName = functionName;
        }
        public string FunctionName { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitFunctionReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitFunctionReference(this);
        }

        string IFunctionReference.FunctionName => FunctionName;
    }

    public sealed partial class ProcedureReference : Reference, IProcedureReference
    {
        public ProcedureReference(string procedureName)
            : base(SyntaxNodeKind.ProcedureReference)
        {
            ProcedureName = procedureName;
        }
        public string ProcedureName { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitProcedureReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitProcedureReference(this);
        }

        string IProcedureReference.ProcedureName => ProcedureName;
    }

    public sealed partial class TypeLabelReference : Reference, ITypeLabelReference
    {
        public TypeLabelReference(string typeLabelName)
            : base(SyntaxNodeKind.TypeLabelReference)
        {
            TypeLabelName = typeLabelName;
        }
        public string TypeLabelName { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 0;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitTypeLabelReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitTypeLabelReference(this);
        }

        string ITypeLabelReference.TypeLabelName => TypeLabelName;
    }

    public sealed partial class Bounds : SyntaxNode, IBounds
    {
        public Bounds(Expression lowerBound, Expression upperBound)
            : base(SyntaxNodeKind.Bounds)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }
        public Expression LowerBound { get; }

        public Expression UpperBound { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return LowerBound;
                case 1: return UpperBound;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitBounds(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitBounds(this);
        }

        IExpression IBounds.LowerBound => LowerBound;

        IExpression IBounds.UpperBound => UpperBound;
    }

    public abstract partial class TypeExpression : SyntaxNode, ITypeExpression
    {
        internal TypeExpression(SyntaxNodeKind kind)
            : base(kind)
        {
        }
    }

    public sealed partial class AndTypeExpression : TypeExpression, IAndTypeExpression
    {
        public AndTypeExpression(TypeExpression left, TypeExpression right)
            : base(SyntaxNodeKind.AndTypeExpression)
        {
            Left = left;
            Right = right;
        }
        public TypeExpression Left { get; }

        public TypeExpression Right { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAndTypeExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitAndTypeExpression(this);
        }

        ITypeExpression IAndTypeExpression.Left => Left;

        ITypeExpression IAndTypeExpression.Right => Right;
    }

    public sealed partial class AndOrTypeExpression : TypeExpression, IAndOrTypeExpression
    {
        public AndOrTypeExpression(TypeExpression left, TypeExpression right)
            : base(SyntaxNodeKind.AndOrTypeExpression)
        {
            Left = left;
            Right = right;
        }
        public TypeExpression Left { get; }

        public TypeExpression Right { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Left;
                case 1: return Right;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitAndOrTypeExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitAndOrTypeExpression(this);
        }

        ITypeExpression IAndOrTypeExpression.Left => Left;

        ITypeExpression IAndOrTypeExpression.Right => Right;
    }

    public sealed partial class OneOfTypeExpression : TypeExpression, IOneOfTypeExpression
    {
        public OneOfTypeExpression(SyntaxList<TypeExpression> expressions)
            : base(SyntaxNodeKind.OneOfTypeExpression)
        {
            Expressions = expressions;
        }
        public SyntaxList<TypeExpression> Expressions { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Expressions;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitOneOfTypeExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitOneOfTypeExpression(this);
        }

        IEnumerable<ITypeExpression> IOneOfTypeExpression.Expressions => Expressions;
    }

    public sealed partial class EntityReferenceTypeExpression : TypeExpression, IEntityReferenceTypeExpression
    {
        public EntityReferenceTypeExpression(EntityReference entity)
            : base(SyntaxNodeKind.EntityReferenceTypeExpression)
        {
            Entity = entity;
        }
        public EntityReference Entity { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Entity;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitEntityReferenceTypeExpression(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitEntityReferenceTypeExpression(this);
        }

        IEntityReference IEntityReferenceTypeExpression.Entity => Entity;
    }

    public abstract partial class InterfaceSpecification : SyntaxNode, IInterfaceSpecification
    {
        internal InterfaceSpecification(SyntaxNodeKind kind, SchemaReference schema, SyntaxList<Reference> references)
            : base(kind)
        {
            Schema = schema;
            References = references;
        }
        public SchemaReference Schema { get; }

        public SyntaxList<Reference> References { get; }


        ISchemaReference IInterfaceSpecification.Schema => Schema;

        IEnumerable<IReference> IInterfaceSpecification.References => References;
    }

    public sealed partial class UseClause : InterfaceSpecification, IUseClause
    {
        public UseClause(SchemaReference schema, SyntaxList<Reference> references)
            : base(SyntaxNodeKind.UseClause, schema, references)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Schema;
                case 1: return References;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitUseClause(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitUseClause(this);
        }
    }

    public sealed partial class ReferenceClause : InterfaceSpecification, IReferenceClause
    {
        public ReferenceClause(SchemaReference schema, SyntaxList<Reference> references)
            : base(SyntaxNodeKind.ReferenceClause, schema, references)
        {
        }
        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Schema;
                case 1: return References;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 2;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitReferenceClause(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitReferenceClause(this);
        }
    }

    public sealed partial class RenamedReference : Reference, IRenamedReference
    {
        public RenamedReference(Reference reference, string name)
            : base(SyntaxNodeKind.RenamedReference)
        {
            Reference = reference;
            Name = name;
        }
        public Reference Reference { get; }

        public string Name { get; }

        public override SyntaxNode GetSlot(int slot)
        {
            switch(slot)
            {
                case 0: return Reference;
                default: throw new ArgumentOutOfRangeException(nameof(slot));
            }
        }

        public override int SlotCount => 1;
        public override void Accept(SyntaxNodeVisitor visitor)
        {
            visitor.VisitRenamedReference(this);
        }

        public override T Accept<T>(SyntaxNodeVisitor<T> visitor)
        {
            return visitor.VisitRenamedReference(this);
        }

        IReference IRenamedReference.Reference => Reference;

        string IRenamedReference.Name => Name;
    }

}

