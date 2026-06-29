using System;
using System.Collections.Generic;
using System.Text;
using Forge.Annotations;
using Forge.RoslynShared;

namespace Forge.Generators.Features.AutoPropertyGenerator.Emit;

internal static class NamingPolicyEngine {
    private static readonly string[] KKnownPrefixes = ["m_", "f_", "s_", "t_"];
    private const string FallbackSuffix = "Value";

    internal static string? Transform(string fieldName, NamingPolicy policy) {
        if (string.IsNullOrWhiteSpace(fieldName)) {
            return null;
        }

        string cleanName = StripPrefixes(fieldName);
        if (cleanName.Length == 0) {
            return null;
        }

        List<string> tokens = Tokenize(cleanName);
        if (tokens.Count == 0) {
            return null;
        }

        string result = policy switch {
            NamingPolicy.PascalCase => AssemblePascalCase(tokens),
            NamingPolicy.CamelCase => AssembleCamelCase(tokens),
            NamingPolicy.SnakeCase => AssembleSeparated(tokens, '_'),
            _ => ThrowHelpers.ThrowUnhandledBranch<string>(policy)
        };

        ReadOnlySpan<char> canonical = fieldName.AsSpan(fieldName[0] == '@' ? 1 : 0);
        if (result.AsSpan().SequenceEqual(canonical)) {
            result += FallbackSuffix;
        }

        return result;
    }

    private static string StripPrefixes(string name) {
        int start = 0;

        if (name.Length > 1 && name[0] == '@') {
            start = 1;
        }

        foreach (string prefix in KKnownPrefixes) {
            if (name.Length - start > prefix.Length &&
                string.CompareOrdinal(name, start, prefix, 0, prefix.Length) == 0) {
                start += prefix.Length;
                break;
            }
        }

        while (start < name.Length && name[start] == '_') {
            start++;
        }

        return name.Substring(start);
    }

    private static List<string> Tokenize(string name) {
        List<string> words = new();
        int l = 0;
        int r = 0;

        while (r < name.Length) {
            char c = name[r];

            if (c is '_' or '-') {
                if (r > l) words.Add(name.Substring(l, r - l));
                l = r + 1;
                r++;
                continue;
            }

            if (r > l) {
                char prev = name[r - 1];
                bool split;

                if (char.IsUpper(c)) {
                    bool lowerToUpper = char.IsLower(prev) || char.IsDigit(prev);
                    bool acronymEnd = char.IsUpper(prev) && r + 1 < name.Length && char.IsLower(name[r + 1]);
                    split = lowerToUpper || acronymEnd;
                }
                else {
                    split = char.IsDigit(c) != char.IsDigit(prev);
                }

                if (split) {
                    words.Add(name.Substring(l, r - l));
                    l = r;
                }
            }

            r++;
        }

        if (r > l) words.Add(name.Substring(l, r - l));

        return words;
    }

    private static string AssemblePascalCase(List<string> tokens) {
        StringBuilder sb = new();
        foreach (string token in tokens) {
            sb.Append(ToTitleCase(token));
        }
        
        return sb.ToString();
    }

    private static string AssembleCamelCase(List<string> tokens) {
        StringBuilder sb = new();
        sb.Append(tokens[0].ToLowerInvariant());
        
        for (int i = 1; i < tokens.Count; i++) {
            sb.Append(ToTitleCase(tokens[i]));
        }
        
        return sb.ToString();
    }

    private static string AssembleSeparated(List<string> tokens, char separator) {
        StringBuilder sb = new();
        for (int i = 0; i < tokens.Count; i++) {
            sb.Append(tokens[i].ToLowerInvariant());
            if (i < tokens.Count - 1) sb.Append(separator);
        }

        return sb.ToString();
    }

    private static string ToTitleCase(string s) {
        if (string.IsNullOrEmpty(s)) {
            return s;
        }
        
        return char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
    }
}