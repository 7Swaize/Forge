using System;

namespace Forge.Annotations;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class AutoPropertyNamingPolicyAttribute(NamingPolicy namingPolicy) : Attribute {
    public NamingPolicy NamingPolicy { get; private set; } = namingPolicy;
}

public enum NamingPolicy : byte {
    /// <summary> Preserves the exact name minus standard field prefixes </summary>
    Preserve = 0, 
    /// <summary> Convert to PascalCase (e.g., _xmlParser -> XmlParser) </summary>
    PascalCase = 1,
    /// <summary> Convert to camelCase (e.g., m_XmlParser -> xmlParser) </summary>
    CamelCase = 2,
    /// <summary> Convert to snake_case (e.g., XmlParser -> xml_parser) </summary>
    SnakeCase = 3,
    /// <summary> Convert to kebab-case (e.g., XmlParser -> xml-parser) </summary>
    KebabCase = 4, 
}