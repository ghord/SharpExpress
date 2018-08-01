using SharpExpress.Parsing;
using SharpExpress.TypeSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpExpress.Analysis
{
    class SemanticModelBuilder : SyntaxNodeWalker
    {
        private Dictionary<Expression, TypeInfo> expressionTypes_ = new Dictionary<Expression, TypeInfo>();
        private Dictionary<Reference, ISymbolInfo> referencedSymbols_ = new Dictionary<Reference, ISymbolInfo>();
        private TypeInfo selfType_;
        private Stack<Scope> scopes_ = new Stack<Scope>();
        private IEnumerable<SchemaDeclaration> schemas_;
        private SymbolTable symbolTable_;

        public SemanticModelBuilder(IEnumerable<SchemaDeclaration> schemas)
        {
            schemas_ = schemas;
        }

        public SemanticModel Build()
        {
            //create symbol table (constructs mapping Declaration->ISymbolInfo)
            var builder = new SymbolTableBuilder(schemas_);
            symbolTable_ = builder.Build();

            //initialize state
            scopes_.Clear();
            scopes_.Push(Scope.CreateRootScope(schemas_));

            //visit expressions (constructs mapping Reference->TypeInfo and Exoression->TypeInfo)
            //foreach (var schema in schemas_)
            //    schema.Accept(this);

            //TODO: expressions
            return new SemanticModel(symbolTable_);
        }


        public override void VisitSchemaDeclaration(SchemaDeclaration schemaDeclaration)
        {
            Debug.WriteLine("Begin SchemaDeclaration " + schemaDeclaration.Name);

            scopes_.Push(CurrentScope.CreateChildScope(schemaDeclaration));

            base.VisitSchemaDeclaration(schemaDeclaration);

            scopes_.Pop();

            Debug.WriteLine("End SchemaDeclaration " + schemaDeclaration.Name);
        }

        public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
        {
            Debug.WriteLine("Begin TypeDeclaration " + typeDeclaration.Name);

            PushScope(CurrentScope.CreateChildScope(typeDeclaration));

            base.VisitTypeDeclaration(typeDeclaration);

            PopScope();

            Debug.WriteLine("End TypeDeclaration " + typeDeclaration.Name);
        }

        public override void VisitProcedureDeclaration(ProcedureDeclaration procedureDeclaration)
        {
            PushScope(CurrentScope.CreateChildScope(procedureDeclaration));

            base.VisitProcedureDeclaration(procedureDeclaration);

            PopScope();
        }

        private void PushScope(Scope scope)
        {
            scopes_.Push(scope);
        }

        private void PopScope()
        {
            scopes_.Pop();
        }

        public override void VisitEntityDeclaration(EntityDeclaration entityDeclaration)
        {
            Debug.WriteLine("Begin EntityDeclaration " + entityDeclaration.Name);

            scopes_.Push(scopes_.Peek().CreateChildScope(entityDeclaration));

            selfType_ = symbolTable_.GetEntity(entityDeclaration);

            base.VisitEntityDeclaration(entityDeclaration);

            selfType_ = null;

            scopes_.Pop();

            Debug.WriteLine("End EntityDeclaration " + entityDeclaration.Name);
        }

        public override void VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
        {
            scopes_.Push(scopes_.Peek().CreateChildScope(functionDeclaration));

            base.VisitFunctionDeclaration(functionDeclaration);

            scopes_.Pop();
        }

        public Scope CurrentScope => scopes_.Peek();

        public override void VisitInverseAttributeDeclaration(InverseAttributeDeclaration inverseAttributeDeclaration)
        {
            inverseAttributeDeclaration.RedeclaredAttribute?.Accept(this);
            inverseAttributeDeclaration.Type?.Accept(this);

            EntityReference attributeEntity;

            if (inverseAttributeDeclaration.ForEntity != null)
            {
                inverseAttributeDeclaration.ForEntity.Accept(this);

                attributeEntity = inverseAttributeDeclaration.ForEntity;
            }
            else
            {
                switch (inverseAttributeDeclaration.Type)
                {
                    case SetDataType set when set.ElementType is ReferenceDataType reference
                                           && reference.Reference is EntityReference entityRef:
                        attributeEntity = entityRef;
                        break;
                    case BagDataType bag when bag.ElementType is ReferenceDataType reference
                                           && reference.Reference is EntityReference entityRef:
                        attributeEntity = entityRef;
                        break;
                    case ReferenceDataType reference when reference.Reference is EntityReference entityRef:
                        attributeEntity = entityRef;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            var scopeEntity = (EntityDeclaration)CurrentScope.ResolveEntity(attributeEntity.EntityName);
            // var scopeEntity = symbolTable_.GetEntity(attributeEntity);
            if (scopeEntity == null)
                throw new NotImplementedException();

            throw new NotImplementedException();
            //scopes_.Push(CurrentScope.CreateQualifiedScope(scopeEntity));

            //inverseAttributeDeclaration.ForAttribute?.Accept(this);

            //scopes_.Pop();
        }

        protected override void DefaultVisit(SyntaxNode node)
        {
#if DEBUG
            if (node is Expression)
                throw new NotImplementedException("Expression(" + node.Kind + ")");
#endif
            base.DefaultVisit(node);
        }

        public override void VisitInExpression(InExpression inExpression)
        {
            inExpression.Left?.Accept(this);
            inExpression.Right?.Accept(this);

            expressionTypes_[inExpression] = TypeInfo.Boolean;
        }

        public override void VisitIntegerLiteral(IntegerLiteral integerLiteral)
        {
            expressionTypes_[integerLiteral] = TypeInfo.Integer;
        }


        public override void VisitAggregateInitializerExpression(AggregateInitializerExpression aggregateInitializerExpression)
        {
            foreach (var element in aggregateInitializerExpression.Elements)
            {
                element.Accept(this);
            }

            var aggregate = new AggregateTypeInfo();
            aggregate.SetElementType(TypeInfo.Generic);

            expressionTypes_[aggregateInitializerExpression] = aggregate;
        }

        public override void VisitRuleDeclaration(RuleDeclaration ruleDeclaration)
        {
            scopes_.Push(scopes_.Peek().CreateChildScope(ruleDeclaration));

            base.VisitRuleDeclaration(ruleDeclaration);

            scopes_.Pop();
        }

        public override void VisitAliasStatement(AliasStatement aliasStatement)
        {
            scopes_.Push(scopes_.Peek().CreateChildScope(aliasStatement));

            base.VisitAliasStatement(aliasStatement);

            scopes_.Pop();
        }

        public override void VisitQueryExpression(QueryExpression queryExpression)
        {
            scopes_.Push(scopes_.Peek().CreateChildScope(queryExpression));

            base.VisitQueryExpression(queryExpression);

            scopes_.Pop();
        }

        public override void VisitRepeatStatement(RepeatStatement repeatStatement)
        {
            scopes_.Push(scopes_.Peek().CreateChildScope(repeatStatement));

            base.VisitRepeatStatement(repeatStatement);

            scopes_.Pop();

        }

        public override void VisitSubtypeConstraintDeclaration(SubtypeConstraintDeclaration subtypeConstraintDeclaration)
        {
            scopes_.Push(scopes_.Peek().CreateChildScope(subtypeConstraintDeclaration));

            base.VisitSubtypeConstraintDeclaration(subtypeConstraintDeclaration);

            scopes_.Pop();
        }



        public override void VisitQualifiedExpression(QualifiedExpression qualifiedExpression)
        {
            qualifiedExpression.Expression?.Accept(this);

            var expressionType = expressionTypes_[qualifiedExpression.Expression];

            foreach (var qualifier in qualifiedExpression.Qualifiers)
            {
                switch (qualifier)
                {
                    case IndexQualifier indexQualifier:

                        qualifier.Accept(this);

                        if (expressionType.ElementType != null)
                            expressionType = expressionType.ElementType;
                        else
                        {
                            //indexing nonCollection
                            throw new NotImplementedException();
                        }

                        break;

                    case AttributeQualifier attributeQualifier:

                        var attribute = expressionType.GetAttribute(attributeQualifier.Attribute.AttributeName, false);

                        if (attribute != null)
                        {
                            expressionType = attribute.Type;

                            referencedSymbols_[attributeQualifier.Attribute] = attribute;
                        }
                        else
                        {
                            //unresolved attribute name
                            throw new NotImplementedException();
                        }

                        break;

                    case GroupQualifier groupQualifier:

                        TypeInfo findAncestor(TypeInfo typeInfo, string name)
                        {
                            if (typeInfo.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                                return typeInfo;

                            foreach(var baseType in typeInfo.BaseTypes)
                            {
                                if (findAncestor(baseType, name) is TypeInfo result)
                                    return result;
                            }

                            return null;
                        }

                        //find ancestor
                        var ancestorType = findAncestor(expressionType, groupQualifier.Entity.EntityName);

                        if(ancestorType != null)
                        {
                            expressionType = ancestorType;
                        }
                        else
                        {
                            //cannot find ancestory type
                            throw new NotImplementedException();
                        }

                        break;

                    default:
                        throw new NotImplementedException();

                }
            }

            expressionTypes_[qualifiedExpression] = expressionType;
        }

        private DataType GetElementType(DataType dataType)
        {
            while (dataType != null)
            {

                switch (dataType)
                {
                    case AggregationDataType aggregation:
                        return aggregation.ElementType;
                    case ReferenceDataType refType when refType.Reference is TypeReference typeRef:
                        dataType = (DataType)CurrentScope.ResolveType(typeRef.TypeName).UnderlyingType;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            return null;
        }

        public override void VisitSchemaReference(SchemaReference schemaReference)
        {
            throw new NotImplementedException();
        }

        public override void VisitParameterReference(ParameterReference parameterReference)
        {
            throw new NotImplementedException();
        }

        public override void VisitTypeLabelReference(TypeLabelReference typeLabelReference)
        {
            throw new NotImplementedException();
        }

        public override void VisitUnresolvedReference(UnresolvedReference unresolvedReference)
        {
            throw new NotImplementedException();
        }

        public override void VisitVariableReference(VariableReference variableReference)
        {
            throw new NotImplementedException();
        }

        public override void VisitProcedureReference(ProcedureReference procedureReference)
        {
            throw new NotImplementedException();
        }

        public override void VisitConstantReferenceExpression(ConstantReferenceExpression constantReferenceExpression)
        {
            if (Keywords.IsBuiltInConstant(constantReferenceExpression.Constant.ConstantName))
            {
                switch (constantReferenceExpression.Constant.ConstantName)
                {
                    case Keywords.Self:
                        expressionTypes_.Add(constantReferenceExpression, selfType_);
                        break;

                    case "?":
                        expressionTypes_.Add(constantReferenceExpression, null);
                        break;

                    default:
                        throw new NotImplementedException(constantReferenceExpression.Constant.ConstantName);
                }
            }
        }
    }
}
