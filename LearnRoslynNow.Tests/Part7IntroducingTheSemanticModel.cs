using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LearnRoslynNow.Tests
{
    public class Part7IntroducingTheSemanticModel
    {
        [Fact]
        public void SemanticModel()
        {
            var tree = CSharpSyntaxTree.ParseText(
@"
public class MyClass {
    int MyMethod() { return 0; }
}
");
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree }, references: new[] { mscorlib } );

            var model = compilation.GetSemanticModel(tree);

        }

        [Fact]
        public void Symbol()
        {
            var tree = CSharpSyntaxTree.ParseText(
@"
public class MyClass {
    public int Method1() { return 0; }
    void Method2() { int x = Method1(); }
}
");

            var mscorlib = PortableExecutableReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation",
                    syntaxTrees: new [] { tree }, references: new [] { mscorlib }
                );
            var model = compilation.GetSemanticModel(tree);
            var methodSyntax = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            var methodSymbol = model.GetDeclaredSymbol(methodSyntax);

            methodSymbol.ToString().Should().Be("MyClass.Method1()");
            methodSymbol.ContainingType.Name.Should().Be("MyClass");
            methodSymbol.IsAbstract.Should().Be(false);

        }
    }
}
