using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace SharpExpress.Parsing
{
    class Scope
    {
        private Scope(ImmutableDictionary<string, IDeclaration> identifiers,
                      ImmutableDictionary<string, IDeclaration> dataTypes)
        {
            DeclaredIdentifiers = identifiers;
            DeclaredDataTypes = dataTypes;
        }

        public ImmutableDictionary<string, IDeclaration> DeclaredIdentifiers { get; }
        public ImmutableDictionary<string, IDeclaration> DeclaredDataTypes { get; }

        public static Scope CreateRootScope(IEnumerable<ISchemaDeclaration> schemaDeclarations)
        {
            var identifiers = ImmutableDictionary.CreateBuilder<string, IDeclaration>(StringComparer.OrdinalIgnoreCase);

            foreach (var schema in schemaDeclarations)
            {
                identifiers.Add(schema.Name, schema);
            }

            return new Scope(identifiers.ToImmutable(),
                ImmutableDictionary<string, IDeclaration>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase));
        }


        private bool IsNamedDataType(IDeclaration declaration)
        {
            switch (declaration)
            {
                case ITypeDeclaration _:
                case IEntityDeclaration _:
                case ITypeLabelDeclaration _:
                    return true;

                default:
                    return false;
            }
        }

        private bool TryGetDeclaredEnumerationItems(ITypeDeclaration typeDeclaration, out IEnumerable<IEnumerationDeclaration> enumerations)
        {
            var result = new List<IEnumerationDeclaration>();

            var underlyingType = typeDeclaration.UnderlyingType;

            while (underlyingType != null)
            {
                switch (underlyingType)
                {
                    case IEnumerationDataType enumerationType:
                        //TODO: duplicates in extended enumerations
                        result.AddRange(enumerationType.Items);

                        if (enumerationType.BasedOn != null)
                        {
                            underlyingType = ResolveType(enumerationType.BasedOn.TypeName).UnderlyingType;
                        }
                        else
                        {
                            underlyingType = null;
                        }
                        break;

                    case IReferenceDataType referenceType when referenceType.Reference is ITypeReference typeRef:
                        underlyingType = ResolveType(typeRef.TypeName)?.UnderlyingType;
                        break;
                    case IReferenceDataType referenceType when referenceType.Reference is IUnresolvedReference unresolvedRef:
                        underlyingType = ResolveType(unresolvedRef.UnresolvedName)?.UnderlyingType;
                        break;

                    default:
                        underlyingType = null;
                        break;
                }
            }

            if (result.Count > 0)
            {
                enumerations = result;
                return true;
            }
            else
            {
                enumerations = null;
                return false;
            }
        }

        public Scope CreateChildScope(ISchemaDeclaration schema)
        {
            var identifers = DeclaredIdentifiers.ToBuilder();
            var dataTypes = DeclaredDataTypes.ToBuilder();

            var importedEnumerationItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            //TODO: duplicate identifiers
            foreach (var declaration in schema.Declarations)
            {
                identifers[declaration.Name] = declaration;

                if (IsNamedDataType(declaration))
                    dataTypes[declaration.Name] = declaration;

                if (declaration is ITypeDeclaration typeDeclaration)
                {
                    if (TryGetDeclaredEnumerationItems(typeDeclaration, out var enumerationDeclarations))
                    {
                        foreach (var enumerationDeclaration in enumerationDeclarations)
                        {
                            if (importedEnumerationItems.Add(enumerationDeclaration.Name))
                            {
                                identifers[enumerationDeclaration.Name] = enumerationDeclaration;
                            }
                            else
                            {
                                identifers.Remove(enumerationDeclaration.Name);
                            }
                        }
                    }
                }
            }

            return new Scope(identifers.ToImmutable(), dataTypes.ToImmutable());
        }

        public Scope CreateChildScope(ISubtypeConstraintDeclaration subtypeConstraintDeclaration)
        {
            throw new NotImplementedException();
        }

        public Scope CreateChildScope(IAliasStatement aliasStatement)
        {
            throw new NotImplementedException();
        }

        public Scope CreateChildScope(ITypeDeclaration type)
        {
            var builder = DeclaredIdentifiers.ToBuilder();

            if (TryGetDeclaredEnumerationItems(type, out var enumerationDeclarations))
            {
                foreach (var item in enumerationDeclarations)
                {
                    builder[item.Name] = item;
                }
            }

            //TODO: Constant SELF

            return new Scope(builder.ToImmutable(), DeclaredDataTypes);
        }

        public Scope CreateChildScope(IFunctionDeclaration function)
        {
            var builder = DeclaredIdentifiers.ToBuilder();

            //TODO: declarations
            //TODO: type labels

            foreach (var parameter in function.Parameters)
            {
                builder[parameter.Name] = parameter;
            }

            foreach (var variable in function.LocalVariables)
            {
                builder[variable.Name] = variable;
            }

            return new Scope(builder.ToImmutable(), DeclaredDataTypes);
        }

        public Scope CreateChildScope(IProcedureDeclaration procedure)
        {
            throw new NotImplementedException();
        }

        public Scope CreateChildScope(IRuleDeclaration rule)
        {
            var builder = DeclaredIdentifiers.ToBuilder();
            //TODO: declarations
            //TODO: type labels (?)

            foreach (var population in rule.Populations)
            {
                builder[population.Name] = population;
            }

            foreach (var variable in rule.LocalVariables)
            {
                builder[variable.Name] = variable;
            }

            return new Scope(builder.ToImmutable(), DeclaredDataTypes);
        }

        public Scope CreateChildScope(IQueryExpression query)
        {
            return new Scope(
                DeclaredIdentifiers.SetItem(query.VariableDeclaration.Name, query.VariableDeclaration),
                DeclaredDataTypes);
        }

        public Scope CreateChildScope(IRepeatStatement repeat)
        {
            if (repeat.IncrementVariable == null)
                return this;

            return new Scope(
                DeclaredIdentifiers.SetItem(repeat.IncrementVariable.Name, repeat.IncrementVariable),
                DeclaredDataTypes);
        }

        public Scope CreateChildScope(IEntityDeclaration entity)
        {
            //TODO: Constant SELF
            var builder = DeclaredIdentifiers.ToBuilder();

            //TODO: renaming
            foreach (var attribute in GetAttributesWithInherited(entity))
            {
                builder[attribute.Name] = attribute;
            }


            return new Scope(builder.ToImmutable(), DeclaredDataTypes);
        }

        private IEnumerable<IAttributeDeclaration> GetAttributes(IEntityDeclaration entityDeclaration)
        {
            foreach (var attribute in entityDeclaration.ExplicitAttributes)
                yield return attribute;

            foreach (var attribute in entityDeclaration.InverseAttributes)
                yield return attribute;

            foreach (var attribute in entityDeclaration.DerivedAttributes)
                yield return attribute;
        }


        private IEnumerable<IAttributeDeclaration> GetAttributesWithInherited(IEntityDeclaration entity)
        {
            var result = new Dictionary<string, IAttributeDeclaration>(StringComparer.OrdinalIgnoreCase);
            var visited = new HashSet<IEntityDeclaration>();

            //TODO: duplicate attributes in inheritance hierarchy have to be disambiguated, so they cannot be added
            // twice here

            void visit(IEntityDeclaration entityDeclaration)
            {
                foreach (var attribute in GetAttributes(entityDeclaration))
                {
                    if (result.TryGetValue(attribute.Name, out var existing) && existing != attribute)
                    {
                        result[attribute.Name] = null;
                    }
                    else
                    {
                        result[attribute.Name] = attribute;
                    }
                }

                foreach (var supertypeRef in entityDeclaration.Supertypes)
                {
                    var supertype = ResolveEntity(supertypeRef.EntityName);

                    if (visited.Add(supertype))
                    {
                        visit(supertype);
                    }
                }
            }

            visit(entity);

            return result.Values.Where(a => a != null);
        }

        private IEnumerable<IEntityDeclaration> GetSupertypes(IEntityDeclaration entityDeclaration)
        {
            var result = new HashSet<IEntityDeclaration>();
            void visit(IEntityDeclaration entity)
            {
                foreach (var supertypeRef in entity.Supertypes)
                {
                    var supertype = ResolveEntity(supertypeRef.EntityName);

                    if (supertype == null)
                        throw new NotImplementedException();

                    if (result.Add(supertype))
                    {
                        visit(supertype);
                    }
                }
            }

            visit(entityDeclaration);

            return result;
        }

        public ITypeDeclaration ResolveType(string name)
        {
            if (DeclaredDataTypes.TryGetValue(name, out var declaration) &&
                declaration is ITypeDeclaration typeDeclaration)
            {
                return typeDeclaration;
            }

            return null;
        }

        public IEntityDeclaration ResolveEntity(string name)
        {
            if (DeclaredDataTypes.TryGetValue(name, out var declaration) &&
                declaration is IEntityDeclaration entityDeclaration)
            {
                return entityDeclaration;
            }

            return null;
        }

        public IEnumerationDeclaration ResolveEnumeration(string name)
        {
            if (DeclaredIdentifiers.TryGetValue(name, out var declaration) &&
                declaration is IEnumerationDeclaration enumerationDeclaration)
            {
                return enumerationDeclaration;
            }

            return null;
        }

        public IAttributeDeclaration ResolveAttribute(string name)
        {
            if (DeclaredIdentifiers.TryGetValue(name, out var declaration) &&
                 declaration is IAttributeDeclaration attributeDeclaration)
            {
                return attributeDeclaration;
            }

            return null;
        }

        public IVariableDeclaration ResolveVariable(string name)
        {
            if (DeclaredIdentifiers.TryGetValue(name, out var declaration) &&
                declaration is IVariableDeclaration variableDeclaration)
            {
                return variableDeclaration;
            }

            return null;
        }

        public IConstantDeclaration ResolveConstant(string name)
        {
            if (DeclaredIdentifiers.TryGetValue(name, out var declaration) &&
                declaration is IConstantDeclaration constantDeclaration)
            {
                return constantDeclaration;
            }

            return null;
        }

        public IFunctionDeclaration ResolveFunction(string name)
        {
            if (DeclaredIdentifiers.TryGetValue(name, out var declaration) &&
                declaration is IFunctionDeclaration functionDeclaration)
            {
                return functionDeclaration;
            }

            return null;
        }

        public IProcedureDeclaration ResolveProcedure(string name)
        {
            if (DeclaredIdentifiers.TryGetValue(name, out var declaration) &&
               declaration is IProcedureDeclaration procedureDeclaration)
            {
                return procedureDeclaration;
            }

            return null;
        }

        public IParameterDeclaration ResolveParameter(string name)
        {
            if (DeclaredIdentifiers.TryGetValue(name, out var declaration) &&
                declaration is IParameterDeclaration parameterDeclaration)
            {
                return parameterDeclaration;
            }

            return null;
        }

    }
}
