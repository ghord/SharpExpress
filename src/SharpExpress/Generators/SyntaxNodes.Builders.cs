using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.Immutable;

namespace SharpExpress.Builders
{
    abstract partial class DataTypeBuilder : SyntaxNodeBuilder<DataType>, IDataType
    {
    }

    abstract partial class AggregationDataTypeBuilder : DataTypeBuilder, IAggregationDataType
    {
        public DataTypeBuilder ElementType { get; set; }
        public BoundsBuilder Bounds { get; set; }

        IDataType IAggregationDataType.ElementType => ElementType;

        IBounds IAggregationDataType.Bounds => Bounds;
    }

    partial class AggregateDataTypeBuilder : AggregationDataTypeBuilder, IAggregateDataType
    {
        public TypeLabelReferenceBuilder TypeLabel { get; set; }
        public override DataType Build()
        {
            return new AggregateDataType(
                ElementType?.Build(),
                (TypeLabelReference) TypeLabel?.Build());
        }

        ITypeLabelReference IAggregateDataType.TypeLabel => TypeLabel;
    }

    partial class ListDataTypeBuilder : AggregationDataTypeBuilder, IListDataType
    {
        public bool IsUnique { get; set; }
        public override DataType Build()
        {
            return new ListDataType(
                ElementType?.Build(),
                Bounds?.Build(),
                IsUnique);
        }

        bool IListDataType.IsUnique => IsUnique;
    }

    partial class ArrayDataTypeBuilder : AggregationDataTypeBuilder, IArrayDataType
    {
        public bool IsOptional { get; set; }
        public bool IsUnique { get; set; }
        public override DataType Build()
        {
            return new ArrayDataType(
                ElementType?.Build(),
                Bounds?.Build(),
                IsOptional,
                IsUnique);
        }

        bool IArrayDataType.IsOptional => IsOptional;

        bool IArrayDataType.IsUnique => IsUnique;
    }

    partial class BagDataTypeBuilder : AggregationDataTypeBuilder, IBagDataType
    {
        public override DataType Build()
        {
            return new BagDataType(
                ElementType?.Build(),
                Bounds?.Build());
        }
    }

    partial class SetDataTypeBuilder : AggregationDataTypeBuilder, ISetDataType
    {
        public override DataType Build()
        {
            return new SetDataType(
                ElementType?.Build(),
                Bounds?.Build());
        }
    }

    abstract partial class ConstructedDataTypeBuilder : DataTypeBuilder, IConstructedDataType
    {
        public bool IsExtensible { get; set; }

        bool IConstructedDataType.IsExtensible => IsExtensible;
    }

    partial class EnumerationDataTypeBuilder : ConstructedDataTypeBuilder, IEnumerationDataType
    {
        public List<EnumerationDeclarationBuilder> Items { get; } = new List<EnumerationDeclarationBuilder>();
        public TypeReferenceBuilder BasedOn { get; set; }
        public override DataType Build()
        {
            return new EnumerationDataType(
                IsExtensible,
                SyntaxList.Create(Items.Select(n => n.Build()).Cast<EnumerationDeclaration>()),
                (TypeReference) BasedOn?.Build());
        }

        IEnumerable<IEnumerationDeclaration> IEnumerationDataType.Items => Items;

        ITypeReference IEnumerationDataType.BasedOn => BasedOn;
    }

    partial class SelectDataTypeBuilder : ConstructedDataTypeBuilder, ISelectDataType
    {
        public List<ReferenceDataTypeBuilder> Items { get; } = new List<ReferenceDataTypeBuilder>();
        public TypeReferenceBuilder BasedOn { get; set; }
        public bool IsGenericIdentity { get; set; }
        public override DataType Build()
        {
            return new SelectDataType(
                IsExtensible,
                SyntaxList.Create(Items.Select(n => n.Build()).Cast<ReferenceDataType>()),
                (TypeReference) BasedOn?.Build(),
                IsGenericIdentity);
        }

        IEnumerable<IReferenceDataType> ISelectDataType.Items => Items;

        ITypeReference ISelectDataType.BasedOn => BasedOn;

        bool ISelectDataType.IsGenericIdentity => IsGenericIdentity;
    }

    partial class ReferenceDataTypeBuilder : DataTypeBuilder, IReferenceDataType
    {
        public ReferenceBuilder Reference { get; set; }
        public override DataType Build()
        {
            return new ReferenceDataType(
                Reference?.Build());
        }

        IReference IReferenceDataType.Reference => Reference;
    }

    abstract partial class SimpleDataTypeBuilder : DataTypeBuilder, ISimpleDataType
    {
    }

    partial class StringDataTypeBuilder : SimpleDataTypeBuilder, IStringDataType
    {
        public ExpressionBuilder Width { get; set; }
        public bool IsFixed { get; set; }
        public override DataType Build()
        {
            return new StringDataType(
                Width?.Build(),
                IsFixed);
        }

        IExpression IStringDataType.Width => Width;

        bool IStringDataType.IsFixed => IsFixed;
    }

    partial class BinaryDataTypeBuilder : SimpleDataTypeBuilder, IBinaryDataType
    {
        public ExpressionBuilder Width { get; set; }
        public bool IsFixed { get; set; }
        public override DataType Build()
        {
            return new BinaryDataType(
                Width?.Build(),
                IsFixed);
        }

        IExpression IBinaryDataType.Width => Width;

        bool IBinaryDataType.IsFixed => IsFixed;
    }

    partial class RealDataTypeBuilder : SimpleDataTypeBuilder, IRealDataType
    {
        public ExpressionBuilder Precision { get; set; }
        public override DataType Build()
        {
            return new RealDataType(
                Precision?.Build());
        }

        IExpression IRealDataType.Precision => Precision;
    }

    partial class BooleanDataTypeBuilder : SimpleDataTypeBuilder, IBooleanDataType
    {
        public override DataType Build()
        {
            return new BooleanDataType(
                );
        }
    }

    partial class LogicalDataTypeBuilder : SimpleDataTypeBuilder, ILogicalDataType
    {
        public override DataType Build()
        {
            return new LogicalDataType(
                );
        }
    }

    partial class IntegerDataTypeBuilder : SimpleDataTypeBuilder, IIntegerDataType
    {
        public override DataType Build()
        {
            return new IntegerDataType(
                );
        }
    }

    partial class NumberDataTypeBuilder : SimpleDataTypeBuilder, INumberDataType
    {
        public override DataType Build()
        {
            return new NumberDataType(
                );
        }
    }

    partial class GenericDataTypeBuilder : DataTypeBuilder, IGenericDataType
    {
        public TypeLabelReferenceBuilder TypeLabel { get; set; }
        public override DataType Build()
        {
            return new GenericDataType(
                (TypeLabelReference) TypeLabel?.Build());
        }

        ITypeLabelReference IGenericDataType.TypeLabel => TypeLabel;
    }

    partial class GenericEntityDataTypeBuilder : DataTypeBuilder, IGenericEntityDataType
    {
        public TypeLabelReferenceBuilder TypeLabel { get; set; }
        public override DataType Build()
        {
            return new GenericEntityDataType(
                (TypeLabelReference) TypeLabel?.Build());
        }

        ITypeLabelReference IGenericEntityDataType.TypeLabel => TypeLabel;
    }

    abstract partial class DeclarationBuilder : SyntaxNodeBuilder<Declaration>, IDeclaration
    {
        public string Name { get; set; }

        string IDeclaration.Name => Name;
    }

    abstract partial class AlgorithmDeclarationBuilder : DeclarationBuilder, IAlgorithmDeclaration
    {
        public List<DeclarationBuilder> Declarations { get; } = new List<DeclarationBuilder>();
        public List<ConstantDeclarationBuilder> Constants { get; } = new List<ConstantDeclarationBuilder>();
        public List<VariableDeclarationBuilder> LocalVariables { get; } = new List<VariableDeclarationBuilder>();
        public List<StatementBuilder> Statements { get; } = new List<StatementBuilder>();

        IEnumerable<IDeclaration> IAlgorithmDeclaration.Declarations => Declarations;

        IEnumerable<IConstantDeclaration> IAlgorithmDeclaration.Constants => Constants;

        IEnumerable<IVariableDeclaration> IAlgorithmDeclaration.LocalVariables => LocalVariables;

        IEnumerable<IStatement> IAlgorithmDeclaration.Statements => Statements;
    }

    partial class SchemaDeclarationBuilder : DeclarationBuilder, ISchemaDeclaration
    {
        public string SchemaVersionId { get; set; }
        public List<InterfaceSpecificationBuilder> InterfaceSpecifications { get; } = new List<InterfaceSpecificationBuilder>();
        public List<ConstantDeclarationBuilder> Constants { get; } = new List<ConstantDeclarationBuilder>();
        public List<DeclarationBuilder> Declarations { get; } = new List<DeclarationBuilder>();
        public override Declaration Build()
        {
            return new SchemaDeclaration(
                Name,
                SchemaVersionId,
                SyntaxList.Create(InterfaceSpecifications.Select(n => n.Build())),
                SyntaxList.Create(Constants.Select(n => n.Build()).Cast<ConstantDeclaration>()),
                SyntaxList.Create(Declarations.Select(n => n.Build())));
        }

        string ISchemaDeclaration.SchemaVersionId => SchemaVersionId;

        IEnumerable<IInterfaceSpecification> ISchemaDeclaration.InterfaceSpecifications => InterfaceSpecifications;

        IEnumerable<IConstantDeclaration> ISchemaDeclaration.Constants => Constants;

        IEnumerable<IDeclaration> ISchemaDeclaration.Declarations => Declarations;
    }

    partial class TypeDeclarationBuilder : DeclarationBuilder, ITypeDeclaration
    {
        public DataTypeBuilder UnderlyingType { get; set; }
        public List<DomainRuleDeclarationBuilder> DomainRules { get; } = new List<DomainRuleDeclarationBuilder>();
        public override Declaration Build()
        {
            return new TypeDeclaration(
                Name,
                UnderlyingType?.Build(),
                SyntaxList.Create(DomainRules.Select(n => n.Build()).Cast<DomainRuleDeclaration>()));
        }

        IDataType ITypeDeclaration.UnderlyingType => UnderlyingType;

        IEnumerable<IDomainRuleDeclaration> ITypeDeclaration.DomainRules => DomainRules;
    }

    abstract partial class AttributeDeclarationBuilder : DeclarationBuilder, IAttributeDeclaration
    {
        public QualifiedReferenceBuilder RedeclaredAttribute { get; set; }
        public DataTypeBuilder Type { get; set; }

        IQualifiedReference IAttributeDeclaration.RedeclaredAttribute => RedeclaredAttribute;

        IDataType IAttributeDeclaration.Type => Type;
    }

    partial class ExplicitAttributeDeclarationBuilder : AttributeDeclarationBuilder, IExplicitAttributeDeclaration
    {
        public bool IsOptional { get; set; }
        public override Declaration Build()
        {
            return new ExplicitAttributeDeclaration(
                Name,
                RedeclaredAttribute?.Build(),
                Type?.Build(),
                IsOptional);
        }

        bool IExplicitAttributeDeclaration.IsOptional => IsOptional;
    }

    partial class InverseAttributeDeclarationBuilder : AttributeDeclarationBuilder, IInverseAttributeDeclaration
    {
        public EntityReferenceBuilder ForEntity { get; set; }
        public AttributeReferenceBuilder ForAttribute { get; set; }
        public override Declaration Build()
        {
            return new InverseAttributeDeclaration(
                Name,
                RedeclaredAttribute?.Build(),
                Type?.Build(),
                (EntityReference) ForEntity?.Build(),
                (AttributeReference) ForAttribute?.Build());
        }

        IEntityReference IInverseAttributeDeclaration.ForEntity => ForEntity;

        IAttributeReference IInverseAttributeDeclaration.ForAttribute => ForAttribute;
    }

    partial class DerivedAttributeDeclarationBuilder : AttributeDeclarationBuilder, IDerivedAttributeDeclaration
    {
        public ExpressionBuilder Value { get; set; }
        public override Declaration Build()
        {
            return new DerivedAttributeDeclaration(
                Name,
                RedeclaredAttribute?.Build(),
                Type?.Build(),
                Value?.Build());
        }

        IExpression IDerivedAttributeDeclaration.Value => Value;
    }

    partial class EntityDeclarationBuilder : DeclarationBuilder, IEntityDeclaration
    {
        public bool IsAbstract { get; set; }
        public bool IsAbstractSupertype { get; set; }
        public List<EntityReferenceBuilder> Supertypes { get; } = new List<EntityReferenceBuilder>();
        public TypeExpressionBuilder Subtype { get; set; }
        public List<ExplicitAttributeDeclarationBuilder> ExplicitAttributes { get; } = new List<ExplicitAttributeDeclarationBuilder>();
        public List<DerivedAttributeDeclarationBuilder> DerivedAttributes { get; } = new List<DerivedAttributeDeclarationBuilder>();
        public List<InverseAttributeDeclarationBuilder> InverseAttributes { get; } = new List<InverseAttributeDeclarationBuilder>();
        public List<UniqueRuleDeclarationBuilder> UniqueRules { get; } = new List<UniqueRuleDeclarationBuilder>();
        public List<DomainRuleDeclarationBuilder> DomainRules { get; } = new List<DomainRuleDeclarationBuilder>();
        public override Declaration Build()
        {
            return new EntityDeclaration(
                Name,
                IsAbstract,
                IsAbstractSupertype,
                SyntaxList.Create(Supertypes.Select(n => n.Build()).Cast<EntityReference>()),
                Subtype?.Build(),
                SyntaxList.Create(ExplicitAttributes.Select(n => n.Build()).Cast<ExplicitAttributeDeclaration>()),
                SyntaxList.Create(DerivedAttributes.Select(n => n.Build()).Cast<DerivedAttributeDeclaration>()),
                SyntaxList.Create(InverseAttributes.Select(n => n.Build()).Cast<InverseAttributeDeclaration>()),
                SyntaxList.Create(UniqueRules.Select(n => n.Build()).Cast<UniqueRuleDeclaration>()),
                SyntaxList.Create(DomainRules.Select(n => n.Build()).Cast<DomainRuleDeclaration>()));
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

    partial class ConstantDeclarationBuilder : DeclarationBuilder, IConstantDeclaration
    {
        public DataTypeBuilder Type { get; set; }
        public ExpressionBuilder Value { get; set; }
        public override Declaration Build()
        {
            return new ConstantDeclaration(
                Name,
                Type?.Build(),
                Value?.Build());
        }

        IDataType IConstantDeclaration.Type => Type;

        IExpression IConstantDeclaration.Value => Value;
    }

    partial class EnumerationDeclarationBuilder : DeclarationBuilder, IEnumerationDeclaration
    {
        public override Declaration Build()
        {
            return new EnumerationDeclaration(
                Name);
        }
    }

    partial class ProcedureDeclarationBuilder : AlgorithmDeclarationBuilder, IProcedureDeclaration
    {
        public List<ParameterDeclarationBuilder> Parameters { get; } = new List<ParameterDeclarationBuilder>();
        public override Declaration Build()
        {
            return new ProcedureDeclaration(
                Name,
                SyntaxList.Create(Declarations.Select(n => n.Build())),
                SyntaxList.Create(Constants.Select(n => n.Build()).Cast<ConstantDeclaration>()),
                SyntaxList.Create(LocalVariables.Select(n => n.Build()).Cast<VariableDeclaration>()),
                SyntaxList.Create(Statements.Select(n => n.Build())),
                SyntaxList.Create(Parameters.Select(n => n.Build()).Cast<ParameterDeclaration>()));
        }

        IEnumerable<IParameterDeclaration> IProcedureDeclaration.Parameters => Parameters;
    }

    partial class FunctionDeclarationBuilder : AlgorithmDeclarationBuilder, IFunctionDeclaration
    {
        public List<ParameterDeclarationBuilder> Parameters { get; } = new List<ParameterDeclarationBuilder>();
        public DataTypeBuilder ReturnType { get; set; }
        public override Declaration Build()
        {
            return new FunctionDeclaration(
                Name,
                SyntaxList.Create(Declarations.Select(n => n.Build())),
                SyntaxList.Create(Constants.Select(n => n.Build()).Cast<ConstantDeclaration>()),
                SyntaxList.Create(LocalVariables.Select(n => n.Build()).Cast<VariableDeclaration>()),
                SyntaxList.Create(Statements.Select(n => n.Build())),
                SyntaxList.Create(Parameters.Select(n => n.Build()).Cast<ParameterDeclaration>()),
                ReturnType?.Build());
        }

        IEnumerable<IParameterDeclaration> IFunctionDeclaration.Parameters => Parameters;

        IDataType IFunctionDeclaration.ReturnType => ReturnType;
    }

    partial class RuleDeclarationBuilder : AlgorithmDeclarationBuilder, IRuleDeclaration
    {
        public List<VariableDeclarationBuilder> Populations { get; } = new List<VariableDeclarationBuilder>();
        public List<DomainRuleDeclarationBuilder> DomainRules { get; } = new List<DomainRuleDeclarationBuilder>();
        public override Declaration Build()
        {
            return new RuleDeclaration(
                Name,
                SyntaxList.Create(Declarations.Select(n => n.Build())),
                SyntaxList.Create(Constants.Select(n => n.Build()).Cast<ConstantDeclaration>()),
                SyntaxList.Create(LocalVariables.Select(n => n.Build()).Cast<VariableDeclaration>()),
                SyntaxList.Create(Statements.Select(n => n.Build())),
                SyntaxList.Create(Populations.Select(n => n.Build()).Cast<VariableDeclaration>()),
                SyntaxList.Create(DomainRules.Select(n => n.Build()).Cast<DomainRuleDeclaration>()));
        }

        IEnumerable<IVariableDeclaration> IRuleDeclaration.Populations => Populations;

        IEnumerable<IDomainRuleDeclaration> IRuleDeclaration.DomainRules => DomainRules;
    }

    abstract partial class LocalRuleDeclarationBuilder : DeclarationBuilder, ILocalRuleDeclaration
    {
    }

    partial class DomainRuleDeclarationBuilder : LocalRuleDeclarationBuilder, IDomainRuleDeclaration
    {
        public ExpressionBuilder Expression { get; set; }
        public override Declaration Build()
        {
            return new DomainRuleDeclaration(
                Name,
                Expression?.Build());
        }

        IExpression IDomainRuleDeclaration.Expression => Expression;
    }

    partial class UniqueRuleDeclarationBuilder : LocalRuleDeclarationBuilder, IUniqueRuleDeclaration
    {
        public List<ExpressionBuilder> Expressions { get; } = new List<ExpressionBuilder>();
        public override Declaration Build()
        {
            return new UniqueRuleDeclaration(
                Name,
                SyntaxList.Create(Expressions.Select(n => n.Build())));
        }

        IEnumerable<IExpression> IUniqueRuleDeclaration.Expressions => Expressions;
    }

    partial class ParameterDeclarationBuilder : DeclarationBuilder, IParameterDeclaration
    {
        public DataTypeBuilder Type { get; set; }
        public bool IsVariable { get; set; }
        public override Declaration Build()
        {
            return new ParameterDeclaration(
                Name,
                Type?.Build(),
                IsVariable);
        }

        IDataType IParameterDeclaration.Type => Type;

        bool IParameterDeclaration.IsVariable => IsVariable;
    }

    partial class VariableDeclarationBuilder : DeclarationBuilder, IVariableDeclaration
    {
        public DataTypeBuilder Type { get; set; }
        public ExpressionBuilder InitialValue { get; set; }
        public override Declaration Build()
        {
            return new VariableDeclaration(
                Name,
                Type?.Build(),
                InitialValue?.Build());
        }

        IDataType IVariableDeclaration.Type => Type;

        IExpression IVariableDeclaration.InitialValue => InitialValue;
    }

    partial class SubtypeConstraintDeclarationBuilder : DeclarationBuilder, ISubtypeConstraintDeclaration
    {
        public override Declaration Build()
        {
            return new SubtypeConstraintDeclaration(
                Name);
        }
    }

    partial class TypeLabelDeclarationBuilder : DeclarationBuilder, ITypeLabelDeclaration
    {
        public override Declaration Build()
        {
            return new TypeLabelDeclaration(
                Name);
        }
    }

    abstract partial class ExpressionBuilder : SyntaxNodeBuilder<Expression>, IExpression
    {
    }

    abstract partial class BinaryExpressionBuilder : ExpressionBuilder, IBinaryExpression
    {
        public ExpressionBuilder Left { get; set; }
        public ExpressionBuilder Right { get; set; }

        IExpression IBinaryExpression.Left => Left;

        IExpression IBinaryExpression.Right => Right;
    }

    partial class LessThanExpressionBuilder : BinaryExpressionBuilder, ILessThanExpression
    {
        public override Expression Build()
        {
            return new LessThanExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class GreaterThanExpressionBuilder : BinaryExpressionBuilder, IGreaterThanExpression
    {
        public override Expression Build()
        {
            return new GreaterThanExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class LessThanOrEqualExpressionBuilder : BinaryExpressionBuilder, ILessThanOrEqualExpression
    {
        public override Expression Build()
        {
            return new LessThanOrEqualExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class GreaterThanOrEqualExpressionBuilder : BinaryExpressionBuilder, IGreaterThanOrEqualExpression
    {
        public override Expression Build()
        {
            return new GreaterThanOrEqualExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class NotEqualExpressionBuilder : BinaryExpressionBuilder, INotEqualExpression
    {
        public override Expression Build()
        {
            return new NotEqualExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class EqualExpressionBuilder : BinaryExpressionBuilder, IEqualExpression
    {
        public override Expression Build()
        {
            return new EqualExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class InstanceNotEqualExpressionBuilder : BinaryExpressionBuilder, IInstanceNotEqualExpression
    {
        public override Expression Build()
        {
            return new InstanceNotEqualExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class InstanceEqualExpressionBuilder : BinaryExpressionBuilder, IInstanceEqualExpression
    {
        public override Expression Build()
        {
            return new InstanceEqualExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class InExpressionBuilder : BinaryExpressionBuilder, IInExpression
    {
        public override Expression Build()
        {
            return new InExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class AdditionExpressionBuilder : BinaryExpressionBuilder, IAdditionExpression
    {
        public override Expression Build()
        {
            return new AdditionExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class SubtractionExpressionBuilder : BinaryExpressionBuilder, ISubtractionExpression
    {
        public override Expression Build()
        {
            return new SubtractionExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class OrExpressionBuilder : BinaryExpressionBuilder, IOrExpression
    {
        public override Expression Build()
        {
            return new OrExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class XorExpressionBuilder : BinaryExpressionBuilder, IXorExpression
    {
        public override Expression Build()
        {
            return new XorExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class MultiplicationExpressionBuilder : BinaryExpressionBuilder, IMultiplicationExpression
    {
        public override Expression Build()
        {
            return new MultiplicationExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class DivisionExpressionBuilder : BinaryExpressionBuilder, IDivisionExpression
    {
        public override Expression Build()
        {
            return new DivisionExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class IntegerDivisionExpressionBuilder : BinaryExpressionBuilder, IIntegerDivisionExpression
    {
        public override Expression Build()
        {
            return new IntegerDivisionExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class ModuloExpressionBuilder : BinaryExpressionBuilder, IModuloExpression
    {
        public override Expression Build()
        {
            return new ModuloExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class AndExpressionBuilder : BinaryExpressionBuilder, IAndExpression
    {
        public override Expression Build()
        {
            return new AndExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class ExponentiationExpressionBuilder : BinaryExpressionBuilder, IExponentiationExpression
    {
        public override Expression Build()
        {
            return new ExponentiationExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class LikeExpressionBuilder : BinaryExpressionBuilder, ILikeExpression
    {
        public override Expression Build()
        {
            return new LikeExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    partial class ComplexEntityConstructionExpressionBuilder : BinaryExpressionBuilder, IComplexEntityConstructionExpression
    {
        public override Expression Build()
        {
            return new ComplexEntityConstructionExpression(
                Left?.Build(),
                Right?.Build());
        }
    }

    abstract partial class UnaryExpressionBuilder : ExpressionBuilder, IUnaryExpression
    {
        public ExpressionBuilder Operand { get; set; }

        IExpression IUnaryExpression.Operand => Operand;
    }

    partial class UnaryPlusExpressionBuilder : UnaryExpressionBuilder, IUnaryPlusExpression
    {
        public override Expression Build()
        {
            return new UnaryPlusExpression(
                Operand?.Build());
        }
    }

    partial class UnaryMinusExpressionBuilder : UnaryExpressionBuilder, IUnaryMinusExpression
    {
        public override Expression Build()
        {
            return new UnaryMinusExpression(
                Operand?.Build());
        }
    }

    partial class NotExpressionBuilder : UnaryExpressionBuilder, INotExpression
    {
        public override Expression Build()
        {
            return new NotExpression(
                Operand?.Build());
        }
    }

    abstract partial class LiteralExpressionBuilder : ExpressionBuilder, ILiteralExpression
    {
    }

    partial class StringLiteralBuilder : LiteralExpressionBuilder, IStringLiteral
    {
        public string Value { get; set; }
        public override Expression Build()
        {
            return new StringLiteral(
                Value);
        }

        string IStringLiteral.Value => Value;
    }

    partial class IntegerLiteralBuilder : LiteralExpressionBuilder, IIntegerLiteral
    {
        public int Value { get; set; }
        public override Expression Build()
        {
            return new IntegerLiteral(
                Value);
        }

        int IIntegerLiteral.Value => Value;
    }

    partial class RealLiteralBuilder : LiteralExpressionBuilder, IRealLiteral
    {
        public double Value { get; set; }
        public override Expression Build()
        {
            return new RealLiteral(
                Value);
        }

        double IRealLiteral.Value => Value;
    }

    partial class BinaryLiteralBuilder : LiteralExpressionBuilder, IBinaryLiteral
    {
        public byte[] Value { get; set; }
        public override Expression Build()
        {
            return new BinaryLiteral(
                Value);
        }

        byte[] IBinaryLiteral.Value => Value;
    }

    partial class AggregateInitializerExpressionBuilder : ExpressionBuilder, IAggregateInitializerExpression
    {
        public List<AggregateInitializerElementBuilder> Elements { get; } = new List<AggregateInitializerElementBuilder>();
        public override Expression Build()
        {
            return new AggregateInitializerExpression(
                SyntaxList.Create(Elements.Select(n => n.Build())));
        }

        IEnumerable<IAggregateInitializerElement> IAggregateInitializerExpression.Elements => Elements;
    }

    partial class AggregateInitializerElementBuilder : SyntaxNodeBuilder<AggregateInitializerElement>, IAggregateInitializerElement
    {
        public ExpressionBuilder Expression { get; set; }
        public ExpressionBuilder Repetition { get; set; }
        public override AggregateInitializerElement Build()
        {
            return new AggregateInitializerElement(
                Expression?.Build(),
                Repetition?.Build());
        }

        IExpression IAggregateInitializerElement.Expression => Expression;

        IExpression IAggregateInitializerElement.Repetition => Repetition;
    }

    partial class QueryExpressionBuilder : ExpressionBuilder, IQueryExpression
    {
        public VariableDeclarationBuilder VariableDeclaration { get; set; }
        public ExpressionBuilder Aggregate { get; set; }
        public ExpressionBuilder Condition { get; set; }
        public override Expression Build()
        {
            return new QueryExpression(
                (VariableDeclaration) VariableDeclaration?.Build(),
                Aggregate?.Build(),
                Condition?.Build());
        }

        IVariableDeclaration IQueryExpression.VariableDeclaration => VariableDeclaration;

        IExpression IQueryExpression.Aggregate => Aggregate;

        IExpression IQueryExpression.Condition => Condition;
    }

    partial class IntervalExpressionBuilder : ExpressionBuilder, IIntervalExpression
    {
        public ExpressionBuilder Low { get; set; }
        public IntervalComparison LowComparison { get; set; }
        public ExpressionBuilder Item { get; set; }
        public IntervalComparison HighComparison { get; set; }
        public ExpressionBuilder High { get; set; }
        public override Expression Build()
        {
            return new IntervalExpression(
                Low?.Build(),
                LowComparison,
                Item?.Build(),
                HighComparison,
                High?.Build());
        }

        IExpression IIntervalExpression.Low => Low;

        IntervalComparison IIntervalExpression.LowComparison => LowComparison;

        IExpression IIntervalExpression.Item => Item;

        IntervalComparison IIntervalExpression.HighComparison => HighComparison;

        IExpression IIntervalExpression.High => High;
    }

    partial class EntityConstructorExpressionBuilder : ExpressionBuilder, IEntityConstructorExpression
    {
        public EntityReferenceBuilder Entity { get; set; }
        public List<ExpressionBuilder> Paramaters { get; } = new List<ExpressionBuilder>();
        public override Expression Build()
        {
            return new EntityConstructorExpression(
                (EntityReference) Entity?.Build(),
                SyntaxList.Create(Paramaters.Select(n => n.Build())));
        }

        IEntityReference IEntityConstructorExpression.Entity => Entity;

        IEnumerable<IExpression> IEntityConstructorExpression.Paramaters => Paramaters;
    }

    partial class EnumerationReferenceExpressionBuilder : ExpressionBuilder, IEnumerationReferenceExpression
    {
        public TypeReferenceBuilder Type { get; set; }
        public EnumerationReferenceBuilder Enumeration { get; set; }
        public override Expression Build()
        {
            return new EnumerationReferenceExpression(
                (TypeReference) Type?.Build(),
                (EnumerationReference) Enumeration?.Build());
        }

        ITypeReference IEnumerationReferenceExpression.Type => Type;

        IEnumerationReference IEnumerationReferenceExpression.Enumeration => Enumeration;
    }

    partial class ConstantReferenceExpressionBuilder : ExpressionBuilder, IConstantReferenceExpression
    {
        public ConstantReferenceBuilder Constant { get; set; }
        public override Expression Build()
        {
            return new ConstantReferenceExpression(
                (ConstantReference) Constant?.Build());
        }

        IConstantReference IConstantReferenceExpression.Constant => Constant;
    }

    partial class ParameterReferenceExpressionBuilder : ExpressionBuilder, IParameterReferenceExpression
    {
        public ParameterReferenceBuilder Parameter { get; set; }
        public override Expression Build()
        {
            return new ParameterReferenceExpression(
                (ParameterReference) Parameter?.Build());
        }

        IParameterReference IParameterReferenceExpression.Parameter => Parameter;
    }

    partial class EntityReferenceExpressionBuilder : ExpressionBuilder, IEntityReferenceExpression
    {
        public EntityReferenceBuilder Entity { get; set; }
        public override Expression Build()
        {
            return new EntityReferenceExpression(
                (EntityReference) Entity?.Build());
        }

        IEntityReference IEntityReferenceExpression.Entity => Entity;
    }

    partial class VariableReferenceExpressionBuilder : ExpressionBuilder, IVariableReferenceExpression
    {
        public VariableReferenceBuilder Variable { get; set; }
        public override Expression Build()
        {
            return new VariableReferenceExpression(
                (VariableReference) Variable?.Build());
        }

        IVariableReference IVariableReferenceExpression.Variable => Variable;
    }

    partial class FunctionCallExpressionBuilder : ExpressionBuilder, IFunctionCallExpression
    {
        public FunctionReferenceBuilder Function { get; set; }
        public List<ExpressionBuilder> Parameters { get; } = new List<ExpressionBuilder>();
        public override Expression Build()
        {
            return new FunctionCallExpression(
                (FunctionReference) Function?.Build(),
                SyntaxList.Create(Parameters.Select(n => n.Build())));
        }

        IFunctionReference IFunctionCallExpression.Function => Function;

        IEnumerable<IExpression> IFunctionCallExpression.Parameters => Parameters;
    }

    partial class QualifiedExpressionBuilder : ExpressionBuilder, IQualifiedExpression
    {
        public ExpressionBuilder Expression { get; set; }
        public List<QualifierBuilder> Qualifiers { get; } = new List<QualifierBuilder>();
        public override Expression Build()
        {
            return new QualifiedExpression(
                Expression?.Build(),
                SyntaxList.Create(Qualifiers.Select(n => n.Build())));
        }

        IExpression IQualifiedExpression.Expression => Expression;

        IEnumerable<IQualifier> IQualifiedExpression.Qualifiers => Qualifiers;
    }

    partial class AttributeReferenceExpressionBuilder : ExpressionBuilder, IAttributeReferenceExpression
    {
        public AttributeReferenceBuilder Attribute { get; set; }
        public override Expression Build()
        {
            return new AttributeReferenceExpression(
                (AttributeReference) Attribute?.Build());
        }

        IAttributeReference IAttributeReferenceExpression.Attribute => Attribute;
    }

    abstract partial class StatementBuilder : SyntaxNodeBuilder<Statement>, IStatement
    {
    }

    partial class NullStatementBuilder : StatementBuilder, INullStatement
    {
        public override Statement Build()
        {
            return new NullStatement(
                );
        }
    }

    partial class AliasStatementBuilder : StatementBuilder, IAliasStatement
    {
        public VariableDeclarationBuilder Variable { get; set; }
        public QualifiedReferenceBuilder RenamedReference { get; set; }
        public override Statement Build()
        {
            return new AliasStatement(
                (VariableDeclaration) Variable?.Build(),
                RenamedReference?.Build());
        }

        IVariableDeclaration IAliasStatement.Variable => Variable;

        IQualifiedReference IAliasStatement.RenamedReference => RenamedReference;
    }

    partial class AssignmentStatementBuilder : StatementBuilder, IAssignmentStatement
    {
        public QualifiedReferenceBuilder Left { get; set; }
        public ExpressionBuilder Right { get; set; }
        public override Statement Build()
        {
            return new AssignmentStatement(
                Left?.Build(),
                Right?.Build());
        }

        IQualifiedReference IAssignmentStatement.Left => Left;

        IExpression IAssignmentStatement.Right => Right;
    }

    partial class CaseStatementBuilder : StatementBuilder, ICaseStatement
    {
        public ExpressionBuilder Selector { get; set; }
        public List<CaseActionBuilder> Actions { get; } = new List<CaseActionBuilder>();
        public StatementBuilder DefaultAction { get; set; }
        public override Statement Build()
        {
            return new CaseStatement(
                Selector?.Build(),
                SyntaxList.Create(Actions.Select(n => n.Build()).Cast<CaseAction>()),
                DefaultAction?.Build());
        }

        IExpression ICaseStatement.Selector => Selector;

        IEnumerable<ICaseAction> ICaseStatement.Actions => Actions;

        IStatement ICaseStatement.DefaultAction => DefaultAction;
    }

    partial class CaseActionBuilder : StatementBuilder, ICaseAction
    {
        public List<ExpressionBuilder> Labels { get; } = new List<ExpressionBuilder>();
        public StatementBuilder Statement { get; set; }
        public override Statement Build()
        {
            return new CaseAction(
                SyntaxList.Create(Labels.Select(n => n.Build())),
                Statement?.Build());
        }

        IEnumerable<IExpression> ICaseAction.Labels => Labels;

        IStatement ICaseAction.Statement => Statement;
    }

    partial class CompoundStatementBuilder : StatementBuilder, ICompoundStatement
    {
        public List<StatementBuilder> Statements { get; } = new List<StatementBuilder>();
        public override Statement Build()
        {
            return new CompoundStatement(
                SyntaxList.Create(Statements.Select(n => n.Build())));
        }

        IEnumerable<IStatement> ICompoundStatement.Statements => Statements;
    }

    partial class IfStatementBuilder : StatementBuilder, IIfStatement
    {
        public ExpressionBuilder Condition { get; set; }
        public List<StatementBuilder> Statements { get; } = new List<StatementBuilder>();
        public List<StatementBuilder> ElseStatements { get; } = new List<StatementBuilder>();
        public override Statement Build()
        {
            return new IfStatement(
                Condition?.Build(),
                SyntaxList.Create(Statements.Select(n => n.Build())),
                SyntaxList.Create(ElseStatements.Select(n => n.Build())));
        }

        IExpression IIfStatement.Condition => Condition;

        IEnumerable<IStatement> IIfStatement.Statements => Statements;

        IEnumerable<IStatement> IIfStatement.ElseStatements => ElseStatements;
    }

    partial class ProcedureCallStatementBuilder : StatementBuilder, IProcedureCallStatement
    {
        public ProcedureReferenceBuilder Procedure { get; set; }
        public List<ExpressionBuilder> Parameters { get; } = new List<ExpressionBuilder>();
        public override Statement Build()
        {
            return new ProcedureCallStatement(
                (ProcedureReference) Procedure?.Build(),
                SyntaxList.Create(Parameters.Select(n => n.Build())));
        }

        IProcedureReference IProcedureCallStatement.Procedure => Procedure;

        IEnumerable<IExpression> IProcedureCallStatement.Parameters => Parameters;
    }

    partial class RepeatStatementBuilder : StatementBuilder, IRepeatStatement
    {
        public VariableDeclarationBuilder IncrementVariable { get; set; }
        public ExpressionBuilder IncrementFrom { get; set; }
        public ExpressionBuilder IncrementTo { get; set; }
        public ExpressionBuilder IncrementStep { get; set; }
        public ExpressionBuilder WhileCondition { get; set; }
        public ExpressionBuilder UntilCondition { get; set; }
        public List<StatementBuilder> Statements { get; } = new List<StatementBuilder>();
        public override Statement Build()
        {
            return new RepeatStatement(
                (VariableDeclaration) IncrementVariable?.Build(),
                IncrementFrom?.Build(),
                IncrementTo?.Build(),
                IncrementStep?.Build(),
                WhileCondition?.Build(),
                UntilCondition?.Build(),
                SyntaxList.Create(Statements.Select(n => n.Build())));
        }

        IVariableDeclaration IRepeatStatement.IncrementVariable => IncrementVariable;

        IExpression IRepeatStatement.IncrementFrom => IncrementFrom;

        IExpression IRepeatStatement.IncrementTo => IncrementTo;

        IExpression IRepeatStatement.IncrementStep => IncrementStep;

        IExpression IRepeatStatement.WhileCondition => WhileCondition;

        IExpression IRepeatStatement.UntilCondition => UntilCondition;

        IEnumerable<IStatement> IRepeatStatement.Statements => Statements;
    }

    partial class ReturnStatementBuilder : StatementBuilder, IReturnStatement
    {
        public ExpressionBuilder Expression { get; set; }
        public override Statement Build()
        {
            return new ReturnStatement(
                Expression?.Build());
        }

        IExpression IReturnStatement.Expression => Expression;
    }

    partial class SkipStatementBuilder : StatementBuilder, ISkipStatement
    {
        public override Statement Build()
        {
            return new SkipStatement(
                );
        }
    }

    partial class EscapeStatementBuilder : StatementBuilder, IEscapeStatement
    {
        public override Statement Build()
        {
            return new EscapeStatement(
                );
        }
    }

    partial class QualifiedReferenceBuilder : SyntaxNodeBuilder<QualifiedReference>, IQualifiedReference
    {
        public ReferenceBuilder Reference { get; set; }
        public List<QualifierBuilder> Qualifiers { get; } = new List<QualifierBuilder>();
        public override QualifiedReference Build()
        {
            return new QualifiedReference(
                Reference?.Build(),
                SyntaxList.Create(Qualifiers.Select(n => n.Build())));
        }

        IReference IQualifiedReference.Reference => Reference;

        IEnumerable<IQualifier> IQualifiedReference.Qualifiers => Qualifiers;
    }

    abstract partial class QualifierBuilder : SyntaxNodeBuilder<Qualifier>, IQualifier
    {
    }

    partial class AttributeQualifierBuilder : QualifierBuilder, IAttributeQualifier
    {
        public AttributeReferenceBuilder Attribute { get; set; }
        public override Qualifier Build()
        {
            return new AttributeQualifier(
                (AttributeReference) Attribute?.Build());
        }

        IAttributeReference IAttributeQualifier.Attribute => Attribute;
    }

    partial class GroupQualifierBuilder : QualifierBuilder, IGroupQualifier
    {
        public EntityReferenceBuilder Entity { get; set; }
        public override Qualifier Build()
        {
            return new GroupQualifier(
                (EntityReference) Entity?.Build());
        }

        IEntityReference IGroupQualifier.Entity => Entity;
    }

    partial class IndexQualifierBuilder : QualifierBuilder, IIndexQualifier
    {
        public ExpressionBuilder From { get; set; }
        public ExpressionBuilder To { get; set; }
        public override Qualifier Build()
        {
            return new IndexQualifier(
                From?.Build(),
                To?.Build());
        }

        IExpression IIndexQualifier.From => From;

        IExpression IIndexQualifier.To => To;
    }

    abstract partial class ReferenceBuilder : SyntaxNodeBuilder<Reference>, IReference
    {
    }

    partial class SchemaReferenceBuilder : ReferenceBuilder, ISchemaReference
    {
        public string SchemaName { get; set; }
        public override Reference Build()
        {
            return new SchemaReference(
                SchemaName);
        }

        string ISchemaReference.SchemaName => SchemaName;
    }

    partial class TypeReferenceBuilder : ReferenceBuilder, ITypeReference
    {
        public string TypeName { get; set; }
        public override Reference Build()
        {
            return new TypeReference(
                TypeName);
        }

        string ITypeReference.TypeName => TypeName;
    }

    partial class EntityReferenceBuilder : ReferenceBuilder, IEntityReference
    {
        public string EntityName { get; set; }
        public override Reference Build()
        {
            return new EntityReference(
                EntityName);
        }

        string IEntityReference.EntityName => EntityName;
    }

    partial class AttributeReferenceBuilder : ReferenceBuilder, IAttributeReference
    {
        public string AttributeName { get; set; }
        public override Reference Build()
        {
            return new AttributeReference(
                AttributeName);
        }

        string IAttributeReference.AttributeName => AttributeName;
    }

    partial class UnresolvedReferenceBuilder : ReferenceBuilder, IUnresolvedReference
    {
        public string UnresolvedName { get; set; }
        public override Reference Build()
        {
            return new UnresolvedReference(
                UnresolvedName);
        }

        string IUnresolvedReference.UnresolvedName => UnresolvedName;
    }

    partial class ParameterReferenceBuilder : ReferenceBuilder, IParameterReference
    {
        public string ParameterName { get; set; }
        public override Reference Build()
        {
            return new ParameterReference(
                ParameterName);
        }

        string IParameterReference.ParameterName => ParameterName;
    }

    partial class VariableReferenceBuilder : ReferenceBuilder, IVariableReference
    {
        public string VariableName { get; set; }
        public override Reference Build()
        {
            return new VariableReference(
                VariableName);
        }

        string IVariableReference.VariableName => VariableName;
    }

    partial class EnumerationReferenceBuilder : ReferenceBuilder, IEnumerationReference
    {
        public string EnumerationName { get; set; }
        public override Reference Build()
        {
            return new EnumerationReference(
                EnumerationName);
        }

        string IEnumerationReference.EnumerationName => EnumerationName;
    }

    partial class ConstantReferenceBuilder : ReferenceBuilder, IConstantReference
    {
        public string ConstantName { get; set; }
        public override Reference Build()
        {
            return new ConstantReference(
                ConstantName);
        }

        string IConstantReference.ConstantName => ConstantName;
    }

    partial class FunctionReferenceBuilder : ReferenceBuilder, IFunctionReference
    {
        public string FunctionName { get; set; }
        public override Reference Build()
        {
            return new FunctionReference(
                FunctionName);
        }

        string IFunctionReference.FunctionName => FunctionName;
    }

    partial class ProcedureReferenceBuilder : ReferenceBuilder, IProcedureReference
    {
        public string ProcedureName { get; set; }
        public override Reference Build()
        {
            return new ProcedureReference(
                ProcedureName);
        }

        string IProcedureReference.ProcedureName => ProcedureName;
    }

    partial class TypeLabelReferenceBuilder : ReferenceBuilder, ITypeLabelReference
    {
        public string TypeLabelName { get; set; }
        public override Reference Build()
        {
            return new TypeLabelReference(
                TypeLabelName);
        }

        string ITypeLabelReference.TypeLabelName => TypeLabelName;
    }

    partial class BoundsBuilder : SyntaxNodeBuilder<Bounds>, IBounds
    {
        public ExpressionBuilder LowerBound { get; set; }
        public ExpressionBuilder UpperBound { get; set; }
        public override Bounds Build()
        {
            return new Bounds(
                LowerBound?.Build(),
                UpperBound?.Build());
        }

        IExpression IBounds.LowerBound => LowerBound;

        IExpression IBounds.UpperBound => UpperBound;
    }

    abstract partial class TypeExpressionBuilder : SyntaxNodeBuilder<TypeExpression>, ITypeExpression
    {
    }

    partial class AndTypeExpressionBuilder : TypeExpressionBuilder, IAndTypeExpression
    {
        public TypeExpressionBuilder Left { get; set; }
        public TypeExpressionBuilder Right { get; set; }
        public override TypeExpression Build()
        {
            return new AndTypeExpression(
                Left?.Build(),
                Right?.Build());
        }

        ITypeExpression IAndTypeExpression.Left => Left;

        ITypeExpression IAndTypeExpression.Right => Right;
    }

    partial class AndOrTypeExpressionBuilder : TypeExpressionBuilder, IAndOrTypeExpression
    {
        public TypeExpressionBuilder Left { get; set; }
        public TypeExpressionBuilder Right { get; set; }
        public override TypeExpression Build()
        {
            return new AndOrTypeExpression(
                Left?.Build(),
                Right?.Build());
        }

        ITypeExpression IAndOrTypeExpression.Left => Left;

        ITypeExpression IAndOrTypeExpression.Right => Right;
    }

    partial class OneOfTypeExpressionBuilder : TypeExpressionBuilder, IOneOfTypeExpression
    {
        public List<TypeExpressionBuilder> Expressions { get; } = new List<TypeExpressionBuilder>();
        public override TypeExpression Build()
        {
            return new OneOfTypeExpression(
                SyntaxList.Create(Expressions.Select(n => n.Build())));
        }

        IEnumerable<ITypeExpression> IOneOfTypeExpression.Expressions => Expressions;
    }

    partial class EntityReferenceTypeExpressionBuilder : TypeExpressionBuilder, IEntityReferenceTypeExpression
    {
        public EntityReferenceBuilder Entity { get; set; }
        public override TypeExpression Build()
        {
            return new EntityReferenceTypeExpression(
                (EntityReference) Entity?.Build());
        }

        IEntityReference IEntityReferenceTypeExpression.Entity => Entity;
    }

    abstract partial class InterfaceSpecificationBuilder : SyntaxNodeBuilder<InterfaceSpecification>, IInterfaceSpecification
    {
        public SchemaReferenceBuilder Schema { get; set; }
        public List<ReferenceBuilder> References { get; } = new List<ReferenceBuilder>();

        ISchemaReference IInterfaceSpecification.Schema => Schema;

        IEnumerable<IReference> IInterfaceSpecification.References => References;
    }

    partial class UseClauseBuilder : InterfaceSpecificationBuilder, IUseClause
    {
        public override InterfaceSpecification Build()
        {
            return new UseClause(
                (SchemaReference) Schema?.Build(),
                SyntaxList.Create(References.Select(n => n.Build())));
        }
    }

    partial class ReferenceClauseBuilder : InterfaceSpecificationBuilder, IReferenceClause
    {
        public override InterfaceSpecification Build()
        {
            return new ReferenceClause(
                (SchemaReference) Schema?.Build(),
                SyntaxList.Create(References.Select(n => n.Build())));
        }
    }

    partial class RenamedReferenceBuilder : ReferenceBuilder, IRenamedReference
    {
        public ReferenceBuilder Reference { get; set; }
        public string Name { get; set; }
        public override Reference Build()
        {
            return new RenamedReference(
                Reference?.Build(),
                Name);
        }

        IReference IRenamedReference.Reference => Reference;

        string IRenamedReference.Name => Name;
    }

}

