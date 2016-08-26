using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace LearnRoslynNow.Tests
{
    public class Part2AnalyzingSyntaxTreeWithLinq
    {
        [Fact]
        public void Start()
        {
            var tree = CSharpSyntaxTree.ParseText(
@"
public class MyClass {
    public void MyMethod() {}
    pulbic void MyMethod(int n) {}
}
");
            var syntaxRoot = tree.GetRoot();
            var myClass = syntaxRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
            var myMethod = syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().First();

            myClass.Identifier.ValueText.Should().Be("MyClass");
            myMethod.Identifier.ValueText.Should().Be("MyMethod");

            var myMethod2 = syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(n => n.ParameterList.Parameters.Any()).First();

            var containingType = myMethod2.Ancestors().OfType<TypeDeclarationSyntax>().First();
            containingType.Identifier.ValueText.Should().Be("MyClass");
            myMethod2.Identifier.ValueText.Should().Be("MyMethod");

        }
    }
}
