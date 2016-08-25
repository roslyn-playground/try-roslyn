using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ConstructionCS
{
    class Program
    {
        static void Main(string[] args)
        {

            var tree = CSharpSyntaxTree.ParseText(
@"using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace HelloWorld {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine(""Hello, World!"")j
        }
    } 
}
"
                );

            var root = (CompilationUnitSyntax)tree.GetRoot();


            NameSyntax name = IdentifierName("System");
            name = QualifiedName(name, IdentifierName("Collections"));
            name = QualifiedName(name, IdentifierName("Generic"));

            var oldUsing = root.Usings[1];
            var newUsing = oldUsing.WithName(name);


            root = root.ReplaceNode(oldUsing, newUsing);

        }
    }
}
