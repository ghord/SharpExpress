using SharpExpress.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    //TODO: emit errors on invalid references/expressions etc..

    class SyntaxAnalysisPass : ParserPass
    {
        public SyntaxAnalysisPass(Token[] tokens, IList<ParsingError> errors)
            : base(tokens, errors)
        {
        }

        public override void Run(SyntaxTreeBuilder builder)
        {
            var scope = Scope.CreateRootScope(builder.Schemas);

            Recover(() =>
            {
                foreach (var schema in builder.Schemas)
                {
                    VisitSchema(schema, scope);
                }

            }, TokenKind.Eof);

            Expect(TokenKind.Eof);
        }

        private void VisitSchema(SchemaDeclarationBuilder schema, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Schema);

            Recover(() =>
            {
                Expect(TokenKind.SimpleId, schema.Name);

                ParseStringLiteral();

                var childScope = scope.CreateChildScope(schema);

                Expect(TokenKind.Semicolon);

                foreach (var specification in schema.InterfaceSpecifications)
                {
                    VisitInterfaceSpecification(specification, scope);
                }

                VisitConstants(schema.Constants, childScope);

                foreach (var declaration in schema.Declarations)
                {
                    if (declaration is RuleDeclarationBuilder rule)
                    {
                        VisitRule(rule, childScope);
                    }
                    else
                    {
                        VisitDeclaration(declaration, childScope);
                    }
                }



            }, TokenKind.Keyword, Keywords.EndSchema);

            Expect(TokenKind.Keyword, Keywords.EndSchema);
            Expect(TokenKind.Semicolon);
        }

        private void VisitDeclaration(DeclarationBuilder declaration, Scope scope)
        {
            switch (declaration)
            {
                case TypeDeclarationBuilder type:
                    VisitType(type, scope);
                    break;
                case EntityDeclarationBuilder entity:
                    VisitEntity(entity, scope);
                    break;
                case ProcedureDeclarationBuilder procedure:
                    VisitProcedure(procedure, scope);
                    break;
                case FunctionDeclarationBuilder function:
                    VisitFunction(function, scope);
                    break;
                case SubtypeConstraintDeclarationBuilder subtypeConstraint:
                    VisitSubtypeConstraint(subtypeConstraint, scope);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void VisitSubtypeConstraint(SubtypeConstraintDeclarationBuilder subtypeConstraint, Scope scope)
        {
            throw new NotImplementedException(nameof(VisitSubtypeConstraint));
        }

        private void VisitFunction(FunctionDeclarationBuilder function, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Function);

            Recover(() =>
            {
                Expect(TokenKind.SimpleId, function.Name);

                VisitParameters(function.Parameters, scope);

                Expect(TokenKind.Colon);

                var functionScope = scope.CreateChildScope(function);

                VisitParameterType(function.ReturnType, functionScope);

                Expect(TokenKind.Semicolon);

                VisitAlgorithmHead(function, functionScope);

                function.Statements.Add(ParseStatement(functionScope));

                while (TryParseStatement(functionScope) is StatementBuilder statement)
                {
                    function.Statements.Add(statement);
                }

            }, TokenKind.Keyword, Keywords.EndFunction);

            Expect(TokenKind.Keyword, Keywords.EndFunction);
            Expect(TokenKind.Semicolon);
        }



        private void VisitAlgorithmHead(AlgorithmDeclarationBuilder algorithm, Scope scope)
        {
            foreach (var declaration in algorithm.Declarations)
            {
                VisitDeclaration(declaration, scope);
            }

            VisitConstants(algorithm.Constants, scope);

            VisitLocalVariables(algorithm.LocalVariables, scope);
        }

        private void VisitLocalVariables(List<VariableDeclarationBuilder> localVariables, Scope scope)
        {
            if (localVariables.Count > 0)
            {
                Expect(TokenKind.Keyword, Keywords.Local);

                Recover(() =>
                {
                    for (int i = 0; i < localVariables.Count; i++)
                    {
                        int idx = i;

                        Expect(TokenKind.SimpleId, localVariables[i].Name);

                        while (i + 1 < localVariables.Count && localVariables[i].Type == localVariables[i + 1].Type)
                        {
                            Expect(TokenKind.Comma);
                            Expect(TokenKind.SimpleId, localVariables[++i].Name);
                        }

                        Expect(TokenKind.Colon);

                        VisitParameterType(localVariables[i].Type, scope);

                        if (Accept(TokenKind.Assignment))
                        {
                            var expression = ParseExpression(scope);

                            while (idx <= i)
                            {
                                localVariables[idx].InitialValue = expression;
                                idx++;
                            }
                        }

                        Expect(TokenKind.Semicolon);
                    }

                }, TokenKind.Keyword, Keywords.EndLocal);

                Expect(TokenKind.Keyword, Keywords.EndLocal);
                Expect(TokenKind.Semicolon);
            }
        }


        private void VisitParameters(IList<ParameterDeclarationBuilder> parameters, Scope scope)
        {
            if (parameters.Count > 0)
            {
                Expect(TokenKind.LeftParen);

                for (int i = 0; i < parameters.Count; i++)
                {
                    if (parameters[i].IsVariable)
                        Expect(TokenKind.Keyword, Keywords.Var);

                    Expect(TokenKind.SimpleId, parameters[i].Name);

                    while (i + 1 < parameters.Count && parameters[i].Type == parameters[i + 1].Type)
                    {
                        Expect(TokenKind.Comma);
                        Expect(TokenKind.SimpleId, parameters[++i].Name);
                    }

                    Expect(TokenKind.Colon);

                    VisitParameterType(parameters[i].Type, scope);

                    if (i != parameters.Count - 1)
                        Expect(TokenKind.Semicolon);
                }

                Expect(TokenKind.RightParen);
            }
        }

        private void VisitProcedure(ProcedureDeclarationBuilder procedure, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Procedure);

            Recover(() =>
            {
                throw new NotImplementedException(nameof(VisitProcedure));

            }, TokenKind.Keyword, Keywords.EndProcedure);

            Expect(TokenKind.Keyword, Keywords.EndProcedure);
            Expect(TokenKind.Semicolon);
        }

        private void VisitEntity(EntityDeclarationBuilder entity, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Entity);

            Recover(() =>
            {
                Expect(TokenKind.SimpleId, entity.Name);

                VisitSupertypeConstraint(entity, scope);
                VisitSubtypeDeclaration(entity, scope);

                Expect(TokenKind.Semicolon);
                var entityScope = scope.CreateChildScope(entity);

                VisitExplicitAttributes(entity.ExplicitAttributes, entityScope);
                VisitDerivedAttributes(entity.DerivedAttributes, entityScope);
                VisitInverseAttributes(entity.InverseAttributes, entityScope);

                VisitUniqueRules(entity.UniqueRules, entityScope);
                VisitDomainRules(entity.DomainRules, entityScope);


            }, TokenKind.Keyword, Keywords.EndEntity);

            Expect(TokenKind.Keyword, Keywords.EndEntity);
            Expect(TokenKind.Semicolon);
        }

        private void VisitUniqueRules(IList<UniqueRuleDeclarationBuilder> uniqueRules, Scope scope)
        {
            if (uniqueRules.Count > 0)
            {
                Expect(TokenKind.Keyword, Keywords.Unique);

                foreach (var rule in uniqueRules)
                {
                    if (rule.Name != null)
                    {
                        Expect(TokenKind.SimpleId, rule.Name);
                        Expect(TokenKind.Colon);
                    }

                    //TODO: narrow to only attributeRef or qualified attribute
                    rule.Expressions.Add(ParseExpression(scope));

                    while (Accept(TokenKind.Comma))
                        rule.Expressions.Add(ParseExpression(scope));

                    Expect(TokenKind.Semicolon);
                }
            }
        }

        private void VisitInverseAttributes(IList<InverseAttributeDeclarationBuilder> attributes, Scope scope)
        {
            if (attributes.Count > 0)
            {
                Expect(TokenKind.Keyword, Keywords.Inverse);

                foreach (var attribute in attributes)
                {
                    VisitAttributeDeclaration(attribute);

                    Expect(TokenKind.Colon);

                    switch (attribute.Type)
                    {
                        case BagDataTypeBuilder bagType:
                            VisitBagType(bagType, false, scope);
                            break;
                        case SetDataTypeBuilder setType:
                            VisitSetType(setType, false, scope);
                            break;
                        case ReferenceDataTypeBuilder entityType when entityType.Reference is EntityReferenceBuilder entityRef:
                            Expect(TokenKind.SimpleId, entityRef.EntityName);
                            break;
                        default:
                            throw new NotSupportedException();
                    }

                    Expect(TokenKind.Keyword, Keywords.For);

                    if (attribute.ForEntity != null)
                        Expect(TokenKind.SimpleId, attribute.ForEntity.EntityName);

                    Expect(TokenKind.SimpleId, attribute.ForAttribute.AttributeName);
                    Expect(TokenKind.Semicolon);
                }
            }
        }

        private void VisitDerivedAttributes(IList<DerivedAttributeDeclarationBuilder> attributes, Scope scope)
        {
            if (attributes.Count > 0)
            {
                Expect(TokenKind.Keyword, Keywords.Derive);

                foreach (var attribute in attributes)
                {
                    VisitAttributeDeclaration(attribute);

                    Expect(TokenKind.Colon);

                    VisitParameterType(attribute.Type, scope);

                    Expect(TokenKind.Assignment);

                    attribute.Value = ParseExpression(scope);

                    Expect(TokenKind.Semicolon);
                }
            }
        }

        private void VisitExplicitAttributes(IList<ExplicitAttributeDeclarationBuilder> attributes, Scope scope)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                VisitAttributeDeclaration(attributes[i]);

                while (i + 1 < attributes.Count && attributes[i].Type == attributes[i + 1].Type)
                {
                    Expect(TokenKind.Comma);
                    VisitAttributeDeclaration(attributes[++i]);
                }

                Expect(TokenKind.Colon);

                if (attributes[i].IsOptional)
                    Expect(TokenKind.Keyword, Keywords.Optional);

                VisitParameterType(attributes[i].Type, scope);

                Expect(TokenKind.Semicolon);
            }
        }

        private void VisitAttributeDeclaration(AttributeDeclarationBuilder attribute)
        {
            if (attribute.RedeclaredAttribute != null)
            {
                Expect(TokenKind.Keyword, Keywords.Self);

                foreach (var qualifier in attribute.RedeclaredAttribute.Qualifiers)
                {
                    VisitQualifier(qualifier);
                }
            }
            else
            {
                Expect(TokenKind.SimpleId, attribute.Name);
            }
        }


        private void VisitQualifier(QualifierBuilder qualifier)
        {
            switch (qualifier)
            {
                case AttributeQualifierBuilder attribute:
                    VisitAttributeQualifier(attribute);
                    break;
                case GroupQualifierBuilder group:
                    VisitGroupQualifier(group);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void VisitAttributeQualifier(AttributeQualifierBuilder qualifier)
        {
            Expect(TokenKind.Period);
            Expect(TokenKind.SimpleId, qualifier.Attribute.AttributeName);
        }

        private void VisitGroupQualifier(GroupQualifierBuilder qualifier)
        {
            Expect(TokenKind.Backslash);
            Expect(TokenKind.SimpleId, qualifier.Entity.EntityName);
        }

        private void VisitSubtypeDeclaration(EntityDeclarationBuilder entity, Scope scope)
        {
            if (entity.Supertypes.Count > 0)
            {
                Expect(TokenKind.Keyword, Keywords.Subtype);

                Expect(TokenKind.Keyword, Keywords.Of);

                Expect(TokenKind.LeftParen);

                Expect(TokenKind.SimpleId, entity.Supertypes.First().EntityName);

                foreach (var supertype in entity.Supertypes.Skip(1))
                {
                    Expect(TokenKind.Comma);

                    Expect(TokenKind.SimpleId, supertype.EntityName);
                }

                Expect(TokenKind.RightParen);
            }
        }

        private void VisitSupertypeConstraint(EntityDeclarationBuilder entity, Scope scope)
        {
            if (entity.IsAbstract)
            {
                Expect(TokenKind.Keyword, Keywords.Abstract);
            }

            if (entity.Subtype != null || entity.IsAbstractSupertype)
            {
                Expect(TokenKind.Keyword, Keywords.Supertype);

                if (entity.Subtype != null)
                {
                    Expect(TokenKind.Keyword, Keywords.Of);

                    SkipTypeExpression();
                }
            }
        }

        private void SkipTypeExpression()
        {
            SkipTypeFactor();

            while (Accept(TokenKind.Keyword, Keywords.AndOr))
            {
                SkipTypeFactor();
            }
        }

        private void SkipTypeFactor()
        {
            SkipTypeTerm();

            while (Accept(TokenKind.Keyword, Keywords.And))
            {
                SkipTypeTerm();
            }
        }

        private void SkipTypeTerm()
        {
            if (Accept(TokenKind.Keyword, Keywords.Oneof))
            {
                Expect(TokenKind.LeftParen);

                SkipTypeExpression();

                while (Accept(TokenKind.Comma))
                    SkipTypeExpression();

                Expect(TokenKind.RightParen);

            }
            else if (Accept(TokenKind.LeftParen))
            {
                SkipTypeExpression();

                Expect(TokenKind.RightParen);
            }
            else
            {
                Expect(TokenKind.SimpleId);
            }
        }


        private void VisitType(TypeDeclarationBuilder type, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Type);

            Recover(() =>
            {
                Expect(TokenKind.SimpleId, type.Name);
                Expect(TokenKind.Equal);

                VisitUnderlyingType(type.UnderlyingType, scope);

                Expect(TokenKind.Semicolon);

                var typeScope = scope.CreateChildScope(type);

                VisitDomainRules(type.DomainRules, typeScope);

            }, TokenKind.Keyword, Keywords.EndType);

            Expect(TokenKind.Keyword, Keywords.EndType);
            Expect(TokenKind.Semicolon);
        }

        private void VisitDomainRules(List<DomainRuleDeclarationBuilder> rules, Scope scope)
        {
            if (rules.Count > 0)
            {
                Expect(TokenKind.Keyword, Keywords.Where);

                foreach (var rule in rules)
                {
                    VisitDomainRule(rule, scope);
                }
            }
        }

        private void VisitDomainRule(DomainRuleDeclarationBuilder localRule, Scope scope)
        {
            if (localRule.Name != null)
            {
                Expect(TokenKind.SimpleId, localRule.Name);
                Expect(TokenKind.Colon);
            }

            localRule.Expression = ParseExpression(scope);

            Expect(TokenKind.Semicolon);
        }

        private void VisitUnderlyingType(DataTypeBuilder underlyingType, Scope scope)
        {
            switch (underlyingType)
            {
                case SimpleDataTypeBuilder simpleType:
                    VisitSimpleType(simpleType, scope);
                    break;
                case AggregationDataTypeBuilder aggregationType:
                    VisitAggregationType(aggregationType, false, scope);
                    break;
                case ConstructedDataTypeBuilder constructedType:
                    VisitConstructedType(constructedType, scope);
                    break;
                case ReferenceDataTypeBuilder referenceType:
                    VisitReferenceType(referenceType, false, scope);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void VisitReferenceType(ReferenceDataTypeBuilder referenceType, bool allowEntity, Scope scope)
        {
            TryResolveNamedTypeReferences(referenceType, scope);

            if (referenceType.Reference is TypeReferenceBuilder typeRef)
            {
                Expect(TokenKind.SimpleId, typeRef.TypeName);
                return;
            }

            else if (allowEntity)
            {
                if (referenceType.Reference is EntityReferenceBuilder entityRef)
                {
                    Expect(TokenKind.SimpleId, entityRef.EntityName);
                    return;
                }
            }

            //TODO: unresolved data type
            throw new NotImplementedException(nameof(VisitReferenceType));
        }

        private void TryResolveNamedTypeReferences(ReferenceDataTypeBuilder referenceType, Scope scope)
        {
            if (referenceType.Reference is UnresolvedReferenceBuilder unresolved)
            {
                var resolvedType = scope.ResolveType(unresolved.UnresolvedName);
                if (resolvedType != null)
                {
                    referenceType.Reference = new TypeReferenceBuilder
                    {
                        TypeName = resolvedType.Name
                    };
                }
                else
                {
                    var resolvedEntity = scope.ResolveEntity(unresolved.UnresolvedName);
                    if (resolvedEntity != null)
                    {
                        referenceType.Reference = new EntityReferenceBuilder
                        {
                            EntityName = resolvedEntity.Name
                        };
                    }
                }
            }
        }

        private void VisitConstructedType(ConstructedDataTypeBuilder constructedType, Scope scope)
        {
            if (constructedType.IsExtensible)
                Expect(TokenKind.Keyword, Keywords.Extensible);

            switch (constructedType)
            {
                case SelectDataTypeBuilder selectType:
                    VisitSelectType(selectType, scope);
                    break;
                case EnumerationDataTypeBuilder enumerationType:
                    VisitEnumerationType(enumerationType, scope);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void VisitEnumerationType(EnumerationDataTypeBuilder enumerationType, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Enumeration);

            if (enumerationType.BasedOn != null)
            {
                Expect(TokenKind.Keyword, Keywords.BasedOn);

                Expect(TokenKind.SimpleId, enumerationType.BasedOn.TypeName);

                if (enumerationType.Items.Count > 0)
                {
                    Expect(TokenKind.Keyword, Keywords.With);
                }
            }
            else
            {
                Expect(TokenKind.Keyword, Keywords.Of);
            }

            if (enumerationType.Items.Count > 0)
            {
                Expect(TokenKind.LeftParen);

                Expect(TokenKind.SimpleId, enumerationType.Items.First().Name);

                foreach (var item in enumerationType.Items.Skip(1))
                {
                    Expect(TokenKind.Comma);
                    Expect(TokenKind.SimpleId, item.Name);
                }

                Expect(TokenKind.RightParen);
            }
        }

        private void VisitSelectType(SelectDataTypeBuilder selectType, Scope scope)
        {
            if (selectType.IsGenericIdentity)
                Expect(TokenKind.Keyword, Keywords.GenericEntity);

            Expect(TokenKind.Keyword, Keywords.Select);

            if (selectType.BasedOn != null)
            {
                Expect(TokenKind.Keyword, Keywords.BasedOn);

                Expect(TokenKind.SimpleId, selectType.BasedOn.TypeName);

                if (selectType.Items.Count > 0)
                    Expect(TokenKind.Keyword, Keywords.With);
            }

            if (selectType.Items.Count > 0)
            {
                Expect(TokenKind.LeftParen);

                VisitReferenceType(selectType.Items.First(), true, scope);

                foreach (var item in selectType.Items.Skip(1))
                {
                    Expect(TokenKind.Comma);

                    VisitReferenceType(item, true, scope);
                }

                Expect(TokenKind.RightParen);
            }
        }

        private void VisitAggregationType(AggregationDataTypeBuilder aggregationType, bool allowGeneral, Scope scope)
        {
            switch (aggregationType)
            {
                case ListDataTypeBuilder listType:
                    VisitListType(listType, allowGeneral, scope);
                    break;
                case ArrayDataTypeBuilder arrayType:
                    VisitArrayType(arrayType, allowGeneral, scope);
                    break;
                case BagDataTypeBuilder bagType:
                    VisitBagType(bagType, allowGeneral, scope);
                    break;
                case SetDataTypeBuilder setType:
                    VisitSetType(setType, allowGeneral, scope);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void VisitSetType(SetDataTypeBuilder setType, bool allowGeneral, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Set);

            setType.Bounds = TryParseBounds(scope);

            Expect(TokenKind.Keyword, Keywords.Of);

            if (allowGeneral)
                VisitParameterType(setType.ElementType, scope);
            else
                VisitInstantiableType(setType.ElementType, scope);
        }

        private void VisitBagType(BagDataTypeBuilder bagType, bool allowGeneral, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Bag);

            bagType.Bounds = TryParseBounds(scope);

            Expect(TokenKind.Keyword, Keywords.Of);

            if (allowGeneral)
                VisitParameterType(bagType.ElementType, scope);
            else
                VisitInstantiableType(bagType.ElementType, scope);
        }

        private void VisitArrayType(ArrayDataTypeBuilder arrayType, bool allowGeneral, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Array);

            if (allowGeneral)
                arrayType.Bounds = TryParseBounds(scope);
            else
                arrayType.Bounds = ParseBounds(scope);

            Expect(TokenKind.Keyword, Keywords.Of);

            if (arrayType.IsOptional)
                Expect(TokenKind.Keyword, Keywords.Optional);

            if (arrayType.IsUnique)
                Expect(TokenKind.Keyword, Keywords.Unique);

            if (allowGeneral)
                VisitParameterType(arrayType.ElementType, scope);
            else
                VisitInstantiableType(arrayType.ElementType, scope);
        }

        private void VisitListType(ListDataTypeBuilder listType, bool allowGeneral, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.List);

            listType.Bounds = TryParseBounds(scope);

            Expect(TokenKind.Keyword, Keywords.Of);

            if (listType.IsUnique)
                Expect(TokenKind.Keyword, Keywords.Unique);

            if (allowGeneral)
                VisitParameterType(listType.ElementType, scope);
            else
                VisitInstantiableType(listType.ElementType, scope);
        }

        private void VisitParameterType(DataTypeBuilder parameterType, Scope scope)
        {
            switch (parameterType)
            {
                case SimpleDataTypeBuilder simpleType:
                    VisitSimpleType(simpleType, scope);
                    break;
                case ReferenceDataTypeBuilder referenceType:
                    VisitReferenceType(referenceType, true, scope);
                    break;
                case AggregateDataTypeBuilder aggregateType:
                    VisitAggregateType(aggregateType, scope);
                    break;
                case AggregationDataTypeBuilder aggregationType:
                    VisitAggregationType(aggregationType, true, scope);
                    break;
                case GenericEntityDataTypeBuilder genericEntityType:
                    VisitGenericEntityType(genericEntityType, scope);
                    break;
                case GenericDataTypeBuilder genericType:
                    VisitGenericType(genericType, scope);

                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void VisitGenericEntityType(GenericEntityDataTypeBuilder genericEntityType, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.GenericEntity);

            if (genericEntityType.TypeLabel != null)
            {
                Expect(TokenKind.Colon);
                Expect(TokenKind.SimpleId, genericEntityType.TypeLabel.TypeLabelName);
            }
        }

        private void VisitGenericType(GenericDataTypeBuilder genericEntityType, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Generic);

            if (genericEntityType.TypeLabel != null)
            {
                Expect(TokenKind.Colon);
                Expect(TokenKind.SimpleId, genericEntityType.TypeLabel.TypeLabelName);
            }
        }


        private void VisitAggregateType(AggregateDataTypeBuilder aggregateType, Scope scope)
        {
            throw new NotImplementedException(nameof(VisitAggregateType));
        }

        private void VisitInstantiableType(DataTypeBuilder instantiableType, Scope scope)
        {
            switch (instantiableType)
            {
                case SimpleDataTypeBuilder simpleType:
                    VisitSimpleType(simpleType, scope);
                    break;
                case AggregationDataTypeBuilder aggregationType:
                    VisitAggregationType(aggregationType, false, scope);
                    break;
                case ConstructedDataTypeBuilder constructedType:
                    VisitConstructedType(constructedType, scope);
                    break;
                case ReferenceDataTypeBuilder referenceType:
                    VisitReferenceType(referenceType, true, scope);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private BoundsBuilder TryParseBounds(Scope scope)
        {
            if (Accept(TokenKind.LeftBracket))
            {
                var lowerBound = ParseNumericExpression(scope);

                Expect(TokenKind.Colon);

                var upperBound = ParseNumericExpression(scope);

                Expect(TokenKind.RightBracket);

                return new BoundsBuilder
                {
                    LowerBound = lowerBound,
                    UpperBound = upperBound
                };
            }

            return null;
        }

        private BoundsBuilder ParseBounds(Scope scope)
        {
            var result = TryParseBounds(scope);

            if (result == null)
                Expect(TokenKind.LeftBracket);

            return result;
        }

        private void VisitSimpleType(SimpleDataTypeBuilder simpleType, Scope scope)
        {
            switch (simpleType)
            {
                case StringDataTypeBuilder stringType:
                    VisitStringType(stringType, scope);
                    break;
                case BinaryDataTypeBuilder binaryType:
                    VisitBinaryType(binaryType, scope);
                    break;
                case RealDataTypeBuilder realType:
                    VisitRealType(realType, scope);
                    break;
                case BooleanDataTypeBuilder booleanType:
                    Expect(TokenKind.Keyword, Keywords.Boolean);
                    break;
                case LogicalDataTypeBuilder logicalType:
                    Expect(TokenKind.Keyword, Keywords.Logical);
                    break;
                case IntegerDataTypeBuilder integerType:
                    Expect(TokenKind.Keyword, Keywords.Integer);
                    break;
                case NumberDataTypeBuilder numberType:
                    Expect(TokenKind.Keyword, Keywords.Number);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void VisitRealType(RealDataTypeBuilder realType, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Real);

            if (Accept(TokenKind.LeftParen))
            {
                realType.Precision = ParseNumericExpression(scope);

                Expect(TokenKind.RightParen);
            }
        }


        private void VisitBinaryType(BinaryDataTypeBuilder binaryType, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Binary);

            if (Accept(TokenKind.LeftParen))
            {
                binaryType.Width = ParseExpression(scope);

                Expect(TokenKind.RightParen);

                if (binaryType.IsFixed)
                    Expect(TokenKind.Keyword, Keywords.Fixed);
            }
        }

        private void VisitStringType(StringDataTypeBuilder stringType, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.String);

            if (Accept(TokenKind.LeftParen))
            {
                stringType.Width = ParseExpression(scope);

                Expect(TokenKind.RightParen);

                if (stringType.IsFixed)
                {
                    Expect(TokenKind.Keyword, Keywords.Fixed);
                }
            }
        }

        private void VisitRule(RuleDeclarationBuilder rule, Scope scope)
        {
            Expect(TokenKind.Keyword, Keywords.Rule);

            Recover(() =>
            {
                Expect(TokenKind.SimpleId, rule.Name);

                Expect(TokenKind.Keyword, Keywords.For);

                Expect(TokenKind.LeftParen);

                Expect(TokenKind.SimpleId, rule.Populations.First().Name);

                foreach(var population in rule.Populations.Skip(1))
                {
                    Expect(TokenKind.Comma);
                    Expect(TokenKind.SimpleId, population.Name);
                }

                Expect(TokenKind.RightParen);

                Expect(TokenKind.Semicolon);

                var ruleScope = scope.CreateChildScope(rule);

                VisitAlgorithmHead(rule, ruleScope);

                while (TryParseStatement(ruleScope) is StatementBuilder statement)
                {
                    rule.Statements.Add(statement);
                }

                VisitDomainRules(rule.DomainRules, ruleScope);

            }, TokenKind.Keyword, Keywords.EndRule);

            Expect(TokenKind.Keyword, Keywords.EndRule);
            Expect(TokenKind.Semicolon);
        }

        private void VisitConstants(List<ConstantDeclarationBuilder> constants, Scope scope)
        {
            if (constants.Count > 0)
            {
                Expect(TokenKind.Keyword, Keywords.Constant);

                Recover(() =>
                {
                    foreach (var constant in constants)
                        VisitConstant(constant, scope);

                }, TokenKind.Keyword, Keywords.EndConstant);

                Expect(TokenKind.Keyword, Keywords.EndConstant);
                Expect(TokenKind.Semicolon);
            }
        }

        private void VisitConstant(ConstantDeclarationBuilder constant, Scope scope)
        {
            throw new NotImplementedException(nameof(VisitConstants));
        }



        //private ProcedureDeclaration ParseProcedureDeclaration()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Procedure))
        //    {
        //        var result = new ProcedureDeclaration();

        //        Recover(() =>
        //        {
        //            throw new NotImplementedException(nameof(ProcedureDeclaration));

        //        }, TokenKind.Keyword, Keywords.EndProcedure);

        //        Expect(TokenKind.Keyword, Keywords.EndProcedure);
        //        Expect(TokenKind.Semicolon);

        //        return result;
        //    }

        //    return null;
        //}

        //private SubtypeConstraintDeclaration ParseSubtypeConstraintDeclaration()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.SubtypeConstraint))
        //    {
        //        var result = new SubtypeConstraintDeclaration();

        //        Recover(() =>
        //        {
        //            throw new NotImplementedException(nameof(SubtypeConstraintDeclaration));
        //        }, TokenKind.Keyword, Keywords.EndProcedure);

        //        Expect(TokenKind.Keyword, Keywords.EndProcedure);
        //        Expect(TokenKind.Semicolon);

        //        return result;
        //    }

        //    return null;
        //}





        //private IntegerDataType ParseIntegerDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Integer))
        //    {
        //        return new IntegerDataType();
        //    }

        //    return null;
        //}

        //private NumberDataType ParseNumberDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Number))
        //    {
        //        return new NumberDataType();
        //    }

        //    return null;
        //}

        //private LogicalDataType ParseLogicalDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Logical))
        //    {
        //        return new LogicalDataType();
        //    }

        //    return null;
        //}

        //private BooleanDataType ParseBooleanDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Boolean))
        //    {
        //        return new BooleanDataType();
        //    }

        //    return null;
        //}


        //private StringDataType ParseStringDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.String))
        //    {
        //        Expression width = null;
        //        bool isFixed = false;

        //        if (Accept(TokenKind.LeftParen))
        //        {
        //            width = ParseExpression(false);

        //            Expect(TokenKind.RightParen);

        //            isFixed = Accept(TokenKind.Keyword, Keywords.Fixed);
        //        }

        //        return new StringDataType(width, isFixed);
        //    }

        //    return null;
        //}


        //private SelectDataType ParseSelectDataType(bool extensible)
        //{
        //    if (extensible && Accept(TokenKind.Keyword, Keywords.GenericEntity))
        //    {
        //        Expect(TokenKind.Keyword, Keywords.Select);

        //        if (Accept(TokenKind.Keyword, Keywords.BasedOn))
        //        {
        //            var typeRef = ParseTypeReference(false);

        //            if (Accept(TokenKind.Keyword, Keywords.With))
        //            {
        //                var items = ParseSelectItems();

        //                return new SelectDataType(true, items, typeRef, true);
        //            }
        //            else
        //            {
        //                return new SelectDataType(true, ImmutableArray<ReferenceDataType>.Empty, typeRef, true);
        //            }
        //        }
        //        else
        //        {
        //            var items = ParseSelectItems();

        //            return new SelectDataType(true, items, null, true);
        //        }
        //    }
        //    else if (Accept(TokenKind.Keyword, Keywords.Select))
        //    {
        //        if (Accept(TokenKind.Keyword, Keywords.BasedOn))
        //        {
        //            var typeRef = ParseTypeReference(false);

        //            if (Accept(TokenKind.Keyword, Keywords.With))
        //            {
        //                var items = ParseSelectItems();

        //                return new SelectDataType(extensible, items, typeRef, false);
        //            }
        //            else
        //            {
        //                return new SelectDataType(extensible, ImmutableArray<ReferenceDataType>.Empty, typeRef, false);
        //            }
        //        }
        //        else
        //        {
        //            var items = ParseSelectItems();

        //            return new SelectDataType(extensible, items, null, false);
        //        }
        //    }

        //    return null;
        //}

        //private ImmutableArray<ReferenceDataType> ParseSelectItems()
        //{
        //    if (Accept(TokenKind.LeftParen))
        //    {
        //        var result = ImmutableArray.CreateBuilder<ReferenceDataType>();

        //        result.Add(ParseNamedDataType(false));

        //        while (Accept(TokenKind.Comma))
        //            result.Add(ParseNamedDataType(false));

        //        Expect(TokenKind.RightParen);

        //        return result.ToImmutable();
        //    }

        //    return ImmutableArray<ReferenceDataType>.Empty;
        //}

        //private ReferenceDataType ParseNamedDataType(bool optional)
        //{
        //    var result = ParseTypeReference(true)
        //              ?? ParseEntityReference(true) as Reference;

        //    if (result != null)
        //        return new ReferenceDataType(result);

        //    return optional ? null : new ReferenceDataType(Reference.Invalid);
        //}



        //private ImmutableArray<EnumerationDeclaration> ParseEnumerationItems()
        //{
        //    if (Accept(TokenKind.LeftParen))
        //    {
        //        var result = ImmutableArray.CreateBuilder<EnumerationDeclaration>();

        //        Expect(TokenKind.SimpleId);

        //        result.Add(new EnumerationDeclaration(CurrentToken.Text));

        //        while (Accept(TokenKind.Comma))
        //        {
        //            Expect(TokenKind.SimpleId);

        //            result.Add(new EnumerationDeclaration(CurrentToken.Text));
        //        }

        //        Expect(TokenKind.RightParen);

        //        return result.ToImmutable();
        //    }

        //    //TODO: error for items
        //    return ImmutableArray<EnumerationDeclaration>.Empty;
        //}



        //private ReferenceDataType ParseEntityDeclarationDataType()
        //{
        //    if (ParseEntityReference(true) is EntityReference entityReference)
        //        return new ReferenceDataType(entityReference);

        //    return null;
        //}

        //private DataType ParseConcreteDataType()
        //{
        //    return ParseAggregationDataType()
        //        ?? ParseSimpleDataType()
        //        ?? ParseTypeReferenceDataType() as DataType;
        //}



        //private SetDataType ParseSetDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Set))
        //    {
        //        var bounds = ParseBounds(true);

        //        Expect(TokenKind.Keyword, Keywords.Of);

        //        var elementType = ParseInstantiableType(false);

        //        if (elementType == null)
        //            EmitError(Errors.ParserMissingElementType);

        //        return new SetDataType(elementType, bounds);
        //    }

        //    return null;
        //}

        //private Bounds ParseBounds(bool optional)
        //{
        //   
        //    return optional ? null : Bounds.Invalid;
        //}

        private StatementBuilder TryParseStatement(Scope scope)
        {
            return TryParseAliasStatement(scope) ??
                   TryParseCaseStatement(scope) ??
                   TryParseCompoundStatement(scope) ??
                   TryParseEscapeStatement(scope) ??
                   TryParseIfStatement(scope) ??
                   TryParseProcedureCallStatement(scope) ??
                   TryParseRepeatStatement(scope) ??
                   TryParseReturnStatement(scope) ??
                   TryParseSkipStatement(scope) ??
                   TryParseNullStatement(scope) ??
                   TryParseAssignmentStatement(scope);

        }

        private StatementBuilder TryParseCaseStatement(Scope scope)
        {
            if (Accept(TokenKind.Keyword, Keywords.Case))
            {
                var builder = new CaseStatementBuilder();

                Recover(() =>
                {
                    builder.Selector = ParseExpression(scope);

                    Expect(TokenKind.Keyword, Keywords.Of);

                    while (Enumerator.TryPeek(out var token) && token.Kind != TokenKind.Keyword &&
                           !token.Text.Equals(Keywords.Otherwise, StringComparison.OrdinalIgnoreCase) &&
                           !token.Text.Equals(Keywords.EndCase, StringComparison.OrdinalIgnoreCase))
                    {
                        builder.Actions.Add(ParseCaseAction(scope));
                    }

                    if (Accept(TokenKind.Keyword, Keywords.Otherwise))
                    {
                        Expect(TokenKind.Colon);
                        builder.DefaultAction = ParseStatement(scope);
                    }

                }, TokenKind.Keyword, Keywords.EndCase);

                Expect(TokenKind.Keyword, Keywords.EndCase);
                Expect(TokenKind.Semicolon);

                return builder;
            }

            return null;
        }

        private CaseActionBuilder ParseCaseAction(Scope scope)
        {
            var action = new CaseActionBuilder();

            action.Labels.Add(ParseExpression(scope));

            while (Accept(TokenKind.Comma))
            {
                action.Labels.Add(ParseExpression(scope));
            }

            Expect(TokenKind.Colon);

            action.Statement = ParseStatement(scope);

            return action;
        }

        private StatementBuilder TryParseCompoundStatement(Scope scope)
        {
            if (Accept(TokenKind.Keyword, Keywords.Begin))
            {
                var builder = new CompoundStatementBuilder();

                Recover(() =>
                {
                    while (TryParseStatement(scope) is StatementBuilder statement)
                    {
                        builder.Statements.Add(statement);
                    }

                }, TokenKind.Keyword, Keywords.End);

                Expect(TokenKind.Keyword, Keywords.End);
                Expect(TokenKind.Semicolon);

                return builder;
            }

            return null;
        }

        private StatementBuilder TryParseEscapeStatement(Scope scope)
        {
            if (Accept(TokenKind.Keyword, Keywords.Escape))
            {
                Expect(TokenKind.Semicolon);

                return new EscapeStatementBuilder();
            }

            return null;
        }

        private StatementBuilder TryParseIfStatement(Scope scope)
        {
            if (Accept(TokenKind.Keyword, Keywords.If))
            {
                var builder = new IfStatementBuilder();


                Recover(() =>
                {
                    builder.Condition = ParseExpression(scope);

                    Expect(TokenKind.Keyword, Keywords.Then);

                    builder.Statements.Add(ParseStatement(scope));

                    while (TryParseStatement(scope) is StatementBuilder statement)
                    {
                        builder.Statements.Add(statement);
                    }

                    if (Accept(TokenKind.Keyword, Keywords.Else))
                    {
                        builder.ElseStatements.Add(ParseStatement(scope));

                        while (TryParseStatement(scope) is StatementBuilder statement)
                        {
                            builder.ElseStatements.Add(statement);
                        }
                    }

                }, TokenKind.Keyword, Keywords.EndIf);

                Expect(TokenKind.Keyword, Keywords.EndIf);
                Expect(TokenKind.Semicolon);

                return builder;
            }

            return null;
        }

        private StatementBuilder ParseStatement(Scope scope)
        {
            if (TryParseStatement(scope) is StatementBuilder statement)
            {
                return statement;
            }

            //TODO: error on missing statement
            throw new NotImplementedException("error on missing statement");
        }

        private StatementBuilder TryParseProcedureCallStatement(Scope scope)
        {
            if (TryParseProcedureReference(scope) is ProcedureReferenceBuilder procedure)
            {
                var result = new ProcedureCallStatementBuilder
                {
                    Procedure = procedure
                };

                if (Accept(TokenKind.LeftParen))
                {
                    result.Parameters.Add(ParseExpression(scope));

                    while (Accept(TokenKind.Comma))
                    {
                        result.Parameters.Add(ParseExpression(scope));
                    }

                    Expect(TokenKind.RightParen);
                }

                Expect(TokenKind.Semicolon);

                return result;
            }

            return null;
        }

        private StatementBuilder TryParseRepeatStatement(Scope scope)
        {
            if (Accept(TokenKind.Keyword, Keywords.Repeat))
            {
                var builder = new RepeatStatementBuilder();

                Recover(() =>
                {
                    if (Accept(TokenKind.SimpleId))
                    {
                        builder.IncrementVariable = new VariableDeclarationBuilder
                        {
                            Name = CurrentToken.Text,
                        };

                        Expect(TokenKind.Assignment);

                        builder.IncrementFrom = ParseExpression(scope);

                        Expect(TokenKind.Keyword, Keywords.To);

                        builder.IncrementTo = ParseExpression(scope);

                        if (Accept(TokenKind.Keyword, Keywords.By))
                        {
                            builder.IncrementStep = ParseExpression(scope);
                        }
                    }

                    if (Accept(TokenKind.Keyword, Keywords.While))
                    {
                        builder.WhileCondition = ParseExpression(scope);
                    }

                    if (Accept(TokenKind.Keyword, Keywords.Until))
                    {
                        builder.UntilCondition = ParseExpression(scope);
                    }

                    Expect(TokenKind.Semicolon);

                    var repeatScope = scope.CreateChildScope(builder);

                    builder.Statements.Add(ParseStatement(repeatScope));

                    while (TryParseStatement(repeatScope) is StatementBuilder statement)
                    {
                        builder.Statements.Add(statement);
                    }

                }, TokenKind.Keyword, Keywords.EndRepeat);

                Expect(TokenKind.Keyword, Keywords.EndRepeat);
                Expect(TokenKind.Semicolon);

                return builder;
            }

            return null;
        }

        private StatementBuilder TryParseReturnStatement(Scope scope)
        {
            if (Accept(TokenKind.Keyword, Keywords.Return))
            {
                var result = new ReturnStatementBuilder
                {
                    Expression = ParseExpression(scope)
                };

                Expect(TokenKind.Semicolon);

                return result;
            }

            return null;
        }

        private StatementBuilder TryParseSkipStatement(Scope scope)
        {
            if (Accept(TokenKind.Keyword, Keywords.Skip))
            {
                Expect(TokenKind.Semicolon);

                return new SkipStatementBuilder();
            }

            return null;
        }

        private StatementBuilder TryParseNullStatement(Scope scope)
        {
            if (Accept(TokenKind.Semicolon))
            {
                return new NullStatementBuilder();
            }

            return null;
        }

        private StatementBuilder TryParseAssignmentStatement(Scope scope)
        {
            if (TryParseGeneralReference(scope) is ReferenceBuilder reference)
            {
                var qualifiedReference = new QualifiedReferenceBuilder
                {
                    Reference = reference
                };

                while (TryParseQualifier(scope) is QualifierBuilder qualifier)
                {
                    qualifiedReference.Qualifiers.Add(qualifier);
                }

                Expect(TokenKind.Assignment);

                var result = new AssignmentStatementBuilder
                {
                    Left = qualifiedReference,
                    Right = ParseExpression(scope)
                };

                Expect(TokenKind.Semicolon);

                return result;
            }

            return null;
        }

        private ReferenceBuilder TryParseGeneralReference(Scope scope)
        {
            return TryParseVariableReference(scope) ??
                   TryParseParameterReference(scope) as ReferenceBuilder;
        }

        private StatementBuilder TryParseAliasStatement(Scope scope)
        {
            if (Accept(TokenKind.Keyword, Keywords.Alias))
            {
                var builder = new AliasStatementBuilder();

                Recover(() =>
                {
                    throw new NotImplementedException(nameof(TryParseAliasStatement));

                }, TokenKind.Keyword, Keywords.EndAlias);

                Expect(TokenKind.Keyword, Keywords.EndAlias);
                Expect(TokenKind.Semicolon);

                return builder;
            }

            return null;
        }

        private ExpressionBuilder ParseExpression(Scope scope)
        {
            var expression = ParseSimpleExpression(scope);

            while (AcceptRelationalOperatorExtended())
            {
                switch (CurrentToken.Kind)
                {
                    case TokenKind.LessThan:
                        expression = new LessThanExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseSimpleExpression(scope)
                        };
                        break;
                    case TokenKind.GreaterThan:
                        expression = new GreaterThanExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseSimpleExpression(scope)
                        };
                        break;
                    case TokenKind.LessThanOrEqual:
                        expression = new LessThanOrEqualExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseSimpleExpression(scope)
                        };
                        break;
                    case TokenKind.GreaterThanOrEqual:
                        expression = new GreaterThanOrEqualExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseSimpleExpression(scope)
                        };
                        break;
                    case TokenKind.NotEqual:
                        expression = new NotEqualExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseSimpleExpression(scope)
                        };
                        break;
                    case TokenKind.Equal:
                        expression = new EqualExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseSimpleExpression(scope)
                        };
                        break;
                    case TokenKind.InstanceNotEqual:
                        expression = new InstanceNotEqualExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseSimpleExpression(scope)
                        };
                        break;
                    case TokenKind.InstanceEqual:
                        expression = new InstanceEqualExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseSimpleExpression(scope)
                        };
                        break;
                    case TokenKind.Keyword when CurrentToken.Text.Equals(Keywords.In, StringComparison.OrdinalIgnoreCase):
                        expression = new InExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseSimpleExpression(scope)
                        };
                        break;
                    case TokenKind.Keyword when CurrentToken.Text.Equals(Keywords.Like, StringComparison.OrdinalIgnoreCase):
                        expression = new LikeExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseSimpleExpression(scope)
                        };
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            return expression;
        }


        private ExpressionBuilder ParseSimpleExpression(Scope scope)
        {
            var expression = ParseTerm(scope);

            while (AcceptAddLikeOperator())
            {
                switch (CurrentToken.Kind)
                {
                    case TokenKind.Plus:
                        expression = new AdditionExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseTerm(scope)
                        };
                        break;
                    case TokenKind.Minus:
                        expression = new SubtractionExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseTerm(scope)
                        };
                        break;
                    case TokenKind.Keyword when CurrentToken.Text.Equals(Keywords.Or, StringComparison.OrdinalIgnoreCase):
                        expression = new OrExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseTerm(scope)
                        };
                        break;
                    case TokenKind.Keyword when CurrentToken.Text.Equals(Keywords.Xor, StringComparison.OrdinalIgnoreCase):
                        expression = new XorExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseTerm(scope)
                        };
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            return expression;
        }

        private ExpressionBuilder ParseTerm(Scope scope)
        {
            var expression = ParseFactor(scope);

            while (AcceptMultiplicationLikeOperator())
            {
                switch (CurrentToken.Kind)
                {
                    case TokenKind.Multiply:
                        expression = new MultiplicationExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseFactor(scope)
                        };
                        break;
                    case TokenKind.Slash:
                        expression = new DivisionExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseFactor(scope)
                        };
                        break;
                    case TokenKind.ComplexEntityConstruction:
                        expression = new ComplexEntityConstructionExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseFactor(scope)
                        };
                        break;
                    case TokenKind.Keyword when CurrentToken.Text.Equals(Keywords.Div, StringComparison.OrdinalIgnoreCase):
                        expression = new IntegerDivisionExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseFactor(scope)
                        };
                        break;
                    case TokenKind.Keyword when CurrentToken.Text.Equals(Keywords.Mod, StringComparison.OrdinalIgnoreCase):
                        expression = new ModuloExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseFactor(scope)
                        };
                        break;
                    case TokenKind.Keyword when CurrentToken.Text.Equals(Keywords.And, StringComparison.OrdinalIgnoreCase):
                        expression = new AndExpressionBuilder
                        {
                            Left = expression,
                            Right = ParseFactor(scope)
                        };
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            return expression;
        }

        private ExpressionBuilder ParseFactor(Scope scope)
        {
            var expression = ParseSimpleFactor(scope);

            while (Accept(TokenKind.Exponent))
            {
                expression = new ExponentiationExpressionBuilder
                {
                    Left = expression,
                    Right = ParseFactor(scope)
                };
            }

            return expression;
        }

        private ExpressionBuilder ParseNumericExpression(Scope scope)
        {
            return ParseSimpleExpression(scope);
        }


        private ExpressionBuilder ParseSimpleFactor(Scope scope)
        {
            var result = TryParseAggregateInitializer(scope)
                ?? TryParseInterval(scope)
                ?? TryParseQueryExpression(scope)
                ?? TryParseEntityConstructor(scope)
                ?? TryParseEnumerationReferenceExpression(scope);

            if (result != null)
                return result;

            if (AcceptUnaryOpeartor())
            {
                switch (CurrentToken.Kind)
                {
                    case TokenKind.Plus:
                        return new UnaryPlusExpressionBuilder
                        {
                            Operand = ParsePrimary(scope)
                        };
                    case TokenKind.Minus:
                        return new UnaryMinusExpressionBuilder
                        {
                            Operand = ParsePrimary(scope)
                        };
                    case TokenKind.Keyword when CurrentToken.Text.Equals(Keywords.Not, StringComparison.OrdinalIgnoreCase):
                        return new NotExpressionBuilder
                        {
                            Operand = ParsePrimary(scope)
                        };

                    default:
                        throw new NotSupportedException();
                }
            }
            else return ParsePrimary(scope);
        }

        private ExpressionBuilder ParsePrimary(Scope scope)
        {
            if (Accept(TokenKind.LeftParen))
            {
                var expression = ParseExpression(scope);

                Expect(TokenKind.RightParen);

                return expression;
            }
            else
            {
                return TryParseLiteralExpression(scope)
                    ?? ParseQualifiedExpression(scope);
            }
        }

        private ExpressionBuilder TryParseEnumerationReferenceExpression(Scope scope)
        {
            var type = TryParseTypeReference(scope);

            if (type != null)
            {
                Expect(TokenKind.Period);

                return new EnumerationReferenceExpressionBuilder
                {
                    Type = type,
                    Enumeration = ParseEnumerationReference(scope)
                };
            }
            else
            {
                var enumeration = TryParseEnumerationReference(scope);

                if (enumeration != null)
                {
                    return new EnumerationReferenceExpressionBuilder
                    {
                        Enumeration = enumeration
                    };
                }

                return null;
            }
        }

        private EnumerationReferenceBuilder TryParseEnumerationReference(Scope scope)
        {
            if (Enumerator.TryPeek(out var token) &&
                token.Kind == TokenKind.SimpleId &&
                scope.ResolveEnumeration(token.Text) is EnumerationDeclarationBuilder enumeration)
            {
                Enumerator.MoveNext();

                return new EnumerationReferenceBuilder
                {
                    EnumerationName = enumeration.Name
                };
            }

            return null;
        }

        private EnumerationReferenceBuilder ParseEnumerationReference(Scope scope)
        {
            Expect(TokenKind.SimpleId);

            return new EnumerationReferenceBuilder
            {
                EnumerationName = CurrentToken.Text
            };
        }

        private ExpressionBuilder TryParseEntityConstructor(Scope scope)
        {
            //TODO: hack it collides with reference to population when declared in rule,
            //      for now we check here
                    
            if(Enumerator.TryPeek(out var token) && token.Kind == TokenKind.SimpleId &&
                scope.ResolveVariable(token.Text) != null)
            {
                return null;
            }

            if (TryParseEntityReference(scope) is EntityReferenceBuilder entity)
            {
                var result = new EntityConstructorExpressionBuilder
                {
                    Entity = entity
                };

                if (Accept(TokenKind.LeftParen))
                {
                    if (!Accept(TokenKind.RightParen))
                    {
                        do
                        {
                            result.Paramaters.Add(ParseExpression(scope));

                        } while (Accept(TokenKind.Comma));

                        Expect(TokenKind.RightParen);
                    }
                }

                return result;
            }

            return null;
        }

        private TypeReferenceBuilder TryParseTypeReference(Scope scope)
        {
            if (Enumerator.TryPeek(out var token) &&
                  token.Kind == TokenKind.SimpleId &&
                  scope.ResolveType(token.Text) is TypeDeclarationBuilder type)
            {
                Enumerator.MoveNext();
                return new TypeReferenceBuilder { TypeName = type.Name };
            }

            return null;
        }

        private EntityReferenceBuilder TryParseEntityReference(Scope scope)
        {
            if (Enumerator.TryPeek(out var token) &&
               token.Kind == TokenKind.SimpleId &&
               scope.ResolveEntity(token.Text) is EntityDeclarationBuilder entity)
            {
                Enumerator.MoveNext();
                return new EntityReferenceBuilder { EntityName = entity.Name };
            }

            return null;
        }

        private ExpressionBuilder TryParseQueryExpression(Scope scope)
        {
            if (Accept(TokenKind.Keyword, Keywords.Query))
            {

                var query = new QueryExpressionBuilder();

                Expect(TokenKind.LeftParen);

                Expect(TokenKind.SimpleId);

                query.VariableDeclaration = new VariableDeclarationBuilder
                {
                    Name = CurrentToken.Text
                };

                Expect(TokenKind.Query);

                var aggregate = ParseExpression(scope);

                Expect(TokenKind.Pipe);

                var queryScope = scope.CreateChildScope(query);

                var condition = ParseExpression(queryScope);

                Expect(TokenKind.RightParen);

                return query;
            }

            return null;
        }

        private ExpressionBuilder TryParseInterval(Scope scope)
        {
            IntervalComparison parseComparison()
            {
                if (Accept(TokenKind.LessThan))
                    return IntervalComparison.Less;

                Expect(TokenKind.LessThanOrEqual);
                return IntervalComparison.LessOrEqual;
            }

            if (Accept(TokenKind.LeftBrace))
            {
                var result = new IntervalExpressionBuilder
                {
                    Low = ParseSimpleExpression(scope),
                    LowComparison = parseComparison(),
                    Item = ParseSimpleExpression(scope),
                    HighComparison = parseComparison(),
                    High = ParseSimpleExpression(scope)
                };

                Expect(TokenKind.RightBrace);

                return result;
            }

            return null;
        }

        private ExpressionBuilder TryParseAggregateInitializer(Scope scope)
        {
            if (Accept(TokenKind.LeftBracket))
            {
                var result = new AggregateInitializerExpressionBuilder();

                if (!Accept(TokenKind.RightBracket))
                {
                    do
                    {
                        result.Elements.Add(new AggregateInitializerElementBuilder
                        {
                            Expression = ParseExpression(scope),
                            Repetition = Accept(TokenKind.Colon) ? ParseExpression(scope) : null
                        });

                    } while (Accept(TokenKind.Comma));

                    Expect(TokenKind.RightBracket);
                }

                return result;
            }

            return null;
        }


        private LiteralExpressionBuilder TryParseLiteralExpression(Scope scope)
        {
            if (Accept(TokenKind.BinaryLiteral))
                throw new NotImplementedException();
            else if (Accept(TokenKind.IntegerLiteral))
                return new IntegerLiteralBuilder
                {
                    Value = int.Parse(CurrentToken.Text, NumberStyles.None)
                };
            else if (Accept(TokenKind.RealLiteral))
                return new RealLiteralBuilder
                {
                    Value = double.Parse(CurrentToken.Text,
                        NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                        CultureInfo.InvariantCulture)
                };
            else if (Accept(TokenKind.SimpleStringLiteral) || Accept(TokenKind.EncodedStringLiteral))
                return new StringLiteralBuilder
                {
                    Value = CurrentToken.Text
                };

            return null;
        }

        private QualifierBuilder TryParseAttributeQualifier(Scope scope)
        {
            if (Accept(TokenKind.Period))
            {
                return new AttributeQualifierBuilder
                {
                    Attribute = ParseAttributeReference(scope)
                };
            }

            return null;
        }

        private QualifierBuilder TryParseGroupQualifier(Scope scope)
        {
            if (Accept(TokenKind.Backslash))
            {
                return new GroupQualifierBuilder
                {
                    Entity = ParseEntityReference(scope)
                };
            }

            return null;
        }

        private QualifierBuilder TryParseIndexQualifier(Scope scope)
        {
            if (Accept(TokenKind.LeftBracket))
            {
                var result = new IndexQualifierBuilder
                {
                    From = ParseExpression(scope),
                    To = Accept(TokenKind.Colon) ? ParseExpression(scope) : null
                };

                Expect(TokenKind.RightBracket);

                return result;
            }

            return null;
        }

        private QualifierBuilder TryParseQualifier(Scope scope)
        {
            return TryParseAttributeQualifier(scope)
                ?? TryParseGroupQualifier(scope)
                ?? TryParseIndexQualifier(scope);
        }

        private ExpressionBuilder ParseQualifiedExpression(Scope scope)
        {
            var expression = ParseQualifiableFactor(scope);

            if (TryParseQualifier(scope) is QualifierBuilder qualifier)
            {
                var result = new QualifiedExpressionBuilder
                {
                    Expression = expression,
                    Qualifiers = { qualifier }
                };

                while (TryParseQualifier(scope) is QualifierBuilder next)
                {
                    result.Qualifiers.Add(next);
                }

                return result;
            }

            return expression;
        }

        private EntityReferenceBuilder ParseEntityReference(Scope scope)
        {
            Expect(TokenKind.SimpleId);

            return new EntityReferenceBuilder
            {
                EntityName = CurrentToken.Text
            };
        }

        private AttributeReferenceBuilder ParseAttributeReference(Scope scope)
        {
            Expect(TokenKind.SimpleId);

            return new AttributeReferenceBuilder
            {
                AttributeName = CurrentToken.Text
            };
        }

        private ExpressionBuilder ParseQualifiableFactor(Scope scope)
        {
            var result = TryParseAttributeReferenceExpression(scope)
                ?? TryParseConstantFactor(scope)
                ?? TryParseFunctionCallExpression(scope)
                ?? TryParseVariableReferenceExpression(scope)
                ?? TryParseParameterReferenceExpression(scope)
                ?? TryParseEntityReferenceExpression(scope);


            if (result == null)
            {
                if (Enumerator.TryPeek(out var token))
                {
                    throw new NotImplementedException("Cannot find factor for " + token.Text.ToString());
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return result;
        }

        private ExpressionBuilder TryParseEntityReferenceExpression(Scope scope)
        {
            var entityRef = TryParseEntityReference(scope);

            if (entityRef != null)
            {
                return new EntityReferenceExpressionBuilder
                {
                    Entity = entityRef
                };
            }

            return null;
        }

        private ExpressionBuilder TryParseParameterReferenceExpression(Scope scope)
        {
            var parameterRef = TryParseParameterReference(scope);

            if (parameterRef != null)
            {
                return new ParameterReferenceExpressionBuilder
                {
                    Parameter = parameterRef
                };
            }

            return null;
        }

        private ParameterReferenceBuilder TryParseParameterReference(Scope scope)
        {
            if (Enumerator.TryPeek(out var token) &&
                token.Kind == TokenKind.SimpleId &&
                scope.ResolveParameter(token.Text) is ParameterDeclarationBuilder parameter)
            {
                Enumerator.MoveNext();
                return new ParameterReferenceBuilder
                {
                    ParameterName = parameter.Name
                };
            }

            return null;
        }

        private ExpressionBuilder TryParseVariableReferenceExpression(Scope scope)
        {
            var variableRef = TryParseVariableReference(scope);

            if (variableRef != null)
            {
                return new VariableReferenceExpressionBuilder
                {
                    Variable = variableRef
                };
            }

            return null;
        }

        private VariableReferenceBuilder TryParseVariableReference(Scope scope)
        {
            if (Enumerator.TryPeek(out var token) &&
                token.Kind == TokenKind.SimpleId &&
                scope.ResolveVariable(token.Text) is VariableDeclarationBuilder variable)
            {
                Enumerator.MoveNext();
                return new VariableReferenceBuilder
                {
                    VariableName = variable.Name
                };
            }

            return null;
        }

        private ExpressionBuilder TryParseAttributeReferenceExpression(Scope scope)
        {
            var attributeRef = TryParseAttributeReference(scope);

            if (attributeRef != null)
            {
                return new AttributeReferenceExpressionBuilder
                {
                    Attribute = attributeRef
                };
            }

            return null;
        }

        private AttributeReferenceBuilder TryParseAttributeReference(Scope scope)
        {
            if (Enumerator.TryPeek(out var token) &&
                token.Kind == TokenKind.SimpleId &&
                scope.ResolveAttribute(token.Text) is AttributeDeclarationBuilder attribute)
            {
                Enumerator.MoveNext();
                return new AttributeReferenceBuilder
                {
                    AttributeName = attribute.Name
                };
            }

            return null;
        }

        //private Expression ParseGeneralReferenceExpression()
        //{
        //    return ParseVariableReferenceExpression()
        //        ?? ParseParameterReferenceExpression() as Expression;
        //}

        private ExpressionBuilder TryParseFunctionCallExpression(Scope scope)
        {
            var functionRef = TryParseBuiltInFunction() ?? TryParseFunctionReference(scope);

            if (functionRef == null)
            {
                return null;
            }

            var result = new FunctionCallExpressionBuilder
            {
                Function = functionRef
            };

            if (Accept(TokenKind.LeftParen))
            {
                result.Parameters.Add(ParseExpression(scope));

                while (Accept(TokenKind.Comma))
                {
                    result.Parameters.Add(ParseExpression(scope));
                }

                Expect(TokenKind.RightParen);
            }

            return result;
        }

        private FunctionReferenceBuilder TryParseFunctionReference(Scope scope)
        {
            if (Enumerator.TryPeek(out var token) &&
                token.Kind == TokenKind.SimpleId &&
                scope.ResolveFunction(token.Text) is FunctionDeclarationBuilder function)
            {
                Enumerator.MoveNext();
                return new FunctionReferenceBuilder
                {
                    FunctionName = function.Name
                };
            }

            return null;
        }

        private ProcedureReferenceBuilder TryParseProcedureReference(Scope scope)
        {
            if (Enumerator.TryPeek(out var token) &&
                token.Kind == TokenKind.SimpleId &&
                scope.ResolveProcedure(token.Text) is ProcedureDeclarationBuilder procedure)
            {
                Enumerator.MoveNext();
                return new ProcedureReferenceBuilder
                {
                    ProcedureName = procedure.Name
                };
            }

            return null;
        }

        private ExpressionBuilder TryParseConstantFactor(Scope scope)
        {
            var builtInConstant = TryParseBuiltInConstant();

            if (builtInConstant != null)
                return new ConstantReferenceExpressionBuilder
                {
                    Constant = builtInConstant
                };

            return TryParseConstantReferenceExpression(scope);
        }

        private ExpressionBuilder TryParseConstantReferenceExpression(Scope scope)
        {
            var constantRef = TryParseConstantReference(scope);

            if (constantRef != null)
            {
                return new ConstantReferenceExpressionBuilder
                {
                    Constant = constantRef
                };
            }

            return null;
        }

        private ConstantReferenceBuilder TryParseConstantReference(Scope scope)
        {
            if (Enumerator.TryPeek(out var token)
                && token.Kind == TokenKind.SimpleId
                && scope.ResolveConstant(token.Text) is ConstantDeclarationBuilder constant)
            {
                Enumerator.MoveNext();
                return new ConstantReferenceBuilder
                {
                    ConstantName = constant.Name
                };
            }

            return null;
        }

        private FunctionReferenceBuilder TryParseBuiltInFunction()
        {
            if (Enumerator.TryPeek(out var token) && token.Kind == TokenKind.Keyword
                && Keywords.IsBuiltInFunction(token.Text))
            {
                Enumerator.MoveNext();
                return new FunctionReferenceBuilder
                {
                    FunctionName = CurrentToken.Text
                };
            }

            return null;
        }

        private ConstantReferenceBuilder TryParseBuiltInConstant()
        {
            if (Accept(TokenKind.Keyword, Keywords.ConstE)
             || Accept(TokenKind.Keyword, Keywords.Pi)
             || Accept(TokenKind.Keyword, Keywords.Self)
             || Accept(TokenKind.Keyword, Keywords.True)
             || Accept(TokenKind.Keyword, Keywords.False)
             || Accept(TokenKind.Keyword, Keywords.Unknown)
             || Accept(TokenKind.QuestionMark))
                return new ConstantReferenceBuilder { ConstantName = CurrentToken.Text };

            return null;
        }

        //private ConstantReferenceExpression ParseConstantReferenceExpression()
        //{
        //    var constantRef = ParseConstantReference();

        //    if (constantRef != null)
        //        return new ConstantReferenceExpression(constantRef);

        //    return null;
        //}


        //private ParameterReferenceExpression ParseParameterReferenceExpression()
        //{
        //    var parameterRef = ParseParameterReference();

        //    if (parameterRef != null)
        //        return new ParameterReferenceExpression(parameterRef);

        //    return null;
        //}

        //private VariableReferenceExpression ParseVariableReferenceExpression()
        //{
        //    var variableRef = ParseVariableReference();

        //    if (variableRef != null)
        //        return new VariableReferenceExpression(variableRef);

        //    return null;
        //}

        //private EntityReferenceExpression ParseEntityReferenceExpression()
        //{
        //    if (ParseEntityReference(true) is EntityReference entityRef)
        //        return new EntityReferenceExpression(entityRef);

        //    return null;
        //}

        //private AttributeReferenceExpression ParseAttributeReferenceExpression()
        //{
        //    var attributeRef = ParseAttributeReference(true);

        //    if (attributeRef != null)
        //        return new AttributeReferenceExpression(null, attributeRef);

        //    return null;
        //}


        //private ReferenceDataType ParseTypeReferenceDataType()
        //{
        //    if (ParseTypeReference(true) is TypeReference typeReference)
        //        return new ReferenceDataType(typeReference);

        //    return null;
        //}


        //private LocalRuleDeclaration ParseLocalRuleDeclaration(bool optional)
        //{
        //    SimpleId? name = null;

        //    if (AcceptIdentifier(IdentifierKind.RuleLabel))
        //    {
        //        name = CurrentToken.Text;

        //        Expect(TokenKind.Colon);
        //    }

        //    var expression = ParseExpression(name == null && optional);

        //    if (expression != null)
        //    {
        //        Expect(TokenKind.Semicolon);

        //        return new LocalRuleDeclaration(name, expression);
        //    }
        //    else
        //        return null;
        //}

        //private FunctionDeclaration ParseFunctionDeclaration()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Function))
        //    {
        //        var result = new FunctionDeclaration();

        //        Recover(() =>
        //        {
        //            throw new NotImplementedException(nameof(FunctionDeclaration));

        //        }, TokenKind.Keyword, Keywords.EndFunction);

        //        Expect(TokenKind.Keyword, Keywords.EndFunction);
        //        Expect(TokenKind.Semicolon);

        //        return result;
        //    }

        //    return null;
        //}



        //private ImmutableArray<LocalRuleDeclaration> ParseUniqueRules()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Unique))
        //    {
        //        throw new NotImplementedException(nameof(ParseUniqueRules));
        //    }

        //    return ImmutableArray<LocalRuleDeclaration>.Empty;
        //}

        //private ImmutableArray<DerivedAttribute> ParseDerivedAttributes()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Derive))
        //    {
        //        throw new NotImplementedException(nameof(ParseDerivedAttributes));
        //    }

        //    return ImmutableArray<DerivedAttribute>.Empty;
        //}

        //private ImmutableArray<InverseAttribute> ParseInverseAttributes()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Inverse))
        //    {
        //        var result = ImmutableArray.CreateBuilder<InverseAttribute>();

        //        while (ParseAttributeDeclaration(true) is AttributeDeclaration declaration)
        //        {
        //            Expect(TokenKind.Colon);

        //            DataType attributeType;

        //            if (Accept(TokenKind.Keyword, Keywords.Bag))
        //            {
        //                var bounds = ParseBounds(true);
        //                Expect(TokenKind.Keyword, Keywords.Of);
        //                attributeType = new BagDataType(new ReferenceDataType(ParseEntityReference(false)), bounds);
        //            }
        //            else if (Accept(TokenKind.Keyword, Keywords.Set))
        //            {
        //                var bounds = ParseBounds(true);
        //                Expect(TokenKind.Keyword, Keywords.Of);
        //                attributeType = new SetDataType(new ReferenceDataType(ParseEntityReference(false)), bounds);
        //            }
        //            else
        //            {
        //                attributeType = new ReferenceDataType(ParseEntityReference(false));
        //            }

        //            Expect(TokenKind.Keyword, Keywords.For);

        //            EntityReference forEntity = ParseEntityReference(true);

        //            if (forEntity != null)
        //            {
        //                Expect(TokenKind.Period);
        //            }

        //            var forAttribute = ParseAttributeReference(false);

        //            Expect(TokenKind.Semicolon);

        //            result.Add(new InverseAttribute(declaration, attributeType, forEntity, forAttribute));
        //        }

        //        return result.ToImmutable();
        //    }

        //    return ImmutableArray<InverseAttribute>.Empty;
        //}

        //private ImmutableArray<ExplicitAttribute> ParseExplicitAttributes()
        //{
        //    AttributeDeclaration declaration;
        //    var result = ImmutableArray.CreateBuilder<ExplicitAttribute>();

        //    while ((declaration = ParseAttributeDeclaration(true)) != null)
        //    {

        //        var declarations = ImmutableArray.CreateBuilder<AttributeDeclaration>();

        //        declarations.Add(declaration);

        //        while (Accept(TokenKind.Comma))
        //        {
        //            declarations.Add(ParseAttributeDeclaration(false));
        //        }

        //        Expect(TokenKind.Colon);

        //        bool isOptional = Accept(TokenKind.Keyword, Keywords.Optional);

        //        var parameterType = ParseParameterType(false);

        //        Expect(TokenKind.Semicolon);

        //        result.Add(new ExplicitAttribute(
        //            declarations.ToImmutable(),
        //            isOptional,
        //            parameterType));
        //    }

        //    return result.ToImmutable();
        //}

        //private DataType ParseParameterType(bool optional)
        //{
        //    var result = ParseGeneralizedDataType()
        //              ?? ParseNamedDataType(true)
        //              ?? ParseSimpleDataType() as DataType;

        //    if (result != null)
        //        return result;

        //    return optional ? null : DataType.Invalid;
        //}

        //private DataType ParseGeneralizedDataType()
        //{
        //    return ParseAggregateDataType()
        //        ?? ParseGeneralAggregationDataType()
        //        ?? ParseGenericEntityDataType()
        //        ?? ParseGenericDataType() as DataType;
        //}

        //private DataType ParseAggregateDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Aggregate))
        //    {
        //        var typeLabel = Accept(TokenKind.Colon)
        //            ? ParseTypeLabelReference(false)
        //            : null;

        //        var dataType = ParseParameterType(false);

        //        return new AggregateDataType(dataType, typeLabel);
        //    }

        //    return null;
        //}

        //private AggregationDataType ParseGeneralAggregationDataType()
        //{
        //    return ParseGeneralArrayDataType()
        //        ?? ParseGeneralListDataType()
        //        ?? ParseGeneralBagDataType()
        //        ?? ParseGeneralSetDataType() as AggregationDataType;
        //}

        //private ArrayDataType ParseGeneralArrayDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Array))
        //    {
        //        throw new NotImplementedException(nameof(ParseGeneralArrayDataType));
        //    }

        //    return null;
        //}

        //private ListDataType ParseGeneralListDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.List))
        //    {
        //        throw new NotImplementedException(nameof(ParseGeneralListDataType));
        //    }

        //    return null;
        //}

        //private BagDataType ParseGeneralBagDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Bag))
        //    {
        //        throw new NotImplementedException(nameof(ParseGeneralBagDataType));
        //    }

        //    return null;
        //}

        //private SetDataType ParseGeneralSetDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Set))
        //    {
        //        throw new NotImplementedException(nameof(ParseGeneralSetDataType));
        //    }

        //    return null;
        //}

        //private DataType ParseGenericEntityDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.GenericEntity))
        //    {
        //        throw new NotImplementedException(nameof(ParseGenericEntityDataType));
        //    }

        //    return null;
        //}

        //private DataType ParseGenericDataType()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Generic))
        //    {
        //        throw new NotImplementedException(nameof(ParseGenericDataType));
        //    }

        //    return null;
        //}

        //private AttributeDeclaration ParseAttributeDeclaration(bool optional)
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Self))
        //    {
        //        Expect(TokenKind.Backslash);

        //        var redeclaredEntityRef = ParseEntityReference(false);

        //        Expect(TokenKind.Period);

        //        var redeclaredAttributeRef = ParseAttributeReference(false);

        //        Expect(TokenKind.Keyword, Keywords.Renamed);

        //        Expect(TokenKind.SimpleId);

        //        var attributeName = CurrentToken.Text;

        //        return new AttributeDeclaration(
        //            attributeName,
        //            new AttributeReferenceExpression(
        //                new GroupReferenceExpression(
        //                    new ConstantReferenceExpression(ConstantReference.Self),
        //                    redeclaredEntityRef),
        //                redeclaredAttributeRef));
        //    }

        //    if (Accept(TokenKind.SimpleId))
        //    {
        //        var attributeName = CurrentToken.Text;

        //        return new AttributeDeclaration(attributeName, null);
        //    }
        //    else if (!optional)
        //    {
        //        Expect(TokenKind.SimpleId);
        //    }

        //    return null;
        //}

        //private SupertypeExpression ParseSupertypeConstraint(out bool isAbstract, out bool isAbstractSupertype)
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Abstract))
        //    {
        //        isAbstract = true;

        //        if (Accept(TokenKind.Keyword, Keywords.Supertype))
        //        {
        //            isAbstractSupertype = true;

        //            if (Accept(TokenKind.Keyword, Keywords.Of))
        //            {
        //                Expect(TokenKind.LeftParen);

        //                var result = ParseSupertypeExpression(false);

        //                Expect(TokenKind.RightParen);

        //                return result;
        //            }
        //        }
        //        else
        //        {
        //            isAbstractSupertype = false;
        //        }

        //        return null;
        //    }
        //    else
        //    {
        //        isAbstract = isAbstractSupertype = false;

        //        if (Accept(TokenKind.Keyword, Keywords.Supertype))
        //        {
        //            Expect(TokenKind.Keyword, Keywords.Of);

        //            Expect(TokenKind.LeftParen);

        //            var result = ParseSupertypeExpression(false);

        //            Expect(TokenKind.RightParen);

        //            return result;
        //        }
        //        else
        //            return null;
        //    }
        //}

        //private SupertypeExpression ParseSupertypeExpression(bool optional)
        //{
        //    var expression = ParseSupertypeFactor(true);

        //    if (expression == null)
        //        return optional ? null : SupertypeExpression.Invalid;

        //    while (Accept(TokenKind.Keyword, Keywords.AndOr))
        //    {
        //        expression = new AndOrSupertypeExpression(expression, ParseSupertypeFactor(false));
        //    }

        //    return expression;
        //}

        //private SupertypeExpression ParseSupertypeFactor(bool optional)
        //{
        //    var expression = ParseSupertypeTerm(true);

        //    if (expression == null)
        //    {
        //        return optional ? null : SupertypeExpression.Invalid;
        //    }

        //    while (Accept(TokenKind.Keyword, Keywords.And))
        //    {
        //        expression = new AndSupertypeExpression(expression, ParseSupertypeTerm(false));
        //    }

        //    return expression;
        //}

        //private SupertypeExpression ParseSupertypeTerm(bool optional)
        //{
        //    var expression = ParseEntityReferenceSupertypeExpression()
        //                  ?? ParseOneOfSupertypeExpression() as SupertypeExpression;

        //    if (expression != null)
        //        return expression;

        //    if (Accept(TokenKind.LeftParen))
        //    {
        //        expression = ParseSupertypeExpression(false);

        //        Expect(TokenKind.RightParen);

        //        return expression;
        //    }

        //    return optional ? null : SupertypeExpression.Invalid;
        //}

        //private OneOfSupertypeExpression ParseOneOfSupertypeExpression()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Oneof))
        //    {
        //        var result = ImmutableArray.CreateBuilder<SupertypeExpression>();

        //        Expect(TokenKind.LeftParen);

        //        result.Add(ParseSupertypeExpression(false));

        //        while (Accept(TokenKind.Comma))
        //        {
        //            result.Add(ParseSupertypeExpression(false));
        //        }

        //        Expect(TokenKind.RightParen);

        //        return new OneOfSupertypeExpression(result.ToImmutable());
        //    }

        //    return null;
        //}

        //private EntityReferenceSupertypeExpression ParseEntityReferenceSupertypeExpression()
        //{
        //    var entityRef = ParseEntityReference(true);

        //    if (entityRef != null)
        //    {
        //        return new EntityReferenceSupertypeExpression(entityRef);
        //    }

        //    return null;
        //}


        //private RuleDeclaration ParseRuleDeclaration()
        //{
        //    if (Accept(TokenKind.Keyword, Keywords.Rule))
        //    {
        //        Recover(() =>
        //        {
        //            throw new NotImplementedException(nameof(RuleDeclaration));

        //        }, TokenKind.Keyword, Keywords.EndRule);

        //        Expect(TokenKind.Keyword, Keywords.EndRule);
        //        Expect(TokenKind.Semicolon);
        //    }

        //    return null;
        //}

        private string ParseStringLiteral()
        {
            if (Accept(TokenKind.SimpleStringLiteral) || Accept(TokenKind.EncodedStringLiteral))
                return CurrentToken.Text;

            return null;
        }

        private void VisitInterfaceSpecification(InterfaceSpecificationBuilder specification, Scope scope)
        {
            switch (specification)
            {
                case UseClauseBuilder useClause:
                    VisitUseClause(useClause, scope);
                    return;
                case ReferenceClauseBuilder referenceClause:
                    VisitReferenceClause(referenceClause, scope);
                    return;
                default:
                    throw new NotSupportedException();
            }
        }

        private void VisitReferenceClause(ReferenceClauseBuilder referenceClause, Scope scope)
        {
            throw new NotImplementedException(nameof(VisitReferenceClause));
        }

        private void VisitUseClause(UseClauseBuilder useClause, Scope scope)
        {
            throw new NotImplementedException(nameof(VisitUseClause));
        }

    }
}
