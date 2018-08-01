using System;
using System.Collections.Generic;

namespace SharpExpress
{
    interface IDataType
    {
    }

    interface IAggregationDataType : IDataType
    {
        IDataType ElementType { get; }
        IBounds Bounds { get; }
    }

    interface IAggregateDataType : IAggregationDataType
    {
        ITypeLabelReference TypeLabel { get; }
    }

    interface IListDataType : IAggregationDataType
    {
        bool IsUnique { get; }
    }

    interface IArrayDataType : IAggregationDataType
    {
        bool IsOptional { get; }
        bool IsUnique { get; }
    }

    interface IBagDataType : IAggregationDataType
    {
    }

    interface ISetDataType : IAggregationDataType
    {
    }

    interface IConstructedDataType : IDataType
    {
        bool IsExtensible { get; }
    }

    interface IEnumerationDataType : IConstructedDataType
    {
        IEnumerable<IEnumerationDeclaration> Items { get; }
        ITypeReference BasedOn { get; }
    }

    interface ISelectDataType : IConstructedDataType
    {
        IEnumerable<IReferenceDataType> Items { get; }
        ITypeReference BasedOn { get; }
        bool IsGenericIdentity { get; }
    }

    interface IReferenceDataType : IDataType
    {
        IReference Reference { get; }
    }

    interface ISimpleDataType : IDataType
    {
    }

    interface IStringDataType : ISimpleDataType
    {
        IExpression Width { get; }
        bool IsFixed { get; }
    }

    interface IBinaryDataType : ISimpleDataType
    {
        IExpression Width { get; }
        bool IsFixed { get; }
    }

    interface IRealDataType : ISimpleDataType
    {
        IExpression Precision { get; }
    }

    interface IBooleanDataType : ISimpleDataType
    {
    }

    interface ILogicalDataType : ISimpleDataType
    {
    }

    interface IIntegerDataType : ISimpleDataType
    {
    }

    interface INumberDataType : ISimpleDataType
    {
    }

    interface IGenericDataType : IDataType
    {
        ITypeLabelReference TypeLabel { get; }
    }

    interface IGenericEntityDataType : IDataType
    {
        ITypeLabelReference TypeLabel { get; }
    }

    interface IDeclaration
    {
        string Name { get; }
    }

    interface IAlgorithmDeclaration : IDeclaration
    {
        IEnumerable<IDeclaration> Declarations { get; }
        IEnumerable<IConstantDeclaration> Constants { get; }
        IEnumerable<IVariableDeclaration> LocalVariables { get; }
        IEnumerable<IStatement> Statements { get; }
    }

    interface ISchemaDeclaration : IDeclaration
    {
        string SchemaVersionId { get; }
        IEnumerable<IInterfaceSpecification> InterfaceSpecifications { get; }
        IEnumerable<IConstantDeclaration> Constants { get; }
        IEnumerable<IDeclaration> Declarations { get; }
    }

    interface ITypeDeclaration : IDeclaration
    {
        IDataType UnderlyingType { get; }
        IEnumerable<IDomainRuleDeclaration> DomainRules { get; }
    }

    interface IAttributeDeclaration : IDeclaration
    {
        IQualifiedReference RedeclaredAttribute { get; }
        IDataType Type { get; }
    }

    interface IExplicitAttributeDeclaration : IAttributeDeclaration
    {
        bool IsOptional { get; }
    }

    interface IInverseAttributeDeclaration : IAttributeDeclaration
    {
        IEntityReference ForEntity { get; }
        IAttributeReference ForAttribute { get; }
    }

    interface IDerivedAttributeDeclaration : IAttributeDeclaration
    {
        IExpression Value { get; }
    }

    interface IEntityDeclaration : IDeclaration
    {
        bool IsAbstract { get; }
        bool IsAbstractSupertype { get; }
        IEnumerable<IEntityReference> Supertypes { get; }
        ITypeExpression Subtype { get; }
        IEnumerable<IExplicitAttributeDeclaration> ExplicitAttributes { get; }
        IEnumerable<IDerivedAttributeDeclaration> DerivedAttributes { get; }
        IEnumerable<IInverseAttributeDeclaration> InverseAttributes { get; }
        IEnumerable<IUniqueRuleDeclaration> UniqueRules { get; }
        IEnumerable<IDomainRuleDeclaration> DomainRules { get; }
    }

    interface IConstantDeclaration : IDeclaration
    {
        IDataType Type { get; }
        IExpression Value { get; }
    }

    interface IEnumerationDeclaration : IDeclaration
    {
    }

    interface IProcedureDeclaration : IAlgorithmDeclaration
    {
        IEnumerable<IParameterDeclaration> Parameters { get; }
    }

    interface IFunctionDeclaration : IAlgorithmDeclaration
    {
        IEnumerable<IParameterDeclaration> Parameters { get; }
        IDataType ReturnType { get; }
    }

    interface IRuleDeclaration : IAlgorithmDeclaration
    {
        IEnumerable<IVariableDeclaration> Populations { get; }
        IEnumerable<IDomainRuleDeclaration> DomainRules { get; }
    }

    interface ILocalRuleDeclaration : IDeclaration
    {
    }

    interface IDomainRuleDeclaration : ILocalRuleDeclaration
    {
        IExpression Expression { get; }
    }

    interface IUniqueRuleDeclaration : ILocalRuleDeclaration
    {
        IEnumerable<IExpression> Expressions { get; }
    }

    interface IParameterDeclaration : IDeclaration
    {
        IDataType Type { get; }
        bool IsVariable { get; }
    }

    interface IVariableDeclaration : IDeclaration
    {
        IDataType Type { get; }
        IExpression InitialValue { get; }
    }

    interface ISubtypeConstraintDeclaration : IDeclaration
    {
    }

    interface ITypeLabelDeclaration : IDeclaration
    {
    }

    interface IExpression
    {
    }

    interface IBinaryExpression : IExpression
    {
        IExpression Left { get; }
        IExpression Right { get; }
    }

    interface ILessThanExpression : IBinaryExpression
    {
    }

    interface IGreaterThanExpression : IBinaryExpression
    {
    }

    interface ILessThanOrEqualExpression : IBinaryExpression
    {
    }

    interface IGreaterThanOrEqualExpression : IBinaryExpression
    {
    }

    interface INotEqualExpression : IBinaryExpression
    {
    }

    interface IEqualExpression : IBinaryExpression
    {
    }

    interface IInstanceNotEqualExpression : IBinaryExpression
    {
    }

    interface IInstanceEqualExpression : IBinaryExpression
    {
    }

    interface IInExpression : IBinaryExpression
    {
    }

    interface IAdditionExpression : IBinaryExpression
    {
    }

    interface ISubtractionExpression : IBinaryExpression
    {
    }

    interface IOrExpression : IBinaryExpression
    {
    }

    interface IXorExpression : IBinaryExpression
    {
    }

    interface IMultiplicationExpression : IBinaryExpression
    {
    }

    interface IDivisionExpression : IBinaryExpression
    {
    }

    interface IIntegerDivisionExpression : IBinaryExpression
    {
    }

    interface IModuloExpression : IBinaryExpression
    {
    }

    interface IAndExpression : IBinaryExpression
    {
    }

    interface IExponentiationExpression : IBinaryExpression
    {
    }

    interface ILikeExpression : IBinaryExpression
    {
    }

    interface IComplexEntityConstructionExpression : IBinaryExpression
    {
    }

    interface IUnaryExpression : IExpression
    {
        IExpression Operand { get; }
    }

    interface IUnaryPlusExpression : IUnaryExpression
    {
    }

    interface IUnaryMinusExpression : IUnaryExpression
    {
    }

    interface INotExpression : IUnaryExpression
    {
    }

    interface ILiteralExpression : IExpression
    {
    }

    interface IStringLiteral : ILiteralExpression
    {
        string Value { get; }
    }

    interface IIntegerLiteral : ILiteralExpression
    {
        int Value { get; }
    }

    interface IRealLiteral : ILiteralExpression
    {
        double Value { get; }
    }

    interface IBinaryLiteral : ILiteralExpression
    {
        byte[] Value { get; }
    }

    interface IAggregateInitializerExpression : IExpression
    {
        IEnumerable<IAggregateInitializerElement> Elements { get; }
    }

    interface IAggregateInitializerElement
    {
        IExpression Expression { get; }
        IExpression Repetition { get; }
    }

    interface IQueryExpression : IExpression
    {
        IVariableDeclaration VariableDeclaration { get; }
        IExpression Aggregate { get; }
        IExpression Condition { get; }
    }

    interface IIntervalExpression : IExpression
    {
        IExpression Low { get; }
        IntervalComparison LowComparison { get; }
        IExpression Item { get; }
        IntervalComparison HighComparison { get; }
        IExpression High { get; }
    }

    interface IEntityConstructorExpression : IExpression
    {
        IEntityReference Entity { get; }
        IEnumerable<IExpression> Paramaters { get; }
    }

    interface IEnumerationReferenceExpression : IExpression
    {
        ITypeReference Type { get; }
        IEnumerationReference Enumeration { get; }
    }

    interface IConstantReferenceExpression : IExpression
    {
        IConstantReference Constant { get; }
    }

    interface IParameterReferenceExpression : IExpression
    {
        IParameterReference Parameter { get; }
    }

    interface IEntityReferenceExpression : IExpression
    {
        IEntityReference Entity { get; }
    }

    interface IVariableReferenceExpression : IExpression
    {
        IVariableReference Variable { get; }
    }

    interface IFunctionCallExpression : IExpression
    {
        IFunctionReference Function { get; }
        IEnumerable<IExpression> Parameters { get; }
    }

    interface IQualifiedExpression : IExpression
    {
        IExpression Expression { get; }
        IEnumerable<IQualifier> Qualifiers { get; }
    }

    interface IAttributeReferenceExpression : IExpression
    {
        IAttributeReference Attribute { get; }
    }

    interface IStatement
    {
    }

    interface INullStatement : IStatement
    {
    }

    interface IAliasStatement : IStatement
    {
        IVariableDeclaration Variable { get; }
        IQualifiedReference RenamedReference { get; }
    }

    interface IAssignmentStatement : IStatement
    {
        IQualifiedReference Left { get; }
        IExpression Right { get; }
    }

    interface ICaseStatement : IStatement
    {
        IExpression Selector { get; }
        IEnumerable<ICaseAction> Actions { get; }
        IStatement DefaultAction { get; }
    }

    interface ICaseAction : IStatement
    {
        IEnumerable<IExpression> Labels { get; }
        IStatement Statement { get; }
    }

    interface ICompoundStatement : IStatement
    {
        IEnumerable<IStatement> Statements { get; }
    }

    interface IIfStatement : IStatement
    {
        IExpression Condition { get; }
        IEnumerable<IStatement> Statements { get; }
        IEnumerable<IStatement> ElseStatements { get; }
    }

    interface IProcedureCallStatement : IStatement
    {
        IProcedureReference Procedure { get; }
        IEnumerable<IExpression> Parameters { get; }
    }

    interface IRepeatStatement : IStatement
    {
        IVariableDeclaration IncrementVariable { get; }
        IExpression IncrementFrom { get; }
        IExpression IncrementTo { get; }
        IExpression IncrementStep { get; }
        IExpression WhileCondition { get; }
        IExpression UntilCondition { get; }
        IEnumerable<IStatement> Statements { get; }
    }

    interface IReturnStatement : IStatement
    {
        IExpression Expression { get; }
    }

    interface ISkipStatement : IStatement
    {
    }

    interface IEscapeStatement : IStatement
    {
    }

    interface IQualifiedReference
    {
        IReference Reference { get; }
        IEnumerable<IQualifier> Qualifiers { get; }
    }

    interface IQualifier
    {
    }

    interface IAttributeQualifier : IQualifier
    {
        IAttributeReference Attribute { get; }
    }

    interface IGroupQualifier : IQualifier
    {
        IEntityReference Entity { get; }
    }

    interface IIndexQualifier : IQualifier
    {
        IExpression From { get; }
        IExpression To { get; }
    }

    interface IReference
    {
    }

    interface ISchemaReference : IReference
    {
        string SchemaName { get; }
    }

    interface ITypeReference : IReference
    {
        string TypeName { get; }
    }

    interface IEntityReference : IReference
    {
        string EntityName { get; }
    }

    interface IAttributeReference : IReference
    {
        string AttributeName { get; }
    }

    interface IUnresolvedReference : IReference
    {
        string UnresolvedName { get; }
    }

    interface IParameterReference : IReference
    {
        string ParameterName { get; }
    }

    interface IVariableReference : IReference
    {
        string VariableName { get; }
    }

    interface IEnumerationReference : IReference
    {
        string EnumerationName { get; }
    }

    interface IConstantReference : IReference
    {
        string ConstantName { get; }
    }

    interface IFunctionReference : IReference
    {
        string FunctionName { get; }
    }

    interface IProcedureReference : IReference
    {
        string ProcedureName { get; }
    }

    interface ITypeLabelReference : IReference
    {
        string TypeLabelName { get; }
    }

    interface IBounds
    {
        IExpression LowerBound { get; }
        IExpression UpperBound { get; }
    }

    interface ITypeExpression
    {
    }

    interface IAndTypeExpression : ITypeExpression
    {
        ITypeExpression Left { get; }
        ITypeExpression Right { get; }
    }

    interface IAndOrTypeExpression : ITypeExpression
    {
        ITypeExpression Left { get; }
        ITypeExpression Right { get; }
    }

    interface IOneOfTypeExpression : ITypeExpression
    {
        IEnumerable<ITypeExpression> Expressions { get; }
    }

    interface IEntityReferenceTypeExpression : ITypeExpression
    {
        IEntityReference Entity { get; }
    }

    interface IInterfaceSpecification
    {
        ISchemaReference Schema { get; }
        IEnumerable<IReference> References { get; }
    }

    interface IUseClause : IInterfaceSpecification
    {
    }

    interface IReferenceClause : IInterfaceSpecification
    {
    }

    interface IRenamedReference : IReference
    {
        IReference Reference { get; }
        string Name { get; }
    }

}


