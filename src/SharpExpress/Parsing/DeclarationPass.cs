using SharpExpress.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    /// <summary>
    /// This pass constructs declarations for syntaxNodes that do not require forward reference
    /// </summary>
    class DeclarationPass : ParserPass
    {
        public DeclarationPass(Token[] tokens, IList<ParsingError> errors = null)
            : base(tokens, errors)
        {
        }

        public override void Run(SyntaxTreeBuilder builder)
        {
            if (Enumerator.TryPeek(out var nextToken))
            {
                Recover(() =>
                {
                    while (CollectSchema(builder.Schemas)) ;
                }, TokenKind.Eof);

                Expect(TokenKind.Eof);
            }
        }


        private bool CollectSchema(IList<SchemaDeclarationBuilder> schemas)
        {
            if (Accept(TokenKind.Keyword, Keywords.Schema))
            {
                Recover(() =>
                {
                    Expect(TokenKind.SimpleId);

                    var schemaBuilder = new SchemaDeclarationBuilder { Name = CurrentToken.Text };
                    schemas.Add(schemaBuilder);

                    Expect(TokenKind.Semicolon);

                    while (CollectInterfaceSpecification(schemaBuilder)) ;

                    CollectConstants(schemaBuilder.Constants);

                    while (CollectDeclaration(schemaBuilder.Declarations) ||
                           CollectRuleDeclaration(schemaBuilder.Declarations)) ;

                }, TokenKind.Keyword, Keywords.EndSchema);

                Expect(TokenKind.Keyword);
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Collets rule declaration
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private bool CollectRuleDeclaration(IList<DeclarationBuilder> declarations)
        {
            if (Accept(TokenKind.Keyword, Keywords.Rule))
            {

                Recover(() =>
                {
                    Expect(TokenKind.SimpleId);

                    var ruleBuilder = new RuleDeclarationBuilder();
                    ruleBuilder.Name = CurrentToken.Text;
                    declarations.Add(ruleBuilder);

                    Expect(TokenKind.Keyword, Keywords.For);

                    Expect(TokenKind.LeftParen);

                    Expect(TokenKind.SimpleId);
                    ruleBuilder.Populations.Add(new VariableDeclarationBuilder
                    {
                        Name = CurrentToken.Text
                    });

                    while (Accept(TokenKind.Comma))
                    {
                        Expect(TokenKind.SimpleId);
                        ruleBuilder.Populations.Add(new VariableDeclarationBuilder
                        {
                            Name = CurrentToken.Text
                        });
                    }

                    Expect(TokenKind.RightParen);

                    Expect(TokenKind.Semicolon);

                    while (CollectDeclaration(ruleBuilder.Declarations)) ;

                    CollectConstants(ruleBuilder.Constants);

                    CollectLocalVariables(ruleBuilder.LocalVariables);

                    while (SkipStatement()) ;

                    CollectDomainRules(ruleBuilder.DomainRules);

                }, TokenKind.Keyword, Keywords.EndRule);

                Expect(TokenKind.Keyword, Keywords.EndRule);
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private bool CollectDeclaration(IList<DeclarationBuilder> declarations)
        {
            if (CollectTypeDeclaration(declarations) ||
                CollectEntityDeclaration(declarations) ||
                CollectFunctionDeclaration(declarations) ||
                CollectProcedureDeclaration(declarations) ||
                CollectSubtypeConstraintDeclaration(declarations))

                return true;

            return false;
        }

        private bool CollectSubtypeConstraintDeclaration(IList<DeclarationBuilder> declarations)
        {
            if (Accept(TokenKind.Keyword, Keywords.SubtypeConstraint))
            {
                Recover(() =>
                {
                    Expect(TokenKind.SimpleId);

                    var constraintBuilder = new SubtypeConstraintDeclarationBuilder { Name = CurrentToken.Text };
                    declarations.Add(constraintBuilder);

                    throw new NotImplementedException(nameof(CollectSubtypeConstraintDeclaration));

                }, TokenKind.Keyword, Keywords.EndSubtypeConstraint);

                Expect(TokenKind.Keyword, Keywords.EndSubtypeConstraint);
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private bool CollectProcedureDeclaration(IList<DeclarationBuilder> declarations)
        {
            if (Accept(TokenKind.Keyword, Keywords.Procedure))
            {
                Recover(() =>
                {
                    Expect(TokenKind.SimpleId);

                    var procedureBuilder = new ProcedureDeclarationBuilder { Name = CurrentToken.Text };
                    declarations.Add(procedureBuilder);

                    CollectParameters(procedureBuilder.Parameters, true);

                    Expect(TokenKind.Semicolon);

                    while (CollectDeclaration(procedureBuilder.Declarations)) ;

                    CollectConstants(procedureBuilder.Constants);

                    CollectLocalVariables(procedureBuilder.LocalVariables);

                    while (SkipStatement()) ;

                }, TokenKind.Keyword, Keywords.EndProcedure);

                Expect(TokenKind.Keyword, Keywords.EndProcedure);
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private DataTypeBuilder ParseParameterType()
        {
            return TryParseGeneralizedType() ?? TryParseSimpleType() ?? ParseReferenceType();
        }

        private DataTypeBuilder ParseReferenceType()
        {
            Expect(TokenKind.SimpleId);

            return new ReferenceDataTypeBuilder
            {
                Reference = new UnresolvedReferenceBuilder
                {
                    UnresolvedName = CurrentToken.Text
                }
            };
        }

        private DataTypeBuilder TryParseGeneralizedType()
        {
            return TryParseAggregateType()
                ?? TryParseGeneralAggregationType()
                ?? TryParseGenericEntityType()
                ?? TryParseGenericType();
        }

        private DataTypeBuilder TryParseGeneralAggregationType()
        {
            return TryParseGeneralArrayType()
                ?? TryParseGeneralListType()
                ?? TryParseGeneralSetType()
                ?? TryParseGeneralBagType();
        }

        private DataTypeBuilder TryParseGenericEntityType()
        {
            if (Accept(TokenKind.Keyword, Keywords.GenericEntity))
            {
                TypeLabelReferenceBuilder typeLabel = null;

                if (Accept(TokenKind.Colon))
                {
                    Expect(TokenKind.SimpleId);

                    typeLabel = new TypeLabelReferenceBuilder { TypeLabelName = CurrentToken.Text };
                }

                return new GenericEntityDataTypeBuilder
                {
                    TypeLabel = typeLabel
                };
            }

            return null;
        }

        private DataTypeBuilder TryParseGenericType()
        {
            if (Accept(TokenKind.Keyword, Keywords.Generic))
            {
                TypeLabelReferenceBuilder typeLabel = null;

                if (Accept(TokenKind.Colon))
                {
                    Expect(TokenKind.SimpleId);

                    typeLabel = new TypeLabelReferenceBuilder { TypeLabelName = CurrentToken.Text };
                }

                return new GenericDataTypeBuilder
                {
                    TypeLabel = typeLabel
                };
            }

            return null;
        }

        private DataTypeBuilder TryParseAggregateType()
        {
            if (Accept(TokenKind.Keyword, Keywords.Aggregate))
            {
                TypeLabelReferenceBuilder typeLabel = null;

                if (Accept(TokenKind.Colon))
                {
                    Expect(TokenKind.SimpleId);

                    typeLabel = new TypeLabelReferenceBuilder { TypeLabelName = CurrentToken.Text };
                }

                Expect(TokenKind.Keyword, Keywords.Of);

                return new AggregateDataTypeBuilder
                {
                    ElementType = ParseParameterType(),
                    TypeLabel = typeLabel
                };
            }

            return null;
        }

        private bool CollectFunctionDeclaration(IList<DeclarationBuilder> declarations)
        {
            if (Accept(TokenKind.Keyword, Keywords.Function))
            {
                Recover(() =>
                {
                    Expect(TokenKind.SimpleId);

                    var functionBuilder = new FunctionDeclarationBuilder { Name = CurrentToken.Text };
                    declarations.Add(functionBuilder);

                    CollectParameters(functionBuilder.Parameters);

                    Expect(TokenKind.Colon);

                    functionBuilder.ReturnType = ParseParameterType();

                    Expect(TokenKind.Semicolon);

                    while (CollectDeclaration(functionBuilder.Declarations)) ;

                    CollectConstants(functionBuilder.Constants);

                    CollectLocalVariables(functionBuilder.LocalVariables);

                    while (SkipStatement()) ;

                }, TokenKind.Keyword, Keywords.EndFunction);

                Expect(TokenKind.Keyword, Keywords.EndFunction);
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private void CollectParameters(IList<ParameterDeclarationBuilder> parameters, bool allowVar = false)
        {
            if (Accept(TokenKind.LeftParen))
            {
                CollectFormalParameter(parameters);

                while (Accept(TokenKind.Semicolon))
                {
                    CollectFormalParameter(parameters);
                }

                Expect(TokenKind.RightParen);
            }

        }

        private void CollectFormalParameter(IList<ParameterDeclarationBuilder> builder, bool allowVariable = false)
        {
            bool isVariable = allowVariable ? Accept(TokenKind.Keyword, Keywords.Var) : false;

            Expect(TokenKind.SimpleId);

            var parameters = new List<ParameterDeclarationBuilder>
            {
                new ParameterDeclarationBuilder { Name = CurrentToken.Text, IsVariable = isVariable }
            };

            while (Accept(TokenKind.Comma))
            {
                Expect(TokenKind.SimpleId);

                parameters.Add(new ParameterDeclarationBuilder { Name = CurrentToken.Text });
            }

            Expect(TokenKind.Colon);

            var parameterType = ParseParameterType();

            foreach (var parameter in parameters)
            {
                parameter.Type = parameterType;
                builder.Add(parameter);
            }
        }

        private bool SkipStatement()
        {
            return SkipAliasStatement()
                || SkipCaseStatement()
                || SkipCompoundStatement()
                || SkipEscapeStatement()
                || SkipIfStatement()
                || SkipNullStatement()
                || SkipProcedureCallOrAssignmentStatement()
                || SkipRepeatStatement()
                || SkipReturnStatement()
                || SkipSkipStatement();

        }

        private bool SkipAliasStatement()
        {
            if (Accept(TokenKind.Keyword, Keywords.Alias))
            {
                Recover(() =>
                {
                    Expect(TokenKind.SimpleId);

                    Expect(TokenKind.Keyword, Keywords.For);

                    Expect(TokenKind.SimpleId);

                    SkipQualifier();

                    Expect(TokenKind.Semicolon);

                    SkipStatement();

                    while (SkipStatement()) ;

                }, TokenKind.Keyword, Keywords.EndAlias);

                Expect(TokenKind.Keyword, Keywords.EndAlias);
                Expect(TokenKind.Semicolon);
            }

            return false;
        }



        private bool SkipCaseStatement()
        {
            if (Accept(TokenKind.Keyword, Keywords.Case))
            {
                Recover(() =>
                {
                    SkipExpression();

                    Expect(TokenKind.Keyword, Keywords.Of);

                    while (SkipCaseAction()) ;

                    if (Accept(TokenKind.Keyword, Keywords.Otherwise))
                    {
                        Expect(TokenKind.Colon);

                        SkipStatement();
                    }

                }, TokenKind.Keyword, Keywords.EndCase);

                Expect(TokenKind.Keyword, Keywords.EndCase);
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private bool SkipCaseAction()
        {
            if (Enumerator.TryPeek(out var token))
            {
                if (token.Kind == TokenKind.Keyword &&
                    (token.Text.Equals(Keywords.Otherwise) ||
                    (token.Text.Equals(Keywords.EndCase))))

                    return false;
            }

            SkipExpression();
            while (Accept(TokenKind.Comma))
                SkipExpression();

            Expect(TokenKind.Colon);

            SkipStatement();

            return true;
        }

        private bool SkipCompoundStatement()
        {
            if (Accept(TokenKind.Keyword, Keywords.Begin))
            {
                Recover(() =>
                {
                    SkipStatement();

                    while (SkipStatement()) ;

                }, TokenKind.Keyword, Keywords.End);

                Expect(TokenKind.Keyword, Keywords.End);
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private bool SkipEscapeStatement()
        {
            if (Accept(TokenKind.Keyword, Keywords.Escape))
            {
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private bool SkipIfStatement()
        {
            if (Accept(TokenKind.Keyword, Keywords.If))
            {
                Recover(() =>
                {
                    SkipExpression();

                    Expect(TokenKind.Keyword, Keywords.Then);

                    SkipStatement();

                    while (SkipStatement()) ;

                    if (Accept(TokenKind.Keyword, Keywords.Else))
                    {
                        SkipStatement();
                        while (SkipStatement()) ;
                    }

                }, TokenKind.Keyword, Keywords.EndIf);

                Expect(TokenKind.Keyword, Keywords.EndIf);
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private bool SkipNullStatement()
        {
            return Accept(TokenKind.Semicolon);
        }

        private bool SkipProcedureCallOrAssignmentStatement()
        {
            if (Accept(TokenKind.SimpleId))
            {
                if (Accept(TokenKind.LeftParen))
                {
                    SkipExpression();

                    while (Accept(TokenKind.Comma))
                        SkipExpression();

                    Expect(TokenKind.RightParen);

                    Expect(TokenKind.Semicolon);
                }
                else
                {
                    if (!Accept(TokenKind.Semicolon))
                    {
                        while (SkipQualifier()) ;

                        Expect(TokenKind.Assignment);

                        SkipExpression();

                        Expect(TokenKind.Semicolon);
                    }
                }


                return true;
            }

            return false;
        }

        private bool SkipRepeatStatement()
        {
            if (Accept(TokenKind.Keyword, Keywords.Repeat))
            {
                Recover(() =>
                {
                    SkipRepeatControl();

                    Expect(TokenKind.Semicolon);

                    while (SkipStatement()) ;

                }, TokenKind.Keyword, Keywords.EndRepeat);

                Expect(TokenKind.Keyword, Keywords.EndRepeat);
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private void SkipRepeatControl()
        {
            SkipIncrementControl();
            SkipWhileControl();
            SkipUntilControl();
        }

        private bool SkipUntilControl()
        {
            if (Accept(TokenKind.Keyword, Keywords.Until))
            {
                SkipExpression();

                return true;
            }

            return false;
        }

        private bool SkipWhileControl()
        {
            if (Accept(TokenKind.Keyword, Keywords.While))
            {
                SkipExpression();

                return true;
            }

            return false;
        }

        private bool SkipIncrementControl()
        {
            if (Accept(TokenKind.SimpleId))
            {
                Expect(TokenKind.Assignment);

                SkipSimpleExpression();

                Expect(TokenKind.Keyword, Keywords.To);

                SkipSimpleExpression();

                if (Accept(TokenKind.Keyword, Keywords.By))
                {
                    SkipSimpleExpression();
                }

                return true;
            }

            return false;
        }

        private bool SkipReturnStatement()
        {
            if (Accept(TokenKind.Keyword, Keywords.Return))
            {
                if (Accept(TokenKind.LeftParen))
                {
                    SkipExpression();

                    Expect(TokenKind.RightParen);
                }

                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private bool SkipSkipStatement()
        {
            if (Accept(TokenKind.Keyword, Keywords.Skip))
            {
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private void CollectLocalVariables(IList<VariableDeclarationBuilder> locals)
        {
            if (Accept(TokenKind.Keyword, Keywords.Local))
            {
                Recover(() =>
                {
                    while (CollectLocalVariable(locals)) ;
                }, TokenKind.Keyword, Keywords.EndLocal);

                Expect(TokenKind.Keyword, Keywords.EndLocal);
                Expect(TokenKind.Semicolon);
            }
        }

        private bool CollectLocalVariable(IList<VariableDeclarationBuilder> locals)
        {
            if (Accept(TokenKind.SimpleId))
            {
                var result = new List<VariableDeclarationBuilder>
                {
                    new VariableDeclarationBuilder
                    {
                        Name = CurrentToken.Text
                    }
                };

                while (Accept(TokenKind.Comma))
                {
                    Expect(TokenKind.SimpleId);

                    result.Add(new VariableDeclarationBuilder
                    {
                        Name = CurrentToken.Text
                    });
                }

                Expect(TokenKind.Colon);

                var localType = ParseParameterType();

                foreach (var local in result)
                {
                    local.Type = localType;
                    locals.Add(local);
                }

                if (Accept(TokenKind.Assignment))
                {
                    SkipExpression();
                }

                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private bool CollectEntityDeclaration(IList<DeclarationBuilder> declarations)
        {
            if (Accept(TokenKind.Keyword, Keywords.Entity))
            {
                Recover(() =>
                {
                    Expect(TokenKind.SimpleId);

                    var entityBuilder = new EntityDeclarationBuilder { Name = CurrentToken.Text };
                    declarations.Add(entityBuilder);

                    CollectSupertypeConstraint(entityBuilder);
                    CollectSubtypeDeclaration(entityBuilder);

                    Expect(TokenKind.Semicolon);

                    while (CollectExplicitAttribute(entityBuilder)) ;

                    CollectDerivedAttributes(entityBuilder);
                    CollectInverseAttributes(entityBuilder);

                    CollectUniqueRules(entityBuilder.UniqueRules);
                    CollectDomainRules(entityBuilder.DomainRules);

                }, TokenKind.Keyword, Keywords.EndEntity);

                Expect(TokenKind.Keyword, Keywords.EndEntity);
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private void CollectSubtypeDeclaration(EntityDeclarationBuilder builder)
        {
            if (Accept(TokenKind.Keyword, Keywords.Subtype))
            {
                Expect(TokenKind.Keyword, Keywords.Of);
                Expect(TokenKind.LeftParen);

                Expect(TokenKind.SimpleId);
                builder.Supertypes.Add(new EntityReferenceBuilder
                {
                    EntityName = CurrentToken.Text
                });

                while (Accept(TokenKind.Comma))
                {
                    Expect(TokenKind.SimpleId);
                    builder.Supertypes.Add(new EntityReferenceBuilder
                    {
                        EntityName = CurrentToken.Text
                    });
                }

                Expect(TokenKind.RightParen);
            }
        }

        private bool CollectExplicitAttribute(EntityDeclarationBuilder builder)
        {
            var attributeBuilder = new ExplicitAttributeDeclarationBuilder();

            if (CollectAttributeName(attributeBuilder))
            {
                var attributes = new List<ExplicitAttributeDeclarationBuilder> { attributeBuilder };

                while (Accept(TokenKind.Comma))
                {
                    attributeBuilder = new ExplicitAttributeDeclarationBuilder();
                    CollectAttributeName(attributeBuilder, false);
                    attributes.Add(attributeBuilder);
                }

                Expect(TokenKind.Colon);

                bool optional = Accept(TokenKind.Keyword, Keywords.Optional);

                var attributeType = ParseParameterType();

                foreach (var attribute in attributes)
                {
                    attribute.IsOptional = optional;
                    attribute.Type = attributeType;
                }

                Expect(TokenKind.Semicolon);

                builder.ExplicitAttributes.AddRange(attributes);

                return true;
            }

            return false;
        }

        private void CollectUniqueRules(IList<UniqueRuleDeclarationBuilder> rules)
        {
            if (Accept(TokenKind.Keyword, Keywords.Unique))
            {
                while (CollectUniqueRule(rules)) ;
            }
        }

        private bool CollectUniqueRule(IList<UniqueRuleDeclarationBuilder> rules)
        {
            var ruleBuilder = new UniqueRuleDeclarationBuilder();

            if (Accept(TokenKind.SimpleId))
            {
                rules.Add(ruleBuilder);

                var ruleToken = CurrentToken;

                if (Accept(TokenKind.Colon))
                {
                    ruleBuilder.Name = ruleToken.Text;

                    SkipReferencedAttribute();
                }

                while (Accept(TokenKind.Comma))
                {
                    SkipReferencedAttribute();
                }

                Expect(TokenKind.Semicolon);

                return true;
            }
            else if (SkipQualifiedAttribute())
            {
                rules.Add(ruleBuilder);

                while (Accept(TokenKind.Comma))
                {
                    SkipReferencedAttribute();
                }

                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private void SkipReferencedAttribute()
        {
            if (!SkipQualifiedAttribute())
            {
                Expect(TokenKind.SimpleId);
            }
        }

        private bool SkipQualifiedAttribute()
        {
            if (Accept(TokenKind.Keyword, Keywords.Self))
            {
                SkipGroupQualifier();
                SkipAttributeQualifier();

                return true;
            }

            return false;
        }

        private bool CollectInverseAttributes(EntityDeclarationBuilder builder)
        {
            if (Accept(TokenKind.Keyword, Keywords.Inverse))
            {
                while (CollectInverseAttribute(builder)) ;

                return true;
            }

            return false;
        }

        private bool CollectInverseAttribute(EntityDeclarationBuilder builder)
        {
            var attributeBuilder = new InverseAttributeDeclarationBuilder();

            if (CollectAttributeName(attributeBuilder))
            {
                builder.InverseAttributes.Add(attributeBuilder);

                Expect(TokenKind.Colon);

                if (Accept(TokenKind.Keyword, Keywords.Set))
                {
                    SkipBoundSpec();

                    Expect(TokenKind.Keyword, Keywords.Of);
                    Expect(TokenKind.SimpleId);

                    attributeBuilder.Type = new SetDataTypeBuilder
                    {
                        ElementType = new ReferenceDataTypeBuilder
                        {
                            Reference = new EntityReferenceBuilder
                            {
                                EntityName = CurrentToken.Text
                            }
                        }
                    };
                }
                else if (Accept(TokenKind.Keyword, Keywords.Bag))
                {
                    SkipBoundSpec();

                    Expect(TokenKind.Keyword, Keywords.Of);
                    Expect(TokenKind.SimpleId);

                    attributeBuilder.Type = new BagDataTypeBuilder
                    {
                        ElementType = new ReferenceDataTypeBuilder
                        {
                            Reference = new EntityReferenceBuilder
                            {
                                EntityName = CurrentToken.Text
                            }
                        }
                    };
                }
                else
                {
                    Expect(TokenKind.SimpleId);

                    attributeBuilder.Type = new ReferenceDataTypeBuilder
                    {
                        Reference = new EntityReferenceBuilder
                        {
                            EntityName = CurrentToken.Text
                        }
                    };
                }

                Expect(TokenKind.Keyword, Keywords.For);
                Expect(TokenKind.SimpleId);

                var reference = CurrentToken.Text;

                //TODO: qualified attribute?
                if (Accept(TokenKind.Period))
                {
                    Expect(TokenKind.SimpleId);


                    attributeBuilder.ForEntity = new EntityReferenceBuilder { EntityName = reference };
                    attributeBuilder.ForAttribute = new AttributeReferenceBuilder { AttributeName = CurrentToken.Text };
                }
                else
                {
                    attributeBuilder.ForAttribute = new AttributeReferenceBuilder { AttributeName = CurrentToken.Text };
                }

                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private void CollectDerivedAttributes(EntityDeclarationBuilder builder)
        {
            if (Accept(TokenKind.Keyword, Keywords.Derive))
            {
                while (CollectDerivedAttributeDeclaration(builder)) ;
            }
        }

        private bool CollectDerivedAttributeDeclaration(EntityDeclarationBuilder builder)
        {
            var attributeBuilder = new DerivedAttributeDeclarationBuilder();

            if (CollectAttributeName(attributeBuilder))
            {
                builder.DerivedAttributes.Add(attributeBuilder);

                Expect(TokenKind.Colon);

                attributeBuilder.Type = ParseParameterType();

                Expect(TokenKind.Assignment);

                SkipExpression();

                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private QualifiedReferenceBuilder TryParseQualifiedAttribute()
        {
            if (Accept(TokenKind.Keyword, Keywords.Self))
            {
                var result = new QualifiedReferenceBuilder
                {
                    Reference = new VariableReferenceBuilder { VariableName = Keywords.Self }
                };

                result.Qualifiers.Add(ParseGroupQualifier());
                result.Qualifiers.Add(ParseAttributeQualifier());

                return result;
            }

            return null;
        }

        private bool CollectAttributeName(AttributeDeclarationBuilder builder, bool optional = true)
        {
            if (TryParseQualifiedAttribute() is QualifiedReferenceBuilder attribute)
            {
                if (Accept(TokenKind.Keyword, Keywords.Renamed))
                {
                    Expect(TokenKind.SimpleId);

                    builder.Name = CurrentToken.Text;
                    builder.RedeclaredAttribute = attribute;

                    return true;
                }

                //TODO: attribute derived without renaming support 
                builder.RedeclaredAttribute = attribute;
                builder.Name = ((AttributeQualifierBuilder)attribute.Qualifiers[1]).Attribute.AttributeName;
                return true;
            }
            else if (Accept(TokenKind.SimpleId))
            {
                builder.Name = CurrentToken.Text;
                return true;
            }
            else if (!optional)
            {
                Expect(TokenKind.SimpleId);
            }

            return false;
        }


        private void CollectSupertypeConstraint(EntityDeclarationBuilder builder)
        {
            builder.IsAbstract = Accept(TokenKind.Keyword, Keywords.Abstract);

            if (Accept(TokenKind.Keyword, Keywords.Supertype))
            {
                if (builder.IsAbstract)
                    builder.IsAbstractSupertype = true;

                CollectSubtypeConstraint(builder);
            }
        }

        private void CollectSubtypeConstraint(EntityDeclarationBuilder builder)
        {
            if (Accept(TokenKind.Keyword, Keywords.Of))
            {
                Expect(TokenKind.LeftParen);

                builder.Subtype = ParseTypeExpression();

                Expect(TokenKind.RightParen);
            }
        }

        private TypeExpressionBuilder ParseTypeExpression()
        {
            var expression = ParseTypeFactor();

            while (Accept(TokenKind.Keyword, Keywords.AndOr))
            {
                expression = new AndOrTypeExpressionBuilder
                {
                    Left = expression,
                    Right = ParseTypeFactor()
                };
            }

            return expression;
        }

        private TypeExpressionBuilder ParseTypeFactor()
        {
            var expression = ParseTypeTerm();

            while (Accept(TokenKind.Keyword, Keywords.And))
            {
                expression = new AndTypeExpressionBuilder
                {
                    Left = expression,
                    Right = ParseTypeTerm()
                };
            }

            return expression;
        }

        private TypeExpressionBuilder ParseTypeTerm()
        {
            if (Accept(TokenKind.Keyword, Keywords.Oneof))
            {
                var expression = new OneOfTypeExpressionBuilder();



                Expect(TokenKind.LeftParen);

                expression.Expressions.Add(ParseTypeExpression());

                while (Accept(TokenKind.Comma))
                    expression.Expressions.Add(ParseTypeExpression());

                Expect(TokenKind.RightParen);

                return expression;
            }
            else if (Accept(TokenKind.LeftParen))
            {
                var expression = ParseTypeExpression();

                Expect(TokenKind.RightParen);

                return expression;
            }
            else
            {
                Expect(TokenKind.SimpleId);

                return new EntityReferenceTypeExpressionBuilder
                {
                    Entity = new EntityReferenceBuilder
                    {
                        EntityName = CurrentToken.Text
                    }
                };
            }
        }



        private bool CollectTypeDeclaration(IList<DeclarationBuilder> declarations)
        {
            if (Accept(TokenKind.Keyword, Keywords.Type))
            {
                Recover(() =>
                {
                    Expect(TokenKind.SimpleId);

                    var typeBuilder = new TypeDeclarationBuilder { Name = CurrentToken.Text };
                    declarations.Add(typeBuilder);

                    Expect(TokenKind.Equal);

                    typeBuilder.UnderlyingType = ParseUnderlyingType();

                    Expect(TokenKind.Semicolon);

                    CollectDomainRules(typeBuilder.DomainRules);

                }, TokenKind.Keyword, Keywords.EndType);

                Expect(TokenKind.Keyword, Keywords.EndType);
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private void CollectDomainRules(IList<DomainRuleDeclarationBuilder> domainRules)
        {
            if (Accept(TokenKind.Keyword, Keywords.Where))
            {
                while (CollectDomainRule(domainRules))
                {
                    Expect(TokenKind.Semicolon);
                }
            }
        }

        private bool CollectDomainRule(IList<DomainRuleDeclarationBuilder> domainRules)
        {
            if (Enumerator.TryPeek(out var next) && next.Kind == TokenKind.Keyword)
            {
                switch (next.Text)
                {
                    case Keywords.EndEntity:
                    case Keywords.EndRule:
                    case Keywords.EndType:
                        return false;
                }
            }

            if (Enumerator.TryPeek2(out var first, out var second) &&
                first.Kind == TokenKind.SimpleId && second.Kind == TokenKind.Colon)
            {
                Enumerator.MoveNext();

                var ruleBuilder = new DomainRuleDeclarationBuilder
                {
                    Name = CurrentToken.Text
                };

                Enumerator.MoveNext();

                domainRules.Add(ruleBuilder);

                SkipExpression();
            }
            else
            {
                domainRules.Add(new DomainRuleDeclarationBuilder());
                SkipExpression();
            }

            return true;
        }


        private DataTypeBuilder ParseUnderlyingType()
        {
            var result = TryParseAggregationType() ??
                TryParseConstructedType() ??
                TryParseSimpleType();

            if (result != null)
                return result;

            Expect(TokenKind.SimpleId);

            return new ReferenceDataTypeBuilder
            {
                Reference = new TypeReferenceBuilder
                {
                    TypeName = CurrentToken.Text
                }
            };
        }

        private DataTypeBuilder TryParseConstructedType()
        {
            bool extensible = Accept(TokenKind.Keyword, Keywords.Extensible);

            var result = TryParseEnumerationType(extensible) ??
                TryParseSelectType(extensible);

            if (extensible && result == null)
            {
                //TODO: error - extensible forces constructed type
                throw new NotImplementedException();
            }

            return result;
        }

        private DataTypeBuilder TryParseSimpleType()
        {
            if (Accept(TokenKind.Keyword, Keywords.String))
            {
                bool isFixed = false;
                if (Accept(TokenKind.LeftParen))
                {
                    SkipSimpleExpression();

                    Expect(TokenKind.RightParen);

                    isFixed = Accept(TokenKind.Keyword, Keywords.Fixed);
                }

                return new StringDataTypeBuilder { IsFixed = isFixed };
            }
            else if (Accept(TokenKind.Keyword, Keywords.Binary))
            {
                bool isFixed = false;
                if (Accept(TokenKind.LeftParen))
                {
                    SkipSimpleExpression();

                    Expect(TokenKind.RightParen);

                    Accept(TokenKind.Keyword, Keywords.Fixed);
                }

                return new BinaryDataTypeBuilder { IsFixed = isFixed };
            }
            else if (Accept(TokenKind.Keyword, Keywords.Real))
            {
                if (Accept(TokenKind.LeftParen))
                {
                    SkipSimpleExpression();

                    Expect(TokenKind.RightParen);
                }

                return new RealDataTypeBuilder();
            }

            else if (Accept(TokenKind.Keyword, Keywords.Boolean))
                return new BooleanDataTypeBuilder();
            else if (Accept(TokenKind.Keyword, Keywords.Integer))
                return new IntegerDataTypeBuilder();
            else if (Accept(TokenKind.Keyword, Keywords.Logical))
                return new LogicalDataTypeBuilder();
            else if (Accept(TokenKind.Keyword, Keywords.Number))
                return new NumberDataTypeBuilder();

            return null;
        }

        private DataTypeBuilder TryParseSelectType(bool extensible)
        {
            var selectBuilder = new SelectDataTypeBuilder { IsExtensible = extensible };

            if (Accept(TokenKind.Keyword, Keywords.GenericEntity))
            {
                selectBuilder.IsGenericIdentity = true;

                Expect(TokenKind.Keyword, Keywords.Select);
            }
            else if (!Accept(TokenKind.Keyword, Keywords.Select))
            {
                return null;
            }

            if (Accept(TokenKind.LeftParen))
            {
                Expect(TokenKind.SimpleId);

                selectBuilder.Items.Add(new ReferenceDataTypeBuilder
                {
                    Reference = new UnresolvedReferenceBuilder
                    {
                        UnresolvedName = CurrentToken.Text
                    }
                });

                while (Accept(TokenKind.Comma))
                {
                    Expect(TokenKind.SimpleId);

                    selectBuilder.Items.Add(new ReferenceDataTypeBuilder
                    {
                        Reference = new UnresolvedReferenceBuilder
                        {
                            UnresolvedName = CurrentToken.Text
                        }
                    });
                }

                Expect(TokenKind.RightParen);

                return selectBuilder;
            }
            else
            {
                Expect(TokenKind.Keyword, Keywords.BasedOn);
                Expect(TokenKind.SimpleId);

                selectBuilder.BasedOn = new TypeReferenceBuilder { TypeName = CurrentToken.Text };

                if (Accept(TokenKind.Keyword, Keywords.With))
                {
                    Expect(TokenKind.LeftParen);

                    Expect(TokenKind.SimpleId);

                    selectBuilder.Items.Add(new ReferenceDataTypeBuilder
                    {
                        Reference = new UnresolvedReferenceBuilder
                        {
                            UnresolvedName = CurrentToken.Text
                        }
                    });

                    while (Accept(TokenKind.Comma))
                    {
                        Expect(TokenKind.SimpleId);

                        selectBuilder.Items.Add(new ReferenceDataTypeBuilder
                        {
                            Reference = new UnresolvedReferenceBuilder
                            {
                                UnresolvedName = CurrentToken.Text
                            }
                        });
                    }

                    Expect(TokenKind.RightParen);
                }
            }

            return selectBuilder;
        }

        private DataTypeBuilder TryParseEnumerationType(bool extensible)
        {
            if (Accept(TokenKind.Keyword, Keywords.Enumeration))
            {
                var builder = new EnumerationDataTypeBuilder();

                if (Accept(TokenKind.Keyword, Keywords.Of))
                {
                    CollectEnumerationItems(builder);

                    return builder;
                }
                else
                {
                    Expect(TokenKind.Keyword, Keywords.BasedOn);

                    Expect(TokenKind.SimpleId);

                    builder.BasedOn = new TypeReferenceBuilder { TypeName = CurrentToken.Text };

                    if (Accept(TokenKind.Keyword, Keywords.With))
                        CollectEnumerationItems(builder);

                    return builder;
                }
            }

            return null;
        }

        private void CollectEnumerationItems(EnumerationDataTypeBuilder builder)
        {
            Expect(TokenKind.LeftParen);

            Expect(TokenKind.SimpleId);

            builder.Items.Add(new EnumerationDeclarationBuilder { Name = CurrentToken.Text });

            while (Accept(TokenKind.Comma))
            {
                Expect(TokenKind.SimpleId);

                builder.Items.Add(new EnumerationDeclarationBuilder { Name = CurrentToken.Text });
            }

            Expect(TokenKind.RightParen);
        }

        private DataTypeBuilder TryParseAggregationType()
        {
            return TryParseArrayType()
                ?? TryParseListType()
                ?? TryParseBagType()
                ?? TryParseSetType();
        }


        private DataTypeBuilder TryParseGeneralArrayType()
        {
            if (Accept(TokenKind.Keyword, Keywords.Array))
            {
                SkipBoundSpec();

                Expect(TokenKind.Keyword, Keywords.Of);

                return new ArrayDataTypeBuilder
                {
                    IsOptional = Accept(TokenKind.Keyword, Keywords.Optional),
                    IsUnique = Accept(TokenKind.Keyword, Keywords.Unique),
                    ElementType = ParseParameterType()
                };
            }

            return null;
        }

        private DataTypeBuilder TryParseArrayType()
        {
            if (Accept(TokenKind.Keyword, Keywords.Array))
            {
                SkipBoundSpec();

                Expect(TokenKind.Keyword, Keywords.Of);

                return new ArrayDataTypeBuilder
                {
                    IsOptional = Accept(TokenKind.Keyword, Keywords.Optional),
                    IsUnique = Accept(TokenKind.Keyword, Keywords.Unique),
                    ElementType = ParseInsantiableType()
                };
            }

            return null;
        }

        private DataTypeBuilder ParseInsantiableType()
        {
            return TryParseAggregationType()
                ?? TryParseSimpleType()
                ?? ParseReferenceType();
        }

        private bool SkipBoundSpec()
        {
            if (Accept(TokenKind.LeftBracket))
            {
                SkipSimpleExpression();

                Expect(TokenKind.Colon);

                SkipSimpleExpression();

                Expect(TokenKind.RightBracket);

                return true;
            }

            return false;
        }

        private void SkipExpression()
        {
            SkipSimpleExpression();

            while (AcceptRelationalOperatorExtended())
                SkipSimpleExpression();
        }

        private void SkipSimpleExpression()
        {
            SkipTerm();

            while (AcceptAddLikeOperator())
            {
                SkipTerm();
            }
        }

        private void SkipTerm()
        {
            SkipFactor();

            while (AcceptMultiplicationLikeOperator())
            {
                SkipFactor();
            }
        }

        private void SkipFactor()
        {
            SkipSimpleFactor();

            while (Accept(TokenKind.Exponent))
            {
                SkipSimpleFactor();
            }
        }

        private void SkipSimpleFactor()
        {
            if (!CollectAggregateInitializer() &&
                !SkipInterval() &&
                !SkipQueryExpression())
            {
                AcceptUnaryOpeartor();

                if (Accept(TokenKind.LeftParen))
                {
                    SkipExpression();

                    Expect(TokenKind.RightParen);
                }
                else if (!SkipLiteral() && !SkipBuiltinConstant() && !SkipBuiltinFunction())
                {
                    Expect(TokenKind.SimpleId);
                }


                if (Accept(TokenKind.LeftParen))
                {
                    if (!Accept(TokenKind.RightParen))
                    {
                        SkipExpression();

                        while (Accept(TokenKind.Comma))
                            SkipExpression();

                        Expect(TokenKind.RightParen);
                    }
                }

                while (SkipQualifier()) ;
            }
        }

        private bool SkipQualifier()
        {
            if (SkipAttributeQualifier()
                || SkipGroupQualifier())
            {
                return true;
            }
            else if (SkipIndexQualifier())
            {
                return true;
            }

            return false;
        }

        private GroupQualifierBuilder ParseGroupQualifier()
        {
            Expect(TokenKind.Backslash);
            Expect(TokenKind.SimpleId);

            return new GroupQualifierBuilder
            {
                Entity = new EntityReferenceBuilder
                {
                    EntityName = CurrentToken.Text
                }
            };
        }

        private AttributeQualifierBuilder ParseAttributeQualifier()
        {
            Expect(TokenKind.Period);
            Expect(TokenKind.SimpleId);

            return new AttributeQualifierBuilder
            {
                Attribute = new AttributeReferenceBuilder
                {
                    AttributeName = CurrentToken.Text
                }
            };
        }

        private bool SkipGroupQualifier()
        {
            if (Accept(TokenKind.Backslash))
            {
                Expect(TokenKind.SimpleId);

                return true;
            }

            return false;
        }

        private bool SkipIndexQualifier()
        {
            if (Accept(TokenKind.LeftBracket))
            {
                SkipSimpleExpression();

                if (Accept(TokenKind.Colon))
                {
                    SkipSimpleExpression();
                }

                Expect(TokenKind.RightBracket);

                return true;
            }

            return false;
        }

        private bool SkipAttributeQualifier()
        {
            if (Accept(TokenKind.Period))
            {
                Expect(TokenKind.SimpleId);

                return true;
            }

            return false;
        }

        private bool SkipBuiltinFunction()
        {
            if (Enumerator.TryPeek(out var token))
            {
                if (token.Kind == TokenKind.Keyword && Keywords.IsBuiltInFunction(token.Text))
                {
                    Enumerator.MoveNext();
                    return true;
                }
            }

            return false;
        }

        private bool SkipBuiltinConstant()
        {
            if (Accept(TokenKind.QuestionMark))
                return true;

            if (Enumerator.TryPeek(out var token))
            {
                if (token.Kind == TokenKind.Keyword && Keywords.IsBuiltInConstant(token.Text))
                {
                    Enumerator.MoveNext();

                    return true;
                }
            }

            return false;
        }



        private bool SkipLiteral()
        {
            return Accept(TokenKind.BinaryLiteral)
                || Accept(TokenKind.RealLiteral)
                || Accept(TokenKind.IntegerLiteral)
                || Accept(TokenKind.SimpleStringLiteral)
                || Accept(TokenKind.EncodedStringLiteral);
        }


        private bool SkipQueryExpression()
        {
            if (Accept(TokenKind.Keyword, Keywords.Query))
            {
                Expect(TokenKind.LeftParen);

                Expect(TokenKind.SimpleId);

                Expect(TokenKind.Query);

                SkipSimpleExpression();

                Expect(TokenKind.Pipe);

                SkipExpression();

                Expect(TokenKind.RightParen);

                return true;
            }

            return false;
        }

        private bool SkipInterval()
        {
            if (Accept(TokenKind.LeftBrace))
            {
                SkipSimpleExpression();

                if (!Accept(TokenKind.LessThan))
                    Expect(TokenKind.LessThanOrEqual);

                SkipSimpleExpression();

                if (!Accept(TokenKind.LessThan))
                    Expect(TokenKind.LessThanOrEqual);

                SkipSimpleExpression();

                Expect(TokenKind.RightBrace);

                return true;
            }

            return false;
        }

        private bool CollectAggregateInitializer()
        {
            if (Accept(TokenKind.LeftBracket))
            {
                if (Accept(TokenKind.RightBracket))
                    return true;

                else
                {
                    SkipSimpleExpression();

                    if (Accept(TokenKind.Colon))
                    {
                        SkipSimpleExpression();
                    }

                    while (Accept(TokenKind.Comma))
                    {
                        SkipSimpleExpression();

                        if (Accept(TokenKind.Colon))
                        {
                            SkipSimpleExpression();
                        }
                    }

                    Expect(TokenKind.RightBracket);
                }
                return true;
            }

            return false;
        }

        private DataTypeBuilder TryParseListType()
        {
            if (Accept(TokenKind.Keyword, Keywords.List))
            {
                SkipBoundSpec();

                Expect(TokenKind.Keyword, Keywords.Of);

                return new ListDataTypeBuilder
                {
                    IsUnique = Accept(TokenKind.Keyword, Keywords.Unique),
                    ElementType = ParseInsantiableType()
                };
            }

            return null;
        }

        private DataTypeBuilder TryParseGeneralListType()
        {
            if (Accept(TokenKind.Keyword, Keywords.List))
            {
                SkipBoundSpec();

                Expect(TokenKind.Keyword, Keywords.Of);

                return new ListDataTypeBuilder
                {
                    IsUnique = Accept(TokenKind.Keyword, Keywords.Unique),
                    ElementType = ParseParameterType()
                };
            }

            return null;
        }

        private DataTypeBuilder TryParseBagType()
        {
            if (Accept(TokenKind.Keyword, Keywords.Bag))
            {
                SkipBoundSpec();

                Expect(TokenKind.Keyword, Keywords.Of);

                return new BagDataTypeBuilder
                {
                    ElementType = ParseInsantiableType()
                };
            }

            return null;
        }

        private DataTypeBuilder TryParseGeneralBagType()
        {
            if (Accept(TokenKind.Keyword, Keywords.Bag))
            {
                SkipBoundSpec();

                Expect(TokenKind.Keyword, Keywords.Of);

                return new BagDataTypeBuilder
                {
                    ElementType = ParseParameterType()
                };
            }

            return null;
        }

        private DataTypeBuilder TryParseSetType()
        {
            if (Accept(TokenKind.Keyword, Keywords.Set))
            {
                SkipBoundSpec();

                Expect(TokenKind.Keyword, Keywords.Of);

                return new SetDataTypeBuilder
                {
                    ElementType = ParseInsantiableType()
                };
            }

            return null;
        }

        private DataTypeBuilder TryParseGeneralSetType()
        {
            if (Accept(TokenKind.Keyword, Keywords.Set))
            {
                SkipBoundSpec();

                Expect(TokenKind.Keyword, Keywords.Of);

                return new SetDataTypeBuilder
                {
                    ElementType = ParseParameterType()
                };
            }

            return null;
        }

        private bool CollectConstants(IList<ConstantDeclarationBuilder> constants)
        {
            if (Accept(TokenKind.Keyword, Keywords.Constant))
            {
                Recover(() =>
                {
                    while (CollectConstant(constants)) ;

                }, TokenKind.Keyword, Keywords.EndConstant);

                Expect(TokenKind.Keyword, Keywords.EndConstant);
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private bool CollectConstant(IList<ConstantDeclarationBuilder> constants)
        {
            if (Accept(TokenKind.SimpleId))
            {
                var constantBuilder = new ConstantDeclarationBuilder { Name = CurrentToken.Text };
                constants.Add(constantBuilder);

                Expect(TokenKind.Colon);

                constantBuilder.Type = ParseInsantiableType();

                Expect(TokenKind.Assignment);

                SkipExpression();

                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private bool CollectInterfaceSpecification(SchemaDeclarationBuilder builder)
        {
            if (Accept(TokenKind.Keyword, Keywords.Reference))
            {
                Expect(TokenKind.Keyword, Keywords.From);

                Expect(TokenKind.SimpleId);

                var interfaceSpecBuilder = new UseClauseBuilder
                {
                    Schema = new SchemaReferenceBuilder { SchemaName = CurrentToken.Text }
                };

                if (Accept(TokenKind.LeftParen))
                {
                    CollectRenamedReference(interfaceSpecBuilder);

                    while (Accept(TokenKind.Comma))
                        CollectRenamedReference(interfaceSpecBuilder);

                    Expect(TokenKind.RightParen);
                }

                Expect(TokenKind.Semicolon);

                return true;

            }
            else if (Accept(TokenKind.Keyword, Keywords.Use))
            {
                Expect(TokenKind.Keyword, Keywords.From);

                Expect(TokenKind.SimpleId);

                var interfaceSpecBuilder = new ReferenceClauseBuilder
                {
                    Schema = new SchemaReferenceBuilder { SchemaName = CurrentToken.Text }
                };

                builder.InterfaceSpecifications.Add(interfaceSpecBuilder);

                if (Accept(TokenKind.LeftParen))
                {
                    CollectRenamedReference(interfaceSpecBuilder);

                    while (Accept(TokenKind.Comma))
                        CollectRenamedReference(interfaceSpecBuilder);

                    Expect(TokenKind.RightParen);

                }
                Expect(TokenKind.Semicolon);

                return true;
            }

            return false;
        }

        private void CollectRenamedReference(InterfaceSpecificationBuilder builder)
        {
            Expect(TokenKind.SimpleId);

            ReferenceBuilder reference = new UnresolvedReferenceBuilder
            {
                UnresolvedName = CurrentToken.Text
            };

            if (Accept(TokenKind.Keyword, Keywords.As))
            {
                Expect(TokenKind.SimpleId);

                reference = new RenamedReferenceBuilder
                {
                    Name = CurrentToken.Text,
                    Reference = reference
                };
            }

            builder.References.Add(reference);
        }
    }
}
