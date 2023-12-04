using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using VContainer.SourceGenerator.CodeBuilder;

namespace VContainer.SourceGenerator
{
    [Generator(LanguageNames.CSharp)]
    public class VContainerSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxCollector());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var moduleName = context.Compilation.SourceModule.Name;
            if (moduleName.StartsWith("UnityEngine.")) return;
            if (moduleName.StartsWith("UnityEditor.")) return;
            if (moduleName.StartsWith("Unity.")) return;
            if (moduleName.StartsWith("VContainer.") && !moduleName.Contains("Test")) return;

            var references = ReferenceSymbols.Create(context.Compilation);
            if (references is null) return;

            var codeWriter = new CodeWriter();
            var builder = new CodeBuilder.CodeBuilder();
            var syntaxCollector = (SyntaxCollector)context.SyntaxReceiver!;
            foreach (var workItem in syntaxCollector.WorkItems)
            {
                var typeMeta = workItem.Analyze(in context, references);
                if (typeMeta is null) continue;

                codeWriter.Clear();
                builder.Clear();

                try
                {
                    BuildInjectorForType(builder, typeMeta, references);
                    builder.Build(codeWriter);
                }
                catch (CodeBuildFailedException codeBuildFailedException)
                {
                    if (typeMeta.ExplicitInjectable)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            codeBuildFailedException.descriptor,
                            typeMeta.Syntax.Identifier.GetLocation(),
                            typeMeta.Symbol.Name));
                    }

                    continue;
                }

                var fullType = typeMeta.FullTypeName
                    .Replace("<", "_")
                    .Replace(">", "_")
                    .Replace(".", "_");

                context.AddSource($"{fullType}GeneratedInjector.g.cs", codeWriter.ToString());
            }
        }

        static void BuildInjectorForType(CodeBuilder.CodeBuilder codeBuilder, TypeMeta typeMeta, ReferenceSymbols references)
        {
            if (typeMeta.IsNested() && !typeMeta.Symbol.CanBeCallFromInternal())
            {
                throw new CodeBuildFailedException(DiagnosticDescriptors.PrivateNestedNotSupported);
            }

            if (typeMeta.Symbol.IsAbstract)
            {
                throw new CodeBuildFailedException(DiagnosticDescriptors.AbstractNotAllow);
            }

            if (typeMeta.IsGenerics)
            {
                throw new CodeBuildFailedException(DiagnosticDescriptors.GenericsNotSupported);
            }

            IScope scope = codeBuilder
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("VContainer");

            var ns = typeMeta.Symbol.ContainingNamespace;
            if (!ns.IsGlobalNamespace)
            {
                scope = scope.StartNamespaceScope(ns.ToString());
            }

            var definition = ObjectDefinition.ClassDefault.WithInterface("IInjector");

            var typeName = typeMeta.TypeName
                .Replace("<", "_")
                .Replace(">", "_")
                .Replace(".", "_");

            var generateTypeName = $"{typeName}GeneratedInjector";

            scope = scope.StartClassScope(generateTypeName, definition);
            scope.Space();
            CreateInstanceMethod(scope, typeMeta, references);
            scope.Space();
            CreateInjectMethod(scope, typeMeta);
            scope.Space();
        }

        static void CreateInstanceMethod(IScope scope, TypeMeta typeMeta, ReferenceSymbols references)
        {
            if (typeMeta.ExplictInjectConstructors.Count > 1)
            {
                throw new CodeBuildFailedException(DiagnosticDescriptors.MultipleCtorAttributeNotSupported);
            }

            var constructorSymbol = typeMeta.ExplictInjectConstructors.Count == 1
                ? typeMeta.ExplictInjectConstructors.First()
                : typeMeta.Constructors.OrderByDescending(ctor => ctor.Parameters.Length).FirstOrDefault();

            if (constructorSymbol != null)
            {
                if (!constructorSymbol.CanBeCallFromInternal())
                {
                    throw new CodeBuildFailedException(DiagnosticDescriptors.PrivateConstructorNotSupported);
                }

                if (constructorSymbol.Arity > 0)
                {
                    throw new CodeBuildFailedException(DiagnosticDescriptors.GenericsNotSupported);
                }
            }

            scope = scope.StartMethodScope(
                AccessibilityLevel.Public,
                "object",
                "CreateInstance",
                ("IObjectResolver", "resolver"),
                ("IReadOnlyList<IInjectParameter>", "parameters"));

            if (references.UnityEngineComponent != null &&
                typeMeta.InheritsFrom(references.UnityEngineComponent))
            {
                scope.AddLine(
                    $"throw new NotSupportedException(\"UnityEngine.Component:{typeMeta.TypeName} cannot be `new`\");");
            }
            else if (constructorSymbol is null)
            {
                scope
                    .AddLine($"{typeMeta.TypeName} __instance = new {typeMeta.TypeName}();")
                    .AddLine("Inject(__instance, resolver, parameters);")
                    .AddLine("return __instance;");
            }
            else
            {
                var parameters = constructorSymbol.Parameters
                    .Select(param =>
                    {
                        var paramType =
                            param.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        var paramName = param.Name;
                        return (paramType, paramName);
                    })
                    .ToArray();

                var resolveLines = parameters.Select(param => $"var __{param.paramName} = resolver.ResolveOrParameter(typeof({param.paramType}), \"{param.paramName}\", parameters);");
                var arguments = parameters.Select(x => $"({x.paramType})__{x.paramName}");

                scope
                    .AddLines(resolveLines)
                    .AddLine($"{typeMeta.TypeName} __instance = new {typeMeta.TypeName}({string.Join(", ", arguments)});")
                    .AddLine("Inject(__instance, resolver, parameters);")
                    .AddLine("return __instance;");
            }
        }

        static void CreateInjectMethod(IScope scope, TypeMeta typeMeta)
        {
            scope = scope.StartMethodScope(
                AccessibilityLevel.Public,
                "void",
                "Inject",
                ("object", "instance"),
                ("IObjectResolver", "resolver"),
                ("IReadOnlyList<IInjectParameter>", "parameters"));

            if (typeMeta.InjectFields.Count <= 0 &&
                typeMeta.InjectProperties.Count <= 0 &&
                typeMeta.InjectMethods.Count <= 0)
            {
                scope.AddLine("return;");
            }
            else
            {
                // verify field
                foreach (var fieldSymbol in typeMeta.InjectFields)
                {
                    if (!fieldSymbol.CanBeCallFromInternal())
                    {
                        throw new CodeBuildFailedException(DiagnosticDescriptors.PrivateFieldNotSupported);
                    }

                    if (fieldSymbol.Type is ITypeParameterSymbol)
                    {
                        throw new CodeBuildFailedException(DiagnosticDescriptors.GenericsNotSupported);
                    }
                }

                // verify property
                foreach (var propSymbol in typeMeta.InjectProperties)
                {
                    if (!propSymbol.CanBeCallFromInternal())
                    {
                        throw new CodeBuildFailedException(DiagnosticDescriptors.PrivatePropertyNotSupported);
                    }

                    if (propSymbol.Type is ITypeParameterSymbol)
                    {
                        throw new CodeBuildFailedException(DiagnosticDescriptors.GenericsNotSupported);
                    }
                }

                // verify method
                if (typeMeta.InjectMethods.Any(symbol => symbol.IsGenericMethod))
                {
                    throw new CodeBuildFailedException(DiagnosticDescriptors.GenericsNotSupported);
                }

                scope.AddLine($"var __x = ({typeMeta.TypeName})instance;");

                foreach (var fieldSymbol in typeMeta.InjectFields)
                {
                    var fieldTypeName = fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    scope.AddLine($"__x.{fieldSymbol.Name} = ({fieldTypeName})resolver.ResolveOrParameter(typeof({fieldTypeName}), \"{fieldSymbol.Name}\", parameters);");
                }

                foreach (var propSymbol in typeMeta.InjectProperties)
                {
                    var propTypeName = propSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    scope.AddLine($"__x.{propSymbol.Name} = ({propTypeName})resolver.ResolveOrParameter(typeof({propTypeName}), \"{propSymbol.Name}\", parameters);");
                }

                foreach (var methodSymbol in typeMeta.InjectMethods)
                {
                    var parameters = methodSymbol.Parameters
                        .Select(param =>
                        {
                            var paramType =
                                param.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            var paramName = param.Name;
                            return (paramType, paramName);
                        })
                        .ToArray();

                    foreach (var (paramType, paramName) in parameters)
                    {
                        scope.AddLine($"var __{paramName} = resolver.ResolveOrParameter(typeof({paramType}), \"{paramName}\", parameters);");
                    }

                    var arguments = parameters.Select(x => $"({x.paramType})__{x.paramName}");
                    scope.AddLine($"__x.{methodSymbol.Name}({string.Join(", ", arguments)});");
                }
            }
        }

        class CodeBuildFailedException : Exception
        {
            public readonly DiagnosticDescriptor descriptor;

            public CodeBuildFailedException(DiagnosticDescriptor descriptor)
            {
                this.descriptor = descriptor;
            }
        }
    }
}