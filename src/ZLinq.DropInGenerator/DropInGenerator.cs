using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace ZLinq;

[Generator(LanguageNames.CSharp)]
public class DropInGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // AssemblyAttribtue

        var generatorOptions = context.CompilationProvider.Select((compilation, token) =>
        {
            foreach (var attr in compilation.Assembly.GetAttributes())
            {
                if (attr.AttributeClass?.Name is "ZLinqDropIn" or "ZLinqDropInAttribute")
                {
                    // (string generateNamespace, DropInGenerateTypes dropInGenerateTypes)
                    var ctor = attr.ConstructorArguments;

                    var generateNamespace = (string)ctor[0].Value!;
                    var dropInGenerateTypes = (DropInGenerateTypes)ctor[1].Value!;

                    var args = attr.NamedArguments;

                    var generateAsPublic = args.FirstOrDefault(x => x.Key == "GenerateAsPublic").Value.Value as bool? ?? false;
                    var conditionalCompilationSymbols = args.FirstOrDefault(x => x.Key == "ConditionalCompilationSymbols").Value.Value as string;
                    var disableEmitSource = args.FirstOrDefault(x => x.Key == "DisableEmitSource").Value.Value as bool? ?? false;

                    return new ZLinqDropInAttribute(generateNamespace, dropInGenerateTypes)
                    {
                        GenerateAsPublic = generateAsPublic,
                        ConditionalCompilationSymbols = conditionalCompilationSymbols,
                        DisableEmitSource = disableEmitSource
                    };
                }
            }

            return null;
        });

        context.RegisterSourceOutput(generatorOptions, EmitSourceOutput);

        // Extension per type

        var extension = context.SyntaxProvider.ForAttributeWithMetadataName("ZLinq.ZLinqDropInExtensionAttribute",
            (_, _) => true, // Class or Struct
            (x, _) =>
            {
                string? elementName = null;
                string? valueEnumeratorName = null;
                bool isElementGenericType = false;
                bool isValueType = false;
                var namedTypeSymbol = x.TargetSymbol as INamedTypeSymbol;
                if (namedTypeSymbol != null)
                {
                    isElementGenericType = namedTypeSymbol.IsGenericType;

                    foreach (var item in namedTypeSymbol.AllInterfaces)
                    {
                        if (item.MetadataName is "IValueEnumerable`2")
                        {
                            // 0 = TEnumerator, 1 = TSource
                            var typeArg = item.TypeArguments[1];
                            elementName = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            isValueType = typeArg.IsValueType;
                            valueEnumeratorName = item.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            break;
                        }
                        else if (item.MetadataName is "IEnumerable`1")
                        {
                            var typeArg = item.TypeArguments[0];
                            elementName = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            isValueType = typeArg.IsValueType;
                        }
                    }
                }

                if (namedTypeSymbol == null || elementName == null)
                {
                    var location = x.Attributes[0].ApplicationSyntaxReference?.GetSyntax()?.GetLocation();
                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ExtensionNotSupported, location);
                    return new DropInExtension(diagnostic);
                }

                if (isElementGenericType)
                {
                    if (namedTypeSymbol.TypeArguments.Length != 1)
                    {
                        var location = x.Attributes[0].ApplicationSyntaxReference?.GetSyntax()?.GetLocation();
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.GenericTypeArgumentTooMuch, location);
                        return new DropInExtension(diagnostic);
                    }
                }

                var fullyQualified = x.TargetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var fullNamespae = x.TargetSymbol.ContainingNamespace.IsGlobalNamespace ? "" : x.TargetSymbol.ContainingNamespace.ToDisplayString();
                string constraint = FormatGenericConstraints(namedTypeSymbol);
                return new DropInExtension(fullNamespae, x.TargetSymbol.Name, fullyQualified, valueEnumeratorName, elementName, isElementGenericType, isValueType, constraint, x.TargetSymbol.DeclaredAccessibility);
            });

        context.RegisterSourceOutput(extension, EmitDropInExtension);

        // Extension per type from assembly

        var externalExtensions = context.CompilationProvider.SelectMany((compilation, token) =>
        {
            var list = new List<DropInExtension>();
            foreach (var attr in compilation.Assembly.GetAttributes())
            {
                if (attr.AttributeClass?.Name is "ZLinqDropInExternalExtension" or "ZLinqDropInExternalExtensionAttribute")
                {
                    var ctor = attr.ConstructorArguments;
                    if (ctor.Length == 0) continue;

                    // parse from attr
                    var generateNamespace = (string)ctor[0].Value!;
                    var sourceTypeFullyQualifiedMetadataName = (string)ctor[1].Value!;
                    var enumeratorTypeFullyQualifiedMetadataName = (string?)ctor[2].Value!;

                    var args = attr.NamedArguments;
                    var generateAsPublic = args.FirstOrDefault(x => x.Key == "GenerateAsPublic").Value.Value as bool? ?? false;

                    // construct from compilation info

                    var namedTypeSymbol = compilation.GetTypeByMetadataName(sourceTypeFullyQualifiedMetadataName);

                    if (namedTypeSymbol == null)
                    {
                        // TODO: diagnostics
                        continue;
                    }

                    string? elementName = null;
                    string? valueEnumeratorName = null;
                    bool isElementGenericType = namedTypeSymbol.IsGenericType;
                    bool isValueType = false;

                    if (enumeratorTypeFullyQualifiedMetadataName == null)
                    {
                        // try to get IEnumerable<T>
                        foreach (var item in namedTypeSymbol.AllInterfaces)
                        {
                            if (item.MetadataName is "IEnumerable`1")
                            {
                                var typeArg = item.TypeArguments[0];
                                elementName = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                                isValueType = typeArg.IsValueType;
                            }
                        }
                    }
                    else
                    {
                        // try to get value-enumerator
                        var enumeratorTypeSymbol = compilation.GetTypeByMetadataName(enumeratorTypeFullyQualifiedMetadataName);
                        if (enumeratorTypeSymbol == null)
                        {
                            // TODO: diagnostics
                            continue;
                        }

                        foreach (var item in enumeratorTypeSymbol.AllInterfaces)
                        {
                            if (item.MetadataName is "IValueEnumerator`1")
                            {
                                // 0 = TEnumerator, 1 = TSource
                                var typeArg = item.TypeArguments[0];
                                elementName = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                                isValueType = typeArg.IsValueType;
                                valueEnumeratorName = enumeratorTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                                break;
                            }
                        }
                    }

                    if (elementName == null)
                    {
                        var location = attr.ApplicationSyntaxReference?.GetSyntax()?.GetLocation();
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ExtensionNotSupported, location); // TODO: other diagnostics
                        list.Add(new DropInExtension(diagnostic));
                        continue;
                    }

                    if (isElementGenericType)
                    {
                        if (namedTypeSymbol.TypeArguments.Length != 1)
                        {
                            var location = attr.ApplicationSyntaxReference?.GetSyntax()?.GetLocation();
                            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.GenericTypeArgumentTooMuch, location);
                            list.Add(new DropInExtension(diagnostic));
                            continue;
                        }
                    }

                    var fullyQualified = namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var accessibility = generateAsPublic ? Accessibility.Public : Accessibility.Internal;

                    string constraint = FormatGenericConstraints(namedTypeSymbol);
                    var extension = new DropInExtension(generateNamespace, namedTypeSymbol.Name, fullyQualified, valueEnumeratorName, elementName, isElementGenericType, isValueType, constraint, accessibility);
                    list.Add(extension);
                }
            }

            return list;
        });

        context.RegisterSourceOutput(externalExtensions, EmitDropInExtension);
    }

    void EmitSourceOutput(SourceProductionContext context, ZLinqDropInAttribute? attribute)
    {
        if (attribute is null)
        {
            // In Unity, Source Generators are automatically referenced even in assemblies that don't need them, causing numerous errors.
            // Until an effective countermeasure can be devised, these warnings will be temporarily excluded.
            // context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.AttributeNotFound, null));
            return;
        }

        if (attribute.DisableEmitSource)
        {
            return;
        }

        var thisAsm = typeof(DropInGenerator).Assembly;
        var resourceNames = thisAsm.GetManifestResourceNames();
        foreach (var resourceName in resourceNames)
        {
            var dropinType = FromResourceName(resourceName);
            if (dropinType == DropInGenerateTypes.None || !attribute.DropInGenerateTypes.HasFlag(dropinType))
            {
                continue;
            }

            var sb = new StringBuilder();

            // #if...
            if (attribute.ConditionalCompilationSymbols is not null)
            {
                sb.AppendLine($"#if {attribute.ConditionalCompilationSymbols}");
            }

            if (dropinType == DropInGenerateTypes.Span)
            {
                sb.AppendLine("#if NET9_0_OR_GREATER");
            }

            using (var stream = thisAsm.GetManifestResourceStream(resourceName))
            using (var sr = new StreamReader(stream!))
            {
                var code = sr.ReadToEnd();
                sb.AppendLine(code); // write code
            }

            // replaces...
            if (!string.IsNullOrWhiteSpace(attribute.GenerateNamespace))
            {
                sb.Replace("using ZLinq.Linq;", $$"""
using ZLinq.Linq;
namespace {{attribute.GenerateNamespace}}
{
""");
            }

            if (attribute.GenerateAsPublic)
            {
                sb.Replace("internal static partial class", "public static partial class");
            }


            if (!string.IsNullOrWhiteSpace(attribute.GenerateNamespace))
            {
                sb.AppendLine("}");
            }
            if (attribute.ConditionalCompilationSymbols is not null)
            {
                sb.AppendLine($"#endif");
            }

            if (dropinType == DropInGenerateTypes.Span)
            {
                sb.AppendLine("#endif");
            }

            var hintName = resourceName.Replace("ZLinq.DropInGenerator.ResourceCodes.", "ZLinq.DropIn.").Replace(".cs", ".g.cs");
            context.AddSource($"{hintName}", sb.ToString().ReplaceLineEndings());
        }
    }

    void EmitDropInExtension(SourceProductionContext context, DropInExtension? extension)
    {
        if (extension == null) return;
        if (extension.Error != null)
        {
            context.ReportDiagnostic(extension.Error);
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("#define ENABLE_EXTENSION");

        var thisAsm = typeof(DropInGenerator).Assembly;
        using (var stream = thisAsm.GetManifestResourceStream("ZLinq.DropInGenerator.ResourceCodes.ForExtension.cs"))
        using (var sr = new StreamReader(stream!))
        {
            var code = sr.ReadToEnd();

            var element = extension.ElementName;

            // Repleace this TSource[] ...=> this CustomType
            code = Regex.Replace(code, "this .+\\[\\]", x => "this " + extension.TypeNameFullyQualified); // ToString shows <T> if type is generics
            if (extension.ValueEnumeratorName != null)
            {
                code = Regex.Replace(code, "FromArray<[^>]+>", $"{extension.ValueEnumeratorName}");
            }
            else
            {
                code = Regex.Replace(code, "FromArray<[^>]+>", $"FromEnumerable<{element}>");
            }

            // element type fix
            code = code.Replace("FromEnumerable<TSource>", $"FromEnumerable<{element}>");
            code = code.Replace("IEnumerable<TSource>", $"IEnumerable<{element}>");
            code = code.Replace("ValueEnumerable<TEnumerator2, TSource>", $"ValueEnumerable<TEnumerator2, {element}>");
            code = code.Replace("IValueEnumerator<TSource>", $"IValueEnumerator<{element}>");
            code = code.Replace("IEqualityComparer<TSource>", $"IEqualityComparer<{element}>");
            code = code.Replace("IComparer<TSource>", $"IComparer<{element}>");

            // generic defnition fix
            if (!extension.IsElementGenericType)
            {
                code = code.Replace("<TEnumerator2, TSource>", "<TEnumerator2>");
                code = code.Replace("<TEnumerator2, TOuter, ", "<TEnumerator2, ");
                code = code.Replace("Func<TOuter, TInner", $"Func<{element}, TInner");
                code = code.Replace("Func<TOuter?, TInner", $"Func<{element}{(extension.IsElementValueType ? "" : "?")}, TInner"); // for RightJoin
                code = code.Replace("<TOuter, TInner", "<TInner");
                code = code.Replace("TOuter, ", $"{element}, ");
                code = code.Replace("<TEnumerator2, TFirst", "<TEnumerator2");
                code = code.Replace("<TEnumerator2, TEnumerator3, TFirst", "<TEnumerator2, TEnumerator3");
                code = code.Replace("Func<TFirst, TSecond", $"Func<{element}, TSecond");
                code = code.Replace("<TFirst, TSecond", "<TSecond");
                code = code.Replace("TFirst", $"{element}");
                code = code.Replace("ToList<TSource>", $"ToList");
                code = code.Replace("ToHashSet<TSource>", $"ToHashSet");
                code = code.Replace("Span<TSource>", $"Span<{element}>");
                code = code.Replace("List<TSource>", $"List<{element}>");
                code = code.Replace("HashSet<TSource>", $"HashSet<{element}>");
                code = code.Replace("<TSource>", "");
                code = Regex.Replace(code, "<(.*)TSource, (.+?)>\\(", x =>
                {
                    if (string.IsNullOrEmpty(x.Groups[1].Value))
                    {
                        return "<" + x.Groups[2].Value + ">(";
                    }
                    else
                    {
                        return "<" + x.Groups[1].Value + x.Groups[2].Value + ">(";
                    }
                });
            }
            else
            {
                // for RightJoin, remove ? annotation where T:struct
                if (extension.ElementConstraint.Contains("struct"))
                {
                    code = code.Replace("Func<TOuter?, TInner", $"Func<{element}{(extension.IsElementValueType ? "" : "?")}, TInner");
                }

                code = code.Replace("TOuter", element);
                code = code.Replace("TFirst", element);
            }

            // Replace TSource -> ElementType(for Func, etc...)
            code = Regex.Replace(code, "TSource", extension.ElementName);
            if (!extension.IsElementGenericType)
            {
                if (extension.ValueEnumeratorName != null)
                {
                    code = code.Replace("WhereArray", $"Where<{extension.ValueEnumeratorName}, {element}>");
                }
                else
                {
                    code = code.Replace("WhereArray", $"Where<FromEnumerable<{element}>, {element}>");
                }
            }
            else
            {
                if (extension.ValueEnumeratorName != null)
                {
                    code = Regex.Replace(code, "WhereArray<.+?>", $"Where<{extension.ValueEnumeratorName}, {element}>");
                }
                else
                {
                    code = Regex.Replace(code, "WhereArray<.+?>", $"Where<FromEnumerable<{element}>, {element}>");
                }
            }

            // finally remove constraint
            if (extension.ElementConstraint != "")
            {
                code = code.Replace("=>", $" {extension.ElementConstraint} =>");
            }

            sb.AppendLine(code); // write code
        }

        // replace namespace
        if (!string.IsNullOrWhiteSpace(extension.NamespaceName))
        {
            sb.Replace("using ZLinq.Linq;", $$"""
using ZLinq.Linq;
namespace {{extension.NamespaceName}}
{
""");
        }

        if (extension.AdditionalUsingNamespaces != null && extension.AdditionalUsingNamespaces.Length > 0)
        {
            var usings = string.Join(Environment.NewLine, extension.AdditionalUsingNamespaces.Select(x => $"using {x};").Concat(["using ZLinq.Linq;"]));
            sb.Replace("using ZLinq.Linq;", usings);
        }

        // replace accessibility
        sb.Replace("internal static partial class", $"{extension.Accessibility.ToString().ToLower().Replace("and", " ").Replace("or", " ").Replace("friend", "internal")} static partial class");

        // replace typename
        sb.Replace("class ZLinqDropInExtensions", $"class {extension.TypeName}ZLinqDropInExtensions");

        // namespace close
        if (!string.IsNullOrWhiteSpace(extension.NamespaceName))
        {
            sb.AppendLine("}");
        }

        var fullType = extension.NamespaceName == "" ? extension.TypeName : (extension.NamespaceName + "." + extension.TypeName);
        var hintName = "ZLinq.DropIn." + fullType + "Extensions.g.cs";
        context.AddSource(hintName, sb.ToString().ReplaceLineEndings());
    }

    DropInGenerateTypes FromResourceName(string name)
    {
        switch (name)
        {
            case "ZLinq.DropInGenerator.ResourceCodes.Array.cs": return DropInGenerateTypes.Array;
            case "ZLinq.DropInGenerator.ResourceCodes.IEnumerable.cs": return DropInGenerateTypes.Enumerable;
            case "ZLinq.DropInGenerator.ResourceCodes.List.cs": return DropInGenerateTypes.List;
            case "ZLinq.DropInGenerator.ResourceCodes.Memory.cs": return DropInGenerateTypes.Memory;
            case "ZLinq.DropInGenerator.ResourceCodes.ReadOnlyMemory.cs": return DropInGenerateTypes.Memory;
            case "ZLinq.DropInGenerator.ResourceCodes.ReadOnlySpan.cs": return DropInGenerateTypes.Span;
            case "ZLinq.DropInGenerator.ResourceCodes.Span.cs": return DropInGenerateTypes.Span;
            default: return DropInGenerateTypes.None;
        }
    }

    static string FormatGenericConstraints(INamedTypeSymbol namedType)
    {
        if (!namedType.IsGenericType)
        {
            return "";
        }

        var builder = new StringBuilder();

        foreach (var typeParameter in namedType.TypeParameters)
        {
            if (!HasAnyConstraints(typeParameter))
            {
                continue;
            }

            builder.Append($"where {typeParameter.Name} : ");

            var constraints = new List<string>();

            if (typeParameter.HasReferenceTypeConstraint)
            {
                constraints.Add("class");
            }
            else if (typeParameter.HasValueTypeConstraint)
            {
                constraints.Add("struct");
            }

            foreach (var constraintType in typeParameter.ConstraintTypes)
            {
                constraints.Add(constraintType.ToDisplayString());
            }

            if (typeParameter.HasNotNullConstraint)
            {
                constraints.Add("notnull");
            }

            if (typeParameter.HasConstructorConstraint)
            {
                constraints.Add("new()");
            }

            builder.Append(string.Join(", ", constraints));
        }

        return builder.ToString();
    }

    private static bool HasAnyConstraints(ITypeParameterSymbol typeParameter)
    {
        return typeParameter.HasReferenceTypeConstraint ||
               typeParameter.HasValueTypeConstraint ||
               typeParameter.HasNotNullConstraint ||
               typeParameter.HasConstructorConstraint ||
               typeParameter.ConstraintTypes.Any();
    }
}

record class DropInExtension(
    string NamespaceName,
    string TypeName,
    string TypeNameFullyQualified, // with generics
    string? ValueEnumeratorName,
    string? ElementName,
    bool IsElementGenericType,
    bool IsElementValueType,
    string ElementConstraint,
    Accessibility Accessibility,
    string[]? AdditionalUsingNamespaces = null,
    Diagnostic? Error = null)
{

    public DropInExtension(Diagnostic error)
        : this(null!, null!, null!, null, null, false, false, null!, Accessibility.Public, null, error)
    {
    }
}

internal static class DiagnosticDescriptors
{
    const string Category = "ZLinqDropInGenerator";

    public static void ReportDiagnostic(this SourceProductionContext context, DiagnosticDescriptor diagnosticDescriptor, Location location, params object?[]? messageArgs)
    {
        var diagnostic = Diagnostic.Create(diagnosticDescriptor, location, messageArgs);
        context.ReportDiagnostic(diagnostic);
    }

    public static DiagnosticDescriptor Create(int id, string message)
    {
        return Create(id, message, message);
    }

    public static DiagnosticDescriptor Create(int id, string title, string messageFormat)
    {
        return new DiagnosticDescriptor(
            id: "ZL" + id.ToString("000"),
            title: title,
            messageFormat: messageFormat,
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }

    public static DiagnosticDescriptor AttributeNotFound { get; } = Create(
        1,
        "ZLinqDropIn AssemblyAttribute is not found, you need to add like [assembly: ZLinq.ZLinqDropInAttribute(\"ZLinq.DropIn\", ZLinq.DropInGenerateTypes.Array)].");

    public static DiagnosticDescriptor ExtensionNotSupported { get; } = Create(
        2,
        "ZLinqDropInExtension requires implementation of IEnumerable<T> or IValueEnumerable<T, TEnumerator>.");

    public static DiagnosticDescriptor GenericTypeArgumentTooMuch { get; } = Create(
        3,
        "ZLinqDropInExtension requires zero or one generic type argument.");
}

internal static class StringExtensions
{
    public static string ReplaceLineEndings(this string input)
    {
#pragma warning disable RS1035
        return ReplaceLineEndings(input, Environment.NewLine);
#pragma warning restore RS1035
    }

    public static string ReplaceLineEndings(this string text, string replacementText)
    {
        text = text.Replace("\r\n", "\n");

        if (replacementText != "\n")
            text = text.Replace("\n", replacementText);

        return text;
    }
}
