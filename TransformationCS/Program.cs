using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;

namespace TransformationCS
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = CreateTestCompilation();
            foreach (var sourceTree in test.SyntaxTrees)
            {
                var model = test.GetSemanticModel(sourceTree);
                var rewriter = new TypeInferenceRewriter(model);
                var newSource = rewriter.Visit(sourceTree.GetRoot());
                if (newSource != sourceTree.GetRoot())
                {
                    File.WriteAllText(sourceTree.FilePath, newSource.ToFullString());
                }
            }
        }

        private static Compilation CreateTestCompilation()
        {
            var x = 100;
            var y = 200;
            var z = 300;

            var programPath = @"..\..\Program.cs";
            var programText = File.ReadAllText(programPath);
            var programTree = CSharpSyntaxTree.ParseText(programText)
                .WithFilePath(programPath);
            var rewriterPath = @"..\..\TypeInferenceRewriter.cs";
            var rewriterText = File.ReadAllText(rewriterPath);
            var rewriterTree = CSharpSyntaxTree.ParseText(rewriterText)
                .WithFilePath(rewriterPath);
            SyntaxTree[] sourceTrees = { programTree, rewriterTree };

            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var codeAnalysis = MetadataReference.CreateFromFile(typeof(SyntaxTree).Assembly.Location);
            var csharpCodeAnalysis = MetadataReference.CreateFromFile(typeof(CSharpSyntaxTree).Assembly.Location);

            MetadataReference[] references = { mscorlib, codeAnalysis, csharpCodeAnalysis };

            return CSharpCompilation.Create("TransformationCS",
                sourceTrees,
                references,
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));

        }
    }
}
