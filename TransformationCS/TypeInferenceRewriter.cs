using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TransformationCS
{
    class TypeInferenceRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel SemanticModel;
        public TypeInferenceRewriter(SemanticModel semanticModel)
        {
            this.SemanticModel = semanticModel;
        }

        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            if (node.Declaration.Variables.Count > 1)
            {
                return node;
            }

            if (node.Declaration.Variables[0].Initializer == null)
            {
                return node;
            }

            var declarator = node.Declaration.Variables.First();
            var variableTypeName = node.Declaration.Type;
            var variableType = (ITypeSymbol)SemanticModel.GetSymbolInfo(variableTypeName).Symbol;


            var initializerInfo = SemanticModel.GetTypeInfo(declarator.Initializer.Value);
            if (variableType == initializerInfo.Type)
            {
                var varTypeName = IdentifierName("var")
                    .WithLeadingTrivia(variableTypeName.GetLeadingTrivia())
                    .WithLeadingTrivia(variableTypeName.GetTrailingTrivia());
                return node.ReplaceNode(variableTypeName, varTypeName);
            }
            else { return node; }
        }
    }
}
