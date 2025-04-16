using Microsoft.CodeAnalysis;
using System;
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
                //x.SemanticModel.GetSymbolInfo(
                string? elementName = null;
                bool isGenericType = false;
                if (x.TargetSymbol is INamedTypeSymbol namedTypeSymbol)
                {
                    foreach (var item in namedTypeSymbol.AllInterfaces)
                    {
                        if (item.MetadataName is "IValueEnumerable`2" or "IEnumerable`1")
                        {
                            var typeArg = item.TypeArguments[0];
                            elementName = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            break;
                        }
                    }
                }

                if (elementName == null)
                {
                    // report diagnostics
                }

                return new DropInExtension(x.TargetSymbol.ContainingNamespace.Name, x.TargetSymbol.Name, elementName, isGenericType, x.TargetSymbol.DeclaredAccessibility);
            });

        context.RegisterSourceOutput(extension, EmitDropInExtension);
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
            if (!attribute.DropInGenerateTypes.HasFlag(dropinType))
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

    void EmitDropInExtension(SourceProductionContext context, DropInExtension extension)
    {
        if (extension.ElementName == null) return;

        var sb = new StringBuilder();

        // based Array Resource(this TSource[])
        var thisAsm = typeof(DropInGenerator).Assembly;
        using (var stream = thisAsm.GetManifestResourceStream("ZLinq.DropInGenerator.ResourceCodes.Array.cs"))
        using (var sr = new StreamReader(stream!))
        {
            var code = sr.ReadToEnd();

            var element = extension.ElementName;

            // Repleace this TSource[] ...=> this CustomType
            code = Regex.Replace(code, "this .+\\[\\]", x => "this " + extension.TypeName);
            code = Regex.Replace(code, "FromArray<[^>]+>", $"FromEnumerable<{element}>");

            // element type fix
            code = code.Replace("FromEnumerable<TSource>", $"FromEnumerable<{element}>");
            code = code.Replace("IEnumerable<TSource>", $"IEnumerable<{element}>");
            code = code.Replace("ValueEnumerable<TEnumerator2, TSource>", $"ValueEnumerable<TEnumerator2, {element}>");
            code = code.Replace("IValueEnumerator<TSource>", $"IValueEnumerator<{element}>");
            code = code.Replace("IEqualityComparer<TSource>", $"IEqualityComparer<{element}>");
            code = code.Replace("IComparer<TSource>", $"IComparer<{element}>");
            code = code.Replace("Span<TSource>", $"Span<{element}>");
            code = code.Replace("List<TSource>", $"List<{element}>");
            code = code.Replace("HashSet<TSource>", $"HashSet<{element}>");

            // generic defnition fix
            code = code.Replace("<TEnumerator2, TSource>", "<TEnumerator2>");
            code = code.Replace("<TEnumerator2, TOuter, ", "<TEnumerator2, ");
            code = code.Replace("Func<TOuter, TInner", $"Func<{element}, TInner");
            code = code.Replace("Func<TOuter?, TInner", $"Func<{element}, TInner"); // is reference type or not?

            code = code.Replace("<TOuter, TInner", "<TInner");
            code = code.Replace("TOuter, ", $"{element}, ");

            code = code.Replace("ToList<TSource>", $"ToList");
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

            // Replace TSource -> ElementType(for Func, etc...)
            code = Regex.Replace(code, "TSource", extension.ElementName);

            code.Replace("WhereArray", $"Where<FromEnumerable<{element}>, {element}>");

            sb.AppendLine(code); // write code
        }

        // replace namespace
        //        if (!string.IsNullOrWhiteSpace(attribute.GenerateNamespace))
        //        {
        //            sb.Replace("using ZLinq.Linq;", $$"""
        //using ZLinq.Linq;
        //namespace {{attribute.GenerateNamespace}}
        //{
        //""");
        //        }

        // replace accessibility
        //if (attribute.GenerateAsPublic)
        //{
        //    sb.Replace("internal static partial class", "public static partial class");
        //}


        // namespace close
        //if (!string.IsNullOrWhiteSpace(attribute.GenerateNamespace))
        //{
        //    sb.AppendLine("}");
        //}

        var fullType = extension.NamespaceName == "" ? extension.TypeName : (extension.NamespaceName + "." + extension.TypeName);
        var hintName = "ZLinq.DropIn." + fullType + "Extensions.g.cs";


        // context.AddSource(hintName, sb.ToString().ReplaceLineEndings());
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
}

record struct DropInExtension(string NamespaceName, string TypeName, string? ElementName, bool IsElementGenericType, Accessibility Accessibility);

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
