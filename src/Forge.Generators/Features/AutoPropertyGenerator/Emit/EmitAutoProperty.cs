using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Forge.Annotations;
using Forge.Generators.Common.Emit;
using Forge.Generators.Features.AutoPropertyGenerator.Models;
using Forge.RoslynShared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using GeneratedSource = (string name, Microsoft.CodeAnalysis.Text.SourceText sourceText);

namespace Forge.Generators.Features.AutoPropertyGenerator.Emit;

internal static class EmitAutoProperty {
    internal static GeneratedSource Emit(GroupTargetModel targetGroup, NamingPolicy namingPolicy) {
        using StringWriter sr = new();
        using IndentedTextWriter writer = new(sr);
        
        EmitHelpers.EmitGeneratedFileHeader(writer);
        
        if (targetGroup.TypeDecl.AsTypeRef.Namespace is not null) {
            writer.WriteLine($"namespace {targetGroup.TypeDecl.AsTypeRef.Namespace} {{");
            writer.Indent++;
        }
        
        EmitHelpers.EmitClassDeclarationFromModel(targetGroup.TypeDecl, writer);

        foreach (TargetFieldModel field in targetGroup.TargetFields) {
            WriteProperty(writer, field, namingPolicy);
        }
        
        // closes class
        writer.Indent--;
        writer.WriteLine("}");
        
        if (targetGroup.TypeDecl.AsTypeRef.Namespace is not null) {
            writer.Indent--;
            writer.WriteLine("}");
        }
        
        SourceText text = SourceText.From(sr.ToString(), Encoding.UTF8);
        return ($"{targetGroup.TypeDecl.AsTypeRef.FlattenedNameNonArityBased}_AutoProperty.g.cs", text);
    }

    private static void WriteProperty(IndentedTextWriter writer, TargetFieldModel field, NamingPolicy namingPolicy) {
        string visibility = ((Accessibility)field.TargetVisibility).AsDeclString();
        string returnMode = GetReturnModeStringRepr(field.TargetReturnMode);
        string propertyName = NamingPolicyEngine.Transform(field.Name, namingPolicy) ?? field.Name;
        
        writer.Write($"{visibility} {returnMode} {field.FieldType.FQNGenericBased} {propertyName}");

        switch (field.TargetAccessors) {
            case Accessors.Get:
                writer.WriteLine($" => {returnMode} {field.Name};");
                break;
            case Accessors.Set:
                writer.WriteLine($" {{ set => {field.Name} = value; }}");
                break;
            case Accessors.GetAndSet:
                writer.WriteLine(" {");
                writer.Indent++;
                writer.WriteLine($"get => {field.Name};");
                writer.WriteLine($"set => {field.Name} = value;");
                writer.Indent--;
                writer.WriteLine("}");
                break;
            case Accessors.GetAndInit:
                writer.WriteLine(" {");
                writer.Indent++;
                writer.WriteLine($"get => {field.Name};");
                writer.WriteLine($"init => {field.Name} = value;");
                writer.Indent--;
                writer.WriteLine("}");
                break;
            default:
                ThrowHelpers.ThrowUnhandledBranch(field.TargetAccessors);
                break;
        }
    }

    private static string GetReturnModeStringRepr(ReturnMode returnMode) {
        return returnMode switch {
            ReturnMode.RefStruct => "ref",
            _ => ""
        };
    }
}