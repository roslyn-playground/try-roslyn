using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.LanguageServices;

namespace SemanticsCS
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = CSharpSyntaxTree.ParseText(
@"using System;
using System.Collection.Generic;
using System.Text;
namespace HelloWorld {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine(""Hello, world!"");
        }
    }
}"
            );

            var root = (CompilationUnitSyntax)tree.GetRoot();

            var location = typeof(object).Assembly.Location;
            var reference = MetadataReference.CreateFromFile(location);

            var compilation = CSharpCompilation.Create("HelloWorld")
                .AddReferences(reference)
                .AddSyntaxTrees(tree);

            var model = compilation.GetSemanticModel(tree);
            var nameInfo = model.GetSymbolInfo(root.Usings[0].Name);
            var systemSymbol = (INamespaceSymbol)nameInfo.Symbol;

            foreach(var ns in systemSymbol.GetNamespaceMembers())
            {
                Console.WriteLine(ns.Name);
            }


            var helloWorldString = root.DescendantNodes()
                .OfType<LiteralExpressionSyntax>()
                .First();

            var literalInfo = model.GetTypeInfo(helloWorldString);

            var stringTypeSymbol = (INamedTypeSymbol)literalInfo.Type;
            var names =
                (from method in stringTypeSymbol.GetMembers().OfType<IMethodSymbol>()
                 where method.ReturnType.Equals(stringTypeSymbol) &&
                     method.DeclaredAccessibility == Accessibility.Public
                 select method.Name).Distinct();

            foreach(var name in names)
            {
                Console.WriteLine(name);
            }

            var rq = RQName.From(stringTypeSymbol);

            Console.ReadLine();
        }
    }
}
