using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Forge.Generators.Common.Emit;
using Forge.Generators.Features.ZeroCopyParseGenerators.Models;
using Microsoft.CodeAnalysis.Text;
using GeneratedSource = (string name, Microsoft.CodeAnalysis.Text.SourceText sourceText);

namespace Forge.Generators.Features.ZeroCopyParseGenerators.Emit;

internal static class EmitParser {
    internal static GeneratedSource Emit(GeneratorTargetModel target) {
        using StringWriter sr = new();
        using IndentedTextWriter writer = new(sr);
        
        EmitHelpers.EmitGeneratedFileHeader(writer);
        writer.WriteLine();
        
        if (target.TypeDecl.AsTypeRef.Namespace is not null) {
            writer.WriteLine($"namespace {target.TypeDecl.AsTypeRef.Namespace} {{");
            writer.Indent++;
        }
        
        EmitHelpers.EmitGeneratedCodeAttribute(writer);
        EmitHelpers.EmitExcludeFromCodeCoverageAttribute(writer);
        EmitHelpers.EmitTypeDeclarationFromModel(target.TypeDecl, writer);
        
        // emit parser
        
        // closes structure
        writer.Indent--;
        writer.WriteLine("}");
        
        if (target.TypeDecl.AsTypeRef.Namespace is not null) {
            writer.Indent--;
            writer.WriteLine("}");
        }
        
        SourceText text = SourceText.From(sr.ToString(), Encoding.UTF8);
        return ($"{target.TypeDecl.AsTypeRef.FlattenedNameArityBased}_ZeroCopyParser.g.cs", text);
    }
}