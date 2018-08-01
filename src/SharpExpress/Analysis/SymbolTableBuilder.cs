using SharpExpress.Analysis;
using SharpExpress.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.TypeSystem
{
    class SymbolTableBuilder : SyntaxNodeWalker
    {
        private Dictionary<Declaration, ISymbolInfo> symbols_ = new Dictionary<Declaration, ISymbolInfo>();
        private IEnumerable<SchemaDeclaration> schemas_;
        private NumericExpressionEvaluator evaluator_ = new NumericExpressionEvaluator();

        private Stack<Scope> scopes_;
        private Stack<IDeclaringSymbolInfo> declaringSymbols_;

        private Dictionary<Declaration, List<Action<ISymbolInfo>>> markedForResolution_ = new Dictionary<Declaration, List<Action<ISymbolInfo>>>();

        private Scope CurrentScope => scopes_.Peek();

        private IDeclaringSymbolInfo CurrentDeclaringSymbol => declaringSymbols_.Peek();

        public SymbolTableBuilder(IEnumerable<SchemaDeclaration> schemas)
        {
            schemas_ = schemas;
        }

        public override void VisitSchemaDeclaration(SchemaDeclaration schemaDeclaration)
        {
            PushScope(CurrentScope.CreateChildScope(schemaDeclaration));

            var schemaInfo = CreateSchemaInfo(schemaDeclaration);

            PushDeclaringSymbol(schemaInfo);

            base.VisitSchemaDeclaration(schemaDeclaration);

            PopDeclaringSymbol();
            PopScope();
        }

        public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
        {
            PushScope(CurrentScope.CreateChildScope(typeDeclaration));

            CreateDefinedTypeInfo(typeDeclaration);

            base.VisitTypeDeclaration(typeDeclaration);

            PopScope();
        }

        public override void VisitEntityDeclaration(EntityDeclaration entityDeclaration)
        {
            PushScope(CurrentScope.CreateChildScope(entityDeclaration));

            var entityInfo = CreateEntityInfo(entityDeclaration);


            PushDeclaringSymbol(entityInfo);

            base.VisitEntityDeclaration(entityDeclaration);

            PopDeclaringSymbol();
            PopScope();
        }

        public override void VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
        {
            PushScope(CurrentScope.CreateChildScope(functionDeclaration));

            var functionInfo = CreateFunctionInfo(functionDeclaration);

            PushDeclaringSymbol(functionInfo);

            base.VisitFunctionDeclaration(functionDeclaration);

            PopDeclaringSymbol();
            PopScope();
        }

        public override void VisitProcedureDeclaration(ProcedureDeclaration procedureDeclaration)
        {
            PushScope(CurrentScope.CreateChildScope(procedureDeclaration));

            var procedureInfo = CreateProcedureInfo(procedureDeclaration);

            PushDeclaringSymbol(procedureInfo);

            base.VisitProcedureDeclaration(procedureDeclaration);

            PopDeclaringSymbol();
            PopScope();
        }

        private ProcedureInfo CreateProcedureInfo(ProcedureDeclaration procedureDeclaration)
        {
            var procedureInfo = new ProcedureInfo(procedureDeclaration.Name, CurrentDeclaringSymbol);

            CurrentDeclaringSymbol.AddDeclaration(procedureInfo);
            symbols_.Add(procedureDeclaration, procedureInfo);

            return procedureInfo;
        }

        public override void VisitParameterDeclaration(ParameterDeclaration parameterDeclaration)
        {
            CreateParameterInfo(parameterDeclaration);

            base.VisitParameterDeclaration(parameterDeclaration);
        }

        public override void VisitVariableDeclaration(VariableDeclaration variableDeclaration)
        {
            CreateVariableInfo(variableDeclaration);

            base.VisitVariableDeclaration(variableDeclaration);
        }

        private VariableInfo CreateVariableInfo(VariableDeclaration variableDeclaration)
        {
            var variableInfo = new VariableInfo(variableDeclaration.Name, CurrentDeclaringSymbol);

            //don't add variable to parent
            //CurrentDeclaringSymbol.AddDeclaration(variableInfo);

            symbols_.Add(variableDeclaration, variableInfo);

            return variableInfo;
        }

        private ParameterInfo CreateParameterInfo(ParameterDeclaration parameterDeclaration)
        {
            var parameterInfo = new ParameterInfo(parameterDeclaration.Name, CurrentDeclaringSymbol);

            CurrentDeclaringSymbol.AddDeclaration(parameterInfo);
            symbols_.Add(parameterDeclaration, parameterInfo);

            SetTypeInfoDelayed(parameterDeclaration.Type, info => parameterInfo.Type = info);

            return parameterInfo;
        }

        public override void VisitExplicitAttributeDeclaration(ExplicitAttributeDeclaration explicitAttributeDeclaration)
        {
            var attributeInfo = CreateAttributeInfo(explicitAttributeDeclaration);

            base.VisitExplicitAttributeDeclaration(explicitAttributeDeclaration);
        }

        public override void VisitInverseAttributeDeclaration(InverseAttributeDeclaration inverseAttributeDeclaration)
        {
            var attributeInfo = CreateAttributeInfo(inverseAttributeDeclaration);

            base.VisitInverseAttributeDeclaration(inverseAttributeDeclaration);
        }

        public override void VisitDerivedAttributeDeclaration(DerivedAttributeDeclaration derivedAttributeDeclaration)
        {
            var attributeInfo = CreateAttributeInfo(derivedAttributeDeclaration);

            base.VisitDerivedAttributeDeclaration(derivedAttributeDeclaration);
        }

        private AttributeInfo CreateAttributeInfo(DerivedAttributeDeclaration derivedAttributeDeclaration)
        {
            var attributeInfo = new AttributeInfo(derivedAttributeDeclaration.Name, CurrentDeclaringSymbol);
            CurrentDeclaringSymbol.AddDeclaration(attributeInfo);

            symbols_.Add(derivedAttributeDeclaration, attributeInfo);

            SetTypeInfoDelayed(derivedAttributeDeclaration.Type, info => attributeInfo.Type = info);

            return attributeInfo;

        }

        private AttributeInfo CreateAttributeInfo(InverseAttributeDeclaration inverseAttributeDeclaration)
        {
            var attributeInfo = new AttributeInfo(inverseAttributeDeclaration.Name, CurrentDeclaringSymbol);
            CurrentDeclaringSymbol.AddDeclaration(attributeInfo);

            symbols_.Add(inverseAttributeDeclaration, attributeInfo);

            SetTypeInfoDelayed(inverseAttributeDeclaration.Type, info => attributeInfo.Type = info);

            return attributeInfo;
        }

        private AttributeInfo CreateAttributeInfo(ExplicitAttributeDeclaration explicitAttributeDeclaration)
        {
            var attributeInfo = new AttributeInfo(explicitAttributeDeclaration.Name, CurrentDeclaringSymbol);
            CurrentDeclaringSymbol.AddDeclaration(attributeInfo);

            symbols_.Add(explicitAttributeDeclaration, attributeInfo);

            SetTypeInfoDelayed(explicitAttributeDeclaration.Type, info => attributeInfo.Type = info);

            return attributeInfo;
        }

        public override void VisitRuleDeclaration(RuleDeclaration ruleDeclaration)
        {
            PushScope(CurrentScope.CreateChildScope(ruleDeclaration));

            var ruleInfo = CreateRuleInfo(ruleDeclaration);

            PushDeclaringSymbol(ruleInfo);

            base.VisitRuleDeclaration(ruleDeclaration);

            PopDeclaringSymbol();
            PopScope();
        }

        public SymbolTable Build()
        {
            scopes_ = new Stack<Scope>();
            declaringSymbols_ = new Stack<IDeclaringSymbolInfo>();
            scopes_.Push(Scope.CreateRootScope(schemas_));

            foreach (var schema in schemas_)
            {
                schema.Accept(this);
            }

            Debug.Assert(scopes_.Count == 1);
            Debug.Assert(declaringSymbols_.Count == 0);

            ResolveMarked();

            return new SymbolTable(symbols_);
        }

        private void ResolveMarked()
        {
            foreach (var item in markedForResolution_)
            {
                var resolvedSymbol = symbols_[item.Key];
                foreach (var callback in item.Value)
                {
                    callback(resolvedSymbol);
                }
            }
        }

        private void MarkForResolution(Declaration declaration, Action<ISymbolInfo> callback)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            if (!markedForResolution_.TryGetValue(declaration, out var callbacks))
                markedForResolution_.Add(declaration, callbacks = new List<Action<ISymbolInfo>>());

            callbacks.Add(callback);
        }

        private RuleInfo CreateRuleInfo(RuleDeclaration ruleDeclaration)
        {
            var ruleInfo = new RuleInfo(ruleDeclaration.Name, CurrentDeclaringSymbol);
            CurrentDeclaringSymbol.AddDeclaration(ruleInfo);
            symbols_.Add(ruleDeclaration, ruleInfo);

            return ruleInfo;
        }

        private SchemaInfo CreateSchemaInfo(SchemaDeclaration schemaDeclaration)
        {
            var schemaInfo = new SchemaInfo(schemaDeclaration.Name);

            symbols_.Add(schemaDeclaration, schemaInfo);

            return schemaInfo;
        }

        private FunctionInfo CreateFunctionInfo(FunctionDeclaration functionDeclaration)
        {
            var functionInfo = new FunctionInfo(functionDeclaration.Name, CurrentDeclaringSymbol);
            CurrentDeclaringSymbol.AddDeclaration(functionInfo);

            symbols_.Add(functionDeclaration, functionInfo);

            SetTypeInfoDelayed(functionDeclaration.ReturnType, info => functionInfo.ReturnType = info);

            return functionInfo;
        }

        private void SetTypeInfoDelayed(DataType dataType, Action<TypeInfo> setter)
        {
            if (dataType == null)
                throw new NotImplementedException();

            switch (dataType)
            {
                case BooleanDataType _:
                    setter(TypeInfo.Boolean);
                    break;
                case LogicalDataType _:
                    setter(TypeInfo.Logical);
                    break;
                case RealDataType realType:
                    setter(CreateRealType(realType));
                    break;
                case ArrayDataType arrayType:
                    setter(CreateArrayType(arrayType));
                    break;
                case ListDataType listType:
                    setter(CreateListType(listType));
                    break;
                case SetDataType setType:
                    setter(CreateSetType(setType));
                    break;
                case BagDataType bagType:
                    setter(CreateBagType(bagType));
                    break;
                case BinaryDataType binaryType:
                    setter(CreateBinaryType(binaryType));
                    break;
                case IntegerDataType integerType:
                    setter(TypeInfo.Integer);
                    break;
                case NumberDataType _:
                    setter(TypeInfo.Number);
                    break;
                case StringDataType stringType:
                    setter(CreateStringType(stringType));
                    break;
                case ReferenceDataType referenceType when referenceType.Reference is TypeReference typeReference:
                    SetTypeInfoDelayed(typeReference, setter);
                    break;
                case ReferenceDataType referenceType when referenceType.Reference is EntityReference entityReference:
                    SetTypeInfoDelayed(entityReference, setter);
                    break;
                case EnumerationDataType enumerationType:
                    setter(CreateEnumerationType(enumerationType));
                    break;
                case SelectDataType selectType:
                    setter(CreateSelectType(selectType));
                    break;
                case GenericDataType genericType:
                    setter(new GenericTypeInfo());
                    break;
                default:
                    throw new NotSupportedException($"UnderlyingType {dataType.Kind} not supported");
            }

        }

        private DefinedTypeInfo CreateDefinedTypeInfo(TypeDeclaration typeDeclaration)
        {
            var typeInfo = new DefinedTypeInfo(typeDeclaration.Name, CurrentDeclaringSymbol);

            CurrentDeclaringSymbol.AddDeclaration(typeInfo);
            symbols_.Add(typeDeclaration, typeInfo);

            SetTypeInfoDelayed(typeDeclaration.UnderlyingType, typeInfo.SetUnderlyingType);

            return typeInfo;
        }

        private SelectTypeInfo CreateSelectType(SelectDataType selectType)
        {
            var typeInfo = new SelectTypeInfo(CurrentDeclaringSymbol);

            if (selectType.BasedOn != null)
                throw new NotImplementedException();

            if (selectType.IsExtensible)
                throw new NotImplementedException();

            if (selectType.IsGenericIdentity)
                throw new NotImplementedException();

            typeInfo.SetSelectTypesCount(selectType.Items.Count);

            for (int i = 0; i < selectType.Items.Count; i++)
            {
                //capture by-value
                int index = i;

                var reference = selectType.Items[i];


                if (reference.Reference is EntityReference entityRef)
                {
                    SetTypeInfoDelayed(entityRef, info => typeInfo.AddSelectType(index, info));
                }
                else if (reference.Reference is TypeReference typeRef)
                {
                    SetTypeInfoDelayed(typeRef, info => typeInfo.AddSelectType(index, info));
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return typeInfo;
        }

        private TypeInfo CreateEnumerationType(EnumerationDataType enumerationType)
        {
            //here enumeration has the same name as it's defining type (there will be duplication)
            //not sure this is the right approach. Doesn't it get messy with extensions?
            var typeInfo = new EnumerationTypeInfo(CurrentDeclaringSymbol);

            if (enumerationType.BasedOn != null)
                throw new NotImplementedException();

            if (enumerationType.IsExtensible)
                throw new NotImplementedException();

            foreach (var item in enumerationType.Items)
            {
                var info = new EnumerationInfo(item.Name, typeInfo);

                symbols_.Add(item, info);

                typeInfo.AddEnumeration(info);
            }

            return typeInfo;
        }

        private void SetTypeInfoDelayed(EntityReference entityReference, Action<TypeInfo> setter)
        {
            var entityType = CurrentScope.ResolveEntity(entityReference.EntityName) as EntityDeclaration;

            if (entityType != null)
            {
                MarkForResolution(entityType, info => setter((TypeInfo)info));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void SetTypeInfoDelayed(TypeReference typeReference, Action<TypeInfo> setter)
        {
            var resolvedType = CurrentScope.ResolveType(typeReference.TypeName) as TypeDeclaration;

            if (resolvedType != null)
            {
                MarkForResolution(resolvedType, info => setter((TypeInfo)info));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private TypeInfo CreateStringType(StringDataType stringType)
        {
            if (stringType.Width != null)
            {
                var width = evaluator_.EvaluateNonIndeterminate(stringType.Width);
                return new StringTypeInfo(width, stringType.IsFixed);
            }
            else
            {
                return TypeInfo.String;
            }
        }

        private TypeInfo CreateBinaryType(BinaryDataType binaryType)
        {
            if (binaryType.Width != null)
            {
                var width = evaluator_.EvaluateNonIndeterminate(binaryType.Width);
                return new BinaryTypeInfo(width, binaryType.IsFixed);
            }
            else
            {
                return TypeInfo.Binary;
            }
        }


        private TypeInfo CreateRealType(RealDataType realType)
        {
            if (realType.Precision != null)
            {
                var precision = evaluator_.EvaluateNonIndeterminate(realType.Precision);
                return new RealTypeInfo(precision);
            }
            else
            {
                return TypeInfo.Real;
            }
        }

        private TypeInfo CreateArrayType(ArrayDataType arrayType)
        {
            var result = new ArrayTypeInfo(arrayType.IsUnique, arrayType.IsOptional);

            SetTypeInfoDelayed(arrayType.ElementType, result.SetElementType);

            return result;
        }

        private TypeInfo CreateListType(ListDataType listType)
        {
            var result = new ListTypeInfo(listType.IsUnique);

            SetTypeInfoDelayed(listType.ElementType, result.SetElementType);

            return result;
        }

        private TypeInfo CreateBagType(BagDataType bagType)
        {
            var result = new BagTypeInfo();

            SetTypeInfoDelayed(bagType.ElementType, result.SetElementType);

            return result;
        }

        private TypeInfo CreateSetType(SetDataType setType)
        {
            var result = new SetTypeInfo();

            SetTypeInfoDelayed(setType.ElementType, result.SetElementType);

            return result;
        }

        private EntityInfo CreateEntityInfo(EntityDeclaration entityDeclaration)
        {
            var entityInfo = new EntityInfo(entityDeclaration.Name, CurrentDeclaringSymbol);
            CurrentDeclaringSymbol.AddDeclaration(entityInfo);

            SetEntityBaseTypes(entityDeclaration, entityInfo);

            symbols_.Add(entityDeclaration, entityInfo);

            return entityInfo;
        }

        private void SetEntityBaseTypes(EntityDeclaration entityDeclaration, EntityInfo entityInfo)
        {
            if (entityDeclaration.Subtype != null)
            {

                entityInfo.SetBaseTypeCount(entityDeclaration.Supertypes.Count);

                for (int i = 0; i < entityDeclaration.Supertypes.Count; i++)
                {
                    int index = i;
                    SetTypeInfoDelayed(entityDeclaration.Supertypes[i], info => entityInfo.SetBaseType(index, info));
                }
            }
        }

        //private EntityReference[] GetEntityReferences(TypeExpression typeExpression)
        //{
        //    var result = new List<EntityReference>();

        //    void getRefs(TypeExpression expr)
        //    {
        //        switch (expr)
        //        {
        //            case EntityReferenceTypeExpression entityRef:
        //                result.Add(entityRef.Entity);
        //                break;

        //            case OneOfTypeExpression oneOf:
        //                foreach (var child in oneOf.Expressions)
        //                    getRefs(child);
        //                break;

        //            default:
        //                throw new NotSupportedException();
        //        }
        //    }

        //    getRefs(typeExpression);

        //    return result.ToArray();
        //}

        private void PushScope(Scope scope)
        {
            scopes_.Push(scope);
        }

        private void PopScope()
        {
            scopes_.Pop();
        }

        private void PushDeclaringSymbol(IDeclaringSymbolInfo declaringSymbol)
        {
            declaringSymbols_.Push(declaringSymbol);
        }

        private void PopDeclaringSymbol()
        {
            declaringSymbols_.Pop();
        }

    }
}
