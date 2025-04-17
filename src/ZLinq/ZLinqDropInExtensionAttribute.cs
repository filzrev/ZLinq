namespace ZLinq;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class ZLinqDropInExtensionAttribute : Attribute;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public class ZLinqDropInExternalExtensionAttribute : Attribute
{
    /// <summary>
    /// Gets the namespace where the generated LINQ implementations will be placed.
    /// If empty, the implementations will be generated in the global namespace.
    /// </summary>
    public string GenerateNamespace { get; }

    public string SourceTypeName { get; }

    public string EnumeratorTypeName { get; }

    public string ElementName { get; }

    public bool IsElementValueType { get; }

    public string ElementConstraint { get; }

    public string[] UsingNamespaces { get; }

    /// <summary>
    /// Gets whether the generated LINQ implementations should be public.
    /// When true, the implementations will be generated with public visibility.
    /// When false (default), the implementations will be generated with internal visibility.
    /// </summary>
    public bool GenerateAsPublic { get; set; }

    /// <param name="generateNamespace">The namespace where the generated LINQ implementations will be placed. If empty, place to global.</param>
    /// <param name="sourceTypeName"></param>
    /// <param name="enumeratorTypeName">Set IValueEnumerator&lt;T&gt; name or "IEnumerator&lt;T&gt;"</param>
    /// <param name="elementName"></param>
    /// <param name="isElementValueType"></param>
    /// <param name="elementConstraint">If elementType is generic and T has constraint, set like "where T : struct", othwewise "".</param>
    /// <param name="usingNamespaces"></param>
    public ZLinqDropInExternalExtensionAttribute(
        string generateNamespace,
        string sourceTypeName,
        string enumeratorTypeName,
        string elementName,
        bool isElementValueType,
        string elementConstraint,
        params string[] usingNamespaces)
    {
        GenerateNamespace = generateNamespace;
        SourceTypeName = sourceTypeName;
        EnumeratorTypeName = enumeratorTypeName;
        ElementName = elementName;
        IsElementValueType = isElementValueType;
        ElementConstraint = elementConstraint;
        UsingNamespaces = usingNamespaces;
    }
}
