using Microsoft.CodeAnalysis.CSharp;
using System;
using Microsoft.CodeAnalysis;
using Xunit;
using System.Text;

namespace LearnRoslynNow.Tests
{
    public class CustomWalker : CSharpSyntaxWalker
    {
        static int _tabs = 0;
        StringBuilder _value = new StringBuilder();
        public override void Visit(SyntaxNode node)
        {
            _tabs++;
            var indents = new String('\t', _tabs);
            var kind = node.Kind();
            _value.Append(indents + kind + "\n");
            base.Visit(node);
            _tabs--;
        }

        public string GetValue() => _value.ToString();

    }

    public class DeeperWalker : CSharpSyntaxWalker
    {
        static int _tabs = 0;
        StringBuilder _value = new StringBuilder();
        public DeeperWalker(): base(SyntaxWalkerDepth.Token)
        {

        }

        public override void Visit(SyntaxNode node)
        {
            _tabs++;
            var indents = new string('\t', _tabs);
            _value.Append(indents + node.Kind() + '\n');
            base.Visit(node);
            _tabs--;
        }

        public override void VisitToken(SyntaxToken token)
        {
            var indents = new string('\t', _tabs);
            _value.Append(indents + token + '\n');
            base.VisitToken(token);
        }

        public string GetValue() => _value.ToString();

    }

    public class Part4CSharpSyntaxWalker
    {
        string _source = @"
public class MyClass {
    public void MyMethod() {}
    public void MyMethod(int n) {}
} ";
        [Fact]
        public void Start() { 
            var tree = CSharpSyntaxTree.ParseText(_source );
            var walker = new CustomWalker();
            walker.Visit(tree.GetRoot());

            var value = walker.GetValue();

        }

        [Fact]
        public void Deep()
        {
            var tree = CSharpSyntaxTree.ParseText(_source);
            var walker = new DeeperWalker();
            walker.Visit(tree.GetRoot());
            var value = walker.GetValue();

        }
    }
}
