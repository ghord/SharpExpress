

namespace SharpExpress
{
    public partial class SyntaxNodeVisitor
    {
        public virtual void VisitAggregateDataType(AggregateDataType aggregateDataType)
        {
            DefaultVisit(aggregateDataType);
        }

        public virtual void VisitListDataType(ListDataType listDataType)
        {
            DefaultVisit(listDataType);
        }

        public virtual void VisitArrayDataType(ArrayDataType arrayDataType)
        {
            DefaultVisit(arrayDataType);
        }

        public virtual void VisitBagDataType(BagDataType bagDataType)
        {
            DefaultVisit(bagDataType);
        }

        public virtual void VisitSetDataType(SetDataType setDataType)
        {
            DefaultVisit(setDataType);
        }

        public virtual void VisitEnumerationDataType(EnumerationDataType enumerationDataType)
        {
            DefaultVisit(enumerationDataType);
        }

        public virtual void VisitSelectDataType(SelectDataType selectDataType)
        {
            DefaultVisit(selectDataType);
        }

        public virtual void VisitReferenceDataType(ReferenceDataType referenceDataType)
        {
            DefaultVisit(referenceDataType);
        }

        public virtual void VisitStringDataType(StringDataType stringDataType)
        {
            DefaultVisit(stringDataType);
        }

        public virtual void VisitBinaryDataType(BinaryDataType binaryDataType)
        {
            DefaultVisit(binaryDataType);
        }

        public virtual void VisitRealDataType(RealDataType realDataType)
        {
            DefaultVisit(realDataType);
        }

        public virtual void VisitBooleanDataType(BooleanDataType booleanDataType)
        {
            DefaultVisit(booleanDataType);
        }

        public virtual void VisitLogicalDataType(LogicalDataType logicalDataType)
        {
            DefaultVisit(logicalDataType);
        }

        public virtual void VisitIntegerDataType(IntegerDataType integerDataType)
        {
            DefaultVisit(integerDataType);
        }

        public virtual void VisitNumberDataType(NumberDataType numberDataType)
        {
            DefaultVisit(numberDataType);
        }

        public virtual void VisitGenericDataType(GenericDataType genericDataType)
        {
            DefaultVisit(genericDataType);
        }

        public virtual void VisitGenericEntityDataType(GenericEntityDataType genericEntityDataType)
        {
            DefaultVisit(genericEntityDataType);
        }

        public virtual void VisitSchemaDeclaration(SchemaDeclaration schemaDeclaration)
        {
            DefaultVisit(schemaDeclaration);
        }

        public virtual void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
        {
            DefaultVisit(typeDeclaration);
        }

        public virtual void VisitExplicitAttributeDeclaration(ExplicitAttributeDeclaration explicitAttributeDeclaration)
        {
            DefaultVisit(explicitAttributeDeclaration);
        }

        public virtual void VisitInverseAttributeDeclaration(InverseAttributeDeclaration inverseAttributeDeclaration)
        {
            DefaultVisit(inverseAttributeDeclaration);
        }

        public virtual void VisitDerivedAttributeDeclaration(DerivedAttributeDeclaration derivedAttributeDeclaration)
        {
            DefaultVisit(derivedAttributeDeclaration);
        }

        public virtual void VisitEntityDeclaration(EntityDeclaration entityDeclaration)
        {
            DefaultVisit(entityDeclaration);
        }

        public virtual void VisitConstantDeclaration(ConstantDeclaration constantDeclaration)
        {
            DefaultVisit(constantDeclaration);
        }

        public virtual void VisitEnumerationDeclaration(EnumerationDeclaration enumerationDeclaration)
        {
            DefaultVisit(enumerationDeclaration);
        }

        public virtual void VisitProcedureDeclaration(ProcedureDeclaration procedureDeclaration)
        {
            DefaultVisit(procedureDeclaration);
        }

        public virtual void VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
        {
            DefaultVisit(functionDeclaration);
        }

        public virtual void VisitRuleDeclaration(RuleDeclaration ruleDeclaration)
        {
            DefaultVisit(ruleDeclaration);
        }

        public virtual void VisitDomainRuleDeclaration(DomainRuleDeclaration domainRuleDeclaration)
        {
            DefaultVisit(domainRuleDeclaration);
        }

        public virtual void VisitUniqueRuleDeclaration(UniqueRuleDeclaration uniqueRuleDeclaration)
        {
            DefaultVisit(uniqueRuleDeclaration);
        }

        public virtual void VisitParameterDeclaration(ParameterDeclaration parameterDeclaration)
        {
            DefaultVisit(parameterDeclaration);
        }

        public virtual void VisitVariableDeclaration(VariableDeclaration variableDeclaration)
        {
            DefaultVisit(variableDeclaration);
        }

        public virtual void VisitSubtypeConstraintDeclaration(SubtypeConstraintDeclaration subtypeConstraintDeclaration)
        {
            DefaultVisit(subtypeConstraintDeclaration);
        }

        public virtual void VisitTypeLabelDeclaration(TypeLabelDeclaration typeLabelDeclaration)
        {
            DefaultVisit(typeLabelDeclaration);
        }

        public virtual void VisitLessThanExpression(LessThanExpression lessThanExpression)
        {
            DefaultVisit(lessThanExpression);
        }

        public virtual void VisitGreaterThanExpression(GreaterThanExpression greaterThanExpression)
        {
            DefaultVisit(greaterThanExpression);
        }

        public virtual void VisitLessThanOrEqualExpression(LessThanOrEqualExpression lessThanOrEqualExpression)
        {
            DefaultVisit(lessThanOrEqualExpression);
        }

        public virtual void VisitGreaterThanOrEqualExpression(GreaterThanOrEqualExpression greaterThanOrEqualExpression)
        {
            DefaultVisit(greaterThanOrEqualExpression);
        }

        public virtual void VisitNotEqualExpression(NotEqualExpression notEqualExpression)
        {
            DefaultVisit(notEqualExpression);
        }

        public virtual void VisitEqualExpression(EqualExpression equalExpression)
        {
            DefaultVisit(equalExpression);
        }

        public virtual void VisitInstanceNotEqualExpression(InstanceNotEqualExpression instanceNotEqualExpression)
        {
            DefaultVisit(instanceNotEqualExpression);
        }

        public virtual void VisitInstanceEqualExpression(InstanceEqualExpression instanceEqualExpression)
        {
            DefaultVisit(instanceEqualExpression);
        }

        public virtual void VisitInExpression(InExpression inExpression)
        {
            DefaultVisit(inExpression);
        }

        public virtual void VisitAdditionExpression(AdditionExpression additionExpression)
        {
            DefaultVisit(additionExpression);
        }

        public virtual void VisitSubtractionExpression(SubtractionExpression subtractionExpression)
        {
            DefaultVisit(subtractionExpression);
        }

        public virtual void VisitOrExpression(OrExpression orExpression)
        {
            DefaultVisit(orExpression);
        }

        public virtual void VisitXorExpression(XorExpression xorExpression)
        {
            DefaultVisit(xorExpression);
        }

        public virtual void VisitMultiplicationExpression(MultiplicationExpression multiplicationExpression)
        {
            DefaultVisit(multiplicationExpression);
        }

        public virtual void VisitDivisionExpression(DivisionExpression divisionExpression)
        {
            DefaultVisit(divisionExpression);
        }

        public virtual void VisitIntegerDivisionExpression(IntegerDivisionExpression integerDivisionExpression)
        {
            DefaultVisit(integerDivisionExpression);
        }

        public virtual void VisitModuloExpression(ModuloExpression moduloExpression)
        {
            DefaultVisit(moduloExpression);
        }

        public virtual void VisitAndExpression(AndExpression andExpression)
        {
            DefaultVisit(andExpression);
        }

        public virtual void VisitExponentiationExpression(ExponentiationExpression exponentiationExpression)
        {
            DefaultVisit(exponentiationExpression);
        }

        public virtual void VisitLikeExpression(LikeExpression likeExpression)
        {
            DefaultVisit(likeExpression);
        }

        public virtual void VisitComplexEntityConstructionExpression(ComplexEntityConstructionExpression complexEntityConstructionExpression)
        {
            DefaultVisit(complexEntityConstructionExpression);
        }

        public virtual void VisitUnaryPlusExpression(UnaryPlusExpression unaryPlusExpression)
        {
            DefaultVisit(unaryPlusExpression);
        }

        public virtual void VisitUnaryMinusExpression(UnaryMinusExpression unaryMinusExpression)
        {
            DefaultVisit(unaryMinusExpression);
        }

        public virtual void VisitNotExpression(NotExpression notExpression)
        {
            DefaultVisit(notExpression);
        }

        public virtual void VisitStringLiteral(StringLiteral stringLiteral)
        {
            DefaultVisit(stringLiteral);
        }

        public virtual void VisitIntegerLiteral(IntegerLiteral integerLiteral)
        {
            DefaultVisit(integerLiteral);
        }

        public virtual void VisitRealLiteral(RealLiteral realLiteral)
        {
            DefaultVisit(realLiteral);
        }

        public virtual void VisitBinaryLiteral(BinaryLiteral binaryLiteral)
        {
            DefaultVisit(binaryLiteral);
        }

        public virtual void VisitAggregateInitializerExpression(AggregateInitializerExpression aggregateInitializerExpression)
        {
            DefaultVisit(aggregateInitializerExpression);
        }

        public virtual void VisitAggregateInitializerElement(AggregateInitializerElement aggregateInitializerElement)
        {
            DefaultVisit(aggregateInitializerElement);
        }

        public virtual void VisitQueryExpression(QueryExpression queryExpression)
        {
            DefaultVisit(queryExpression);
        }

        public virtual void VisitIntervalExpression(IntervalExpression intervalExpression)
        {
            DefaultVisit(intervalExpression);
        }

        public virtual void VisitEntityConstructorExpression(EntityConstructorExpression entityConstructorExpression)
        {
            DefaultVisit(entityConstructorExpression);
        }

        public virtual void VisitEnumerationReferenceExpression(EnumerationReferenceExpression enumerationReferenceExpression)
        {
            DefaultVisit(enumerationReferenceExpression);
        }

        public virtual void VisitConstantReferenceExpression(ConstantReferenceExpression constantReferenceExpression)
        {
            DefaultVisit(constantReferenceExpression);
        }

        public virtual void VisitParameterReferenceExpression(ParameterReferenceExpression parameterReferenceExpression)
        {
            DefaultVisit(parameterReferenceExpression);
        }

        public virtual void VisitEntityReferenceExpression(EntityReferenceExpression entityReferenceExpression)
        {
            DefaultVisit(entityReferenceExpression);
        }

        public virtual void VisitVariableReferenceExpression(VariableReferenceExpression variableReferenceExpression)
        {
            DefaultVisit(variableReferenceExpression);
        }

        public virtual void VisitFunctionCallExpression(FunctionCallExpression functionCallExpression)
        {
            DefaultVisit(functionCallExpression);
        }

        public virtual void VisitQualifiedExpression(QualifiedExpression qualifiedExpression)
        {
            DefaultVisit(qualifiedExpression);
        }

        public virtual void VisitAttributeReferenceExpression(AttributeReferenceExpression attributeReferenceExpression)
        {
            DefaultVisit(attributeReferenceExpression);
        }

        public virtual void VisitNullStatement(NullStatement nullStatement)
        {
            DefaultVisit(nullStatement);
        }

        public virtual void VisitAliasStatement(AliasStatement aliasStatement)
        {
            DefaultVisit(aliasStatement);
        }

        public virtual void VisitAssignmentStatement(AssignmentStatement assignmentStatement)
        {
            DefaultVisit(assignmentStatement);
        }

        public virtual void VisitCaseStatement(CaseStatement caseStatement)
        {
            DefaultVisit(caseStatement);
        }

        public virtual void VisitCaseAction(CaseAction caseAction)
        {
            DefaultVisit(caseAction);
        }

        public virtual void VisitCompoundStatement(CompoundStatement compoundStatement)
        {
            DefaultVisit(compoundStatement);
        }

        public virtual void VisitIfStatement(IfStatement ifStatement)
        {
            DefaultVisit(ifStatement);
        }

        public virtual void VisitProcedureCallStatement(ProcedureCallStatement procedureCallStatement)
        {
            DefaultVisit(procedureCallStatement);
        }

        public virtual void VisitRepeatStatement(RepeatStatement repeatStatement)
        {
            DefaultVisit(repeatStatement);
        }

        public virtual void VisitReturnStatement(ReturnStatement returnStatement)
        {
            DefaultVisit(returnStatement);
        }

        public virtual void VisitSkipStatement(SkipStatement skipStatement)
        {
            DefaultVisit(skipStatement);
        }

        public virtual void VisitEscapeStatement(EscapeStatement escapeStatement)
        {
            DefaultVisit(escapeStatement);
        }

        public virtual void VisitQualifiedReference(QualifiedReference qualifiedReference)
        {
            DefaultVisit(qualifiedReference);
        }

        public virtual void VisitAttributeQualifier(AttributeQualifier attributeQualifier)
        {
            DefaultVisit(attributeQualifier);
        }

        public virtual void VisitGroupQualifier(GroupQualifier groupQualifier)
        {
            DefaultVisit(groupQualifier);
        }

        public virtual void VisitIndexQualifier(IndexQualifier indexQualifier)
        {
            DefaultVisit(indexQualifier);
        }

        public virtual void VisitSchemaReference(SchemaReference schemaReference)
        {
            DefaultVisit(schemaReference);
        }

        public virtual void VisitTypeReference(TypeReference typeReference)
        {
            DefaultVisit(typeReference);
        }

        public virtual void VisitEntityReference(EntityReference entityReference)
        {
            DefaultVisit(entityReference);
        }

        public virtual void VisitAttributeReference(AttributeReference attributeReference)
        {
            DefaultVisit(attributeReference);
        }

        public virtual void VisitUnresolvedReference(UnresolvedReference unresolvedReference)
        {
            DefaultVisit(unresolvedReference);
        }

        public virtual void VisitParameterReference(ParameterReference parameterReference)
        {
            DefaultVisit(parameterReference);
        }

        public virtual void VisitVariableReference(VariableReference variableReference)
        {
            DefaultVisit(variableReference);
        }

        public virtual void VisitEnumerationReference(EnumerationReference enumerationReference)
        {
            DefaultVisit(enumerationReference);
        }

        public virtual void VisitConstantReference(ConstantReference constantReference)
        {
            DefaultVisit(constantReference);
        }

        public virtual void VisitFunctionReference(FunctionReference functionReference)
        {
            DefaultVisit(functionReference);
        }

        public virtual void VisitProcedureReference(ProcedureReference procedureReference)
        {
            DefaultVisit(procedureReference);
        }

        public virtual void VisitTypeLabelReference(TypeLabelReference typeLabelReference)
        {
            DefaultVisit(typeLabelReference);
        }

        public virtual void VisitBounds(Bounds bounds)
        {
            DefaultVisit(bounds);
        }

        public virtual void VisitAndTypeExpression(AndTypeExpression andTypeExpression)
        {
            DefaultVisit(andTypeExpression);
        }

        public virtual void VisitAndOrTypeExpression(AndOrTypeExpression andOrTypeExpression)
        {
            DefaultVisit(andOrTypeExpression);
        }

        public virtual void VisitOneOfTypeExpression(OneOfTypeExpression oneOfTypeExpression)
        {
            DefaultVisit(oneOfTypeExpression);
        }

        public virtual void VisitEntityReferenceTypeExpression(EntityReferenceTypeExpression entityReferenceTypeExpression)
        {
            DefaultVisit(entityReferenceTypeExpression);
        }

        public virtual void VisitUseClause(UseClause useClause)
        {
            DefaultVisit(useClause);
        }

        public virtual void VisitReferenceClause(ReferenceClause referenceClause)
        {
            DefaultVisit(referenceClause);
        }

        public virtual void VisitRenamedReference(RenamedReference renamedReference)
        {
            DefaultVisit(renamedReference);
        }

    }

    public partial class SyntaxNodeVisitor<TResult>
    {
        public virtual TResult VisitAggregateDataType(AggregateDataType aggregateDataType)
        {
            return DefaultVisit(aggregateDataType);
        }

        public virtual TResult VisitListDataType(ListDataType listDataType)
        {
            return DefaultVisit(listDataType);
        }

        public virtual TResult VisitArrayDataType(ArrayDataType arrayDataType)
        {
            return DefaultVisit(arrayDataType);
        }

        public virtual TResult VisitBagDataType(BagDataType bagDataType)
        {
            return DefaultVisit(bagDataType);
        }

        public virtual TResult VisitSetDataType(SetDataType setDataType)
        {
            return DefaultVisit(setDataType);
        }

        public virtual TResult VisitEnumerationDataType(EnumerationDataType enumerationDataType)
        {
            return DefaultVisit(enumerationDataType);
        }

        public virtual TResult VisitSelectDataType(SelectDataType selectDataType)
        {
            return DefaultVisit(selectDataType);
        }

        public virtual TResult VisitReferenceDataType(ReferenceDataType referenceDataType)
        {
            return DefaultVisit(referenceDataType);
        }

        public virtual TResult VisitStringDataType(StringDataType stringDataType)
        {
            return DefaultVisit(stringDataType);
        }

        public virtual TResult VisitBinaryDataType(BinaryDataType binaryDataType)
        {
            return DefaultVisit(binaryDataType);
        }

        public virtual TResult VisitRealDataType(RealDataType realDataType)
        {
            return DefaultVisit(realDataType);
        }

        public virtual TResult VisitBooleanDataType(BooleanDataType booleanDataType)
        {
            return DefaultVisit(booleanDataType);
        }

        public virtual TResult VisitLogicalDataType(LogicalDataType logicalDataType)
        {
            return DefaultVisit(logicalDataType);
        }

        public virtual TResult VisitIntegerDataType(IntegerDataType integerDataType)
        {
            return DefaultVisit(integerDataType);
        }

        public virtual TResult VisitNumberDataType(NumberDataType numberDataType)
        {
            return DefaultVisit(numberDataType);
        }

        public virtual TResult VisitGenericDataType(GenericDataType genericDataType)
        {
            return DefaultVisit(genericDataType);
        }

        public virtual TResult VisitGenericEntityDataType(GenericEntityDataType genericEntityDataType)
        {
            return DefaultVisit(genericEntityDataType);
        }

        public virtual TResult VisitSchemaDeclaration(SchemaDeclaration schemaDeclaration)
        {
            return DefaultVisit(schemaDeclaration);
        }

        public virtual TResult VisitTypeDeclaration(TypeDeclaration typeDeclaration)
        {
            return DefaultVisit(typeDeclaration);
        }

        public virtual TResult VisitExplicitAttributeDeclaration(ExplicitAttributeDeclaration explicitAttributeDeclaration)
        {
            return DefaultVisit(explicitAttributeDeclaration);
        }

        public virtual TResult VisitInverseAttributeDeclaration(InverseAttributeDeclaration inverseAttributeDeclaration)
        {
            return DefaultVisit(inverseAttributeDeclaration);
        }

        public virtual TResult VisitDerivedAttributeDeclaration(DerivedAttributeDeclaration derivedAttributeDeclaration)
        {
            return DefaultVisit(derivedAttributeDeclaration);
        }

        public virtual TResult VisitEntityDeclaration(EntityDeclaration entityDeclaration)
        {
            return DefaultVisit(entityDeclaration);
        }

        public virtual TResult VisitConstantDeclaration(ConstantDeclaration constantDeclaration)
        {
            return DefaultVisit(constantDeclaration);
        }

        public virtual TResult VisitEnumerationDeclaration(EnumerationDeclaration enumerationDeclaration)
        {
            return DefaultVisit(enumerationDeclaration);
        }

        public virtual TResult VisitProcedureDeclaration(ProcedureDeclaration procedureDeclaration)
        {
            return DefaultVisit(procedureDeclaration);
        }

        public virtual TResult VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
        {
            return DefaultVisit(functionDeclaration);
        }

        public virtual TResult VisitRuleDeclaration(RuleDeclaration ruleDeclaration)
        {
            return DefaultVisit(ruleDeclaration);
        }

        public virtual TResult VisitDomainRuleDeclaration(DomainRuleDeclaration domainRuleDeclaration)
        {
            return DefaultVisit(domainRuleDeclaration);
        }

        public virtual TResult VisitUniqueRuleDeclaration(UniqueRuleDeclaration uniqueRuleDeclaration)
        {
            return DefaultVisit(uniqueRuleDeclaration);
        }

        public virtual TResult VisitParameterDeclaration(ParameterDeclaration parameterDeclaration)
        {
            return DefaultVisit(parameterDeclaration);
        }

        public virtual TResult VisitVariableDeclaration(VariableDeclaration variableDeclaration)
        {
            return DefaultVisit(variableDeclaration);
        }

        public virtual TResult VisitSubtypeConstraintDeclaration(SubtypeConstraintDeclaration subtypeConstraintDeclaration)
        {
            return DefaultVisit(subtypeConstraintDeclaration);
        }

        public virtual TResult VisitTypeLabelDeclaration(TypeLabelDeclaration typeLabelDeclaration)
        {
            return DefaultVisit(typeLabelDeclaration);
        }

        public virtual TResult VisitLessThanExpression(LessThanExpression lessThanExpression)
        {
            return DefaultVisit(lessThanExpression);
        }

        public virtual TResult VisitGreaterThanExpression(GreaterThanExpression greaterThanExpression)
        {
            return DefaultVisit(greaterThanExpression);
        }

        public virtual TResult VisitLessThanOrEqualExpression(LessThanOrEqualExpression lessThanOrEqualExpression)
        {
            return DefaultVisit(lessThanOrEqualExpression);
        }

        public virtual TResult VisitGreaterThanOrEqualExpression(GreaterThanOrEqualExpression greaterThanOrEqualExpression)
        {
            return DefaultVisit(greaterThanOrEqualExpression);
        }

        public virtual TResult VisitNotEqualExpression(NotEqualExpression notEqualExpression)
        {
            return DefaultVisit(notEqualExpression);
        }

        public virtual TResult VisitEqualExpression(EqualExpression equalExpression)
        {
            return DefaultVisit(equalExpression);
        }

        public virtual TResult VisitInstanceNotEqualExpression(InstanceNotEqualExpression instanceNotEqualExpression)
        {
            return DefaultVisit(instanceNotEqualExpression);
        }

        public virtual TResult VisitInstanceEqualExpression(InstanceEqualExpression instanceEqualExpression)
        {
            return DefaultVisit(instanceEqualExpression);
        }

        public virtual TResult VisitInExpression(InExpression inExpression)
        {
            return DefaultVisit(inExpression);
        }

        public virtual TResult VisitAdditionExpression(AdditionExpression additionExpression)
        {
            return DefaultVisit(additionExpression);
        }

        public virtual TResult VisitSubtractionExpression(SubtractionExpression subtractionExpression)
        {
            return DefaultVisit(subtractionExpression);
        }

        public virtual TResult VisitOrExpression(OrExpression orExpression)
        {
            return DefaultVisit(orExpression);
        }

        public virtual TResult VisitXorExpression(XorExpression xorExpression)
        {
            return DefaultVisit(xorExpression);
        }

        public virtual TResult VisitMultiplicationExpression(MultiplicationExpression multiplicationExpression)
        {
            return DefaultVisit(multiplicationExpression);
        }

        public virtual TResult VisitDivisionExpression(DivisionExpression divisionExpression)
        {
            return DefaultVisit(divisionExpression);
        }

        public virtual TResult VisitIntegerDivisionExpression(IntegerDivisionExpression integerDivisionExpression)
        {
            return DefaultVisit(integerDivisionExpression);
        }

        public virtual TResult VisitModuloExpression(ModuloExpression moduloExpression)
        {
            return DefaultVisit(moduloExpression);
        }

        public virtual TResult VisitAndExpression(AndExpression andExpression)
        {
            return DefaultVisit(andExpression);
        }

        public virtual TResult VisitExponentiationExpression(ExponentiationExpression exponentiationExpression)
        {
            return DefaultVisit(exponentiationExpression);
        }

        public virtual TResult VisitLikeExpression(LikeExpression likeExpression)
        {
            return DefaultVisit(likeExpression);
        }

        public virtual TResult VisitComplexEntityConstructionExpression(ComplexEntityConstructionExpression complexEntityConstructionExpression)
        {
            return DefaultVisit(complexEntityConstructionExpression);
        }

        public virtual TResult VisitUnaryPlusExpression(UnaryPlusExpression unaryPlusExpression)
        {
            return DefaultVisit(unaryPlusExpression);
        }

        public virtual TResult VisitUnaryMinusExpression(UnaryMinusExpression unaryMinusExpression)
        {
            return DefaultVisit(unaryMinusExpression);
        }

        public virtual TResult VisitNotExpression(NotExpression notExpression)
        {
            return DefaultVisit(notExpression);
        }

        public virtual TResult VisitStringLiteral(StringLiteral stringLiteral)
        {
            return DefaultVisit(stringLiteral);
        }

        public virtual TResult VisitIntegerLiteral(IntegerLiteral integerLiteral)
        {
            return DefaultVisit(integerLiteral);
        }

        public virtual TResult VisitRealLiteral(RealLiteral realLiteral)
        {
            return DefaultVisit(realLiteral);
        }

        public virtual TResult VisitBinaryLiteral(BinaryLiteral binaryLiteral)
        {
            return DefaultVisit(binaryLiteral);
        }

        public virtual TResult VisitAggregateInitializerExpression(AggregateInitializerExpression aggregateInitializerExpression)
        {
            return DefaultVisit(aggregateInitializerExpression);
        }

        public virtual TResult VisitAggregateInitializerElement(AggregateInitializerElement aggregateInitializerElement)
        {
            return DefaultVisit(aggregateInitializerElement);
        }

        public virtual TResult VisitQueryExpression(QueryExpression queryExpression)
        {
            return DefaultVisit(queryExpression);
        }

        public virtual TResult VisitIntervalExpression(IntervalExpression intervalExpression)
        {
            return DefaultVisit(intervalExpression);
        }

        public virtual TResult VisitEntityConstructorExpression(EntityConstructorExpression entityConstructorExpression)
        {
            return DefaultVisit(entityConstructorExpression);
        }

        public virtual TResult VisitEnumerationReferenceExpression(EnumerationReferenceExpression enumerationReferenceExpression)
        {
            return DefaultVisit(enumerationReferenceExpression);
        }

        public virtual TResult VisitConstantReferenceExpression(ConstantReferenceExpression constantReferenceExpression)
        {
            return DefaultVisit(constantReferenceExpression);
        }

        public virtual TResult VisitParameterReferenceExpression(ParameterReferenceExpression parameterReferenceExpression)
        {
            return DefaultVisit(parameterReferenceExpression);
        }

        public virtual TResult VisitEntityReferenceExpression(EntityReferenceExpression entityReferenceExpression)
        {
            return DefaultVisit(entityReferenceExpression);
        }

        public virtual TResult VisitVariableReferenceExpression(VariableReferenceExpression variableReferenceExpression)
        {
            return DefaultVisit(variableReferenceExpression);
        }

        public virtual TResult VisitFunctionCallExpression(FunctionCallExpression functionCallExpression)
        {
            return DefaultVisit(functionCallExpression);
        }

        public virtual TResult VisitQualifiedExpression(QualifiedExpression qualifiedExpression)
        {
            return DefaultVisit(qualifiedExpression);
        }

        public virtual TResult VisitAttributeReferenceExpression(AttributeReferenceExpression attributeReferenceExpression)
        {
            return DefaultVisit(attributeReferenceExpression);
        }

        public virtual TResult VisitNullStatement(NullStatement nullStatement)
        {
            return DefaultVisit(nullStatement);
        }

        public virtual TResult VisitAliasStatement(AliasStatement aliasStatement)
        {
            return DefaultVisit(aliasStatement);
        }

        public virtual TResult VisitAssignmentStatement(AssignmentStatement assignmentStatement)
        {
            return DefaultVisit(assignmentStatement);
        }

        public virtual TResult VisitCaseStatement(CaseStatement caseStatement)
        {
            return DefaultVisit(caseStatement);
        }

        public virtual TResult VisitCaseAction(CaseAction caseAction)
        {
            return DefaultVisit(caseAction);
        }

        public virtual TResult VisitCompoundStatement(CompoundStatement compoundStatement)
        {
            return DefaultVisit(compoundStatement);
        }

        public virtual TResult VisitIfStatement(IfStatement ifStatement)
        {
            return DefaultVisit(ifStatement);
        }

        public virtual TResult VisitProcedureCallStatement(ProcedureCallStatement procedureCallStatement)
        {
            return DefaultVisit(procedureCallStatement);
        }

        public virtual TResult VisitRepeatStatement(RepeatStatement repeatStatement)
        {
            return DefaultVisit(repeatStatement);
        }

        public virtual TResult VisitReturnStatement(ReturnStatement returnStatement)
        {
            return DefaultVisit(returnStatement);
        }

        public virtual TResult VisitSkipStatement(SkipStatement skipStatement)
        {
            return DefaultVisit(skipStatement);
        }

        public virtual TResult VisitEscapeStatement(EscapeStatement escapeStatement)
        {
            return DefaultVisit(escapeStatement);
        }

        public virtual TResult VisitQualifiedReference(QualifiedReference qualifiedReference)
        {
            return DefaultVisit(qualifiedReference);
        }

        public virtual TResult VisitAttributeQualifier(AttributeQualifier attributeQualifier)
        {
            return DefaultVisit(attributeQualifier);
        }

        public virtual TResult VisitGroupQualifier(GroupQualifier groupQualifier)
        {
            return DefaultVisit(groupQualifier);
        }

        public virtual TResult VisitIndexQualifier(IndexQualifier indexQualifier)
        {
            return DefaultVisit(indexQualifier);
        }

        public virtual TResult VisitSchemaReference(SchemaReference schemaReference)
        {
            return DefaultVisit(schemaReference);
        }

        public virtual TResult VisitTypeReference(TypeReference typeReference)
        {
            return DefaultVisit(typeReference);
        }

        public virtual TResult VisitEntityReference(EntityReference entityReference)
        {
            return DefaultVisit(entityReference);
        }

        public virtual TResult VisitAttributeReference(AttributeReference attributeReference)
        {
            return DefaultVisit(attributeReference);
        }

        public virtual TResult VisitUnresolvedReference(UnresolvedReference unresolvedReference)
        {
            return DefaultVisit(unresolvedReference);
        }

        public virtual TResult VisitParameterReference(ParameterReference parameterReference)
        {
            return DefaultVisit(parameterReference);
        }

        public virtual TResult VisitVariableReference(VariableReference variableReference)
        {
            return DefaultVisit(variableReference);
        }

        public virtual TResult VisitEnumerationReference(EnumerationReference enumerationReference)
        {
            return DefaultVisit(enumerationReference);
        }

        public virtual TResult VisitConstantReference(ConstantReference constantReference)
        {
            return DefaultVisit(constantReference);
        }

        public virtual TResult VisitFunctionReference(FunctionReference functionReference)
        {
            return DefaultVisit(functionReference);
        }

        public virtual TResult VisitProcedureReference(ProcedureReference procedureReference)
        {
            return DefaultVisit(procedureReference);
        }

        public virtual TResult VisitTypeLabelReference(TypeLabelReference typeLabelReference)
        {
            return DefaultVisit(typeLabelReference);
        }

        public virtual TResult VisitBounds(Bounds bounds)
        {
            return DefaultVisit(bounds);
        }

        public virtual TResult VisitAndTypeExpression(AndTypeExpression andTypeExpression)
        {
            return DefaultVisit(andTypeExpression);
        }

        public virtual TResult VisitAndOrTypeExpression(AndOrTypeExpression andOrTypeExpression)
        {
            return DefaultVisit(andOrTypeExpression);
        }

        public virtual TResult VisitOneOfTypeExpression(OneOfTypeExpression oneOfTypeExpression)
        {
            return DefaultVisit(oneOfTypeExpression);
        }

        public virtual TResult VisitEntityReferenceTypeExpression(EntityReferenceTypeExpression entityReferenceTypeExpression)
        {
            return DefaultVisit(entityReferenceTypeExpression);
        }

        public virtual TResult VisitUseClause(UseClause useClause)
        {
            return DefaultVisit(useClause);
        }

        public virtual TResult VisitReferenceClause(ReferenceClause referenceClause)
        {
            return DefaultVisit(referenceClause);
        }

        public virtual TResult VisitRenamedReference(RenamedReference renamedReference)
        {
            return DefaultVisit(renamedReference);
        }

    }

}

