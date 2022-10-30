using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stryker.Core.Mutators
{
    /// <summary>
    /// Mutator that will mutate the access to <c>string.Empty</c> to a string that is not empty.
    /// </summary>
    /// <remarks>
    /// Will only apply the mutation to the lowercase <c>string</c> since that is a reserved keyword in c# and can be distinguished from any variable or member access.
    /// </remarks>
    public class StringEmptyNullMutator : MutatorBase<InvocationExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        /// <inheritdoc />
        public override IEnumerable<Mutation> ApplyMutations(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax)
            {
                var childNode = (MemberAccessExpressionSyntax) node.ChildNodes().FirstOrDefault();

                if(childNode.Expression is PredefinedTypeSyntax typeSyntax &&
                    typeSyntax.Keyword.ValueText == "string" &&
                    childNode.Name.Identifier.ValueText == nameof(string.IsNullOrEmpty))
                {
                    yield return new Mutation{
                        OriginalNode = node,
                        ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression,
                        node.ArgumentList.Arguments.First().Expression,
                        SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                        DisplayName = "String mutation",
                        Type = Mutator.String
                    };
                }
            }
        }
    }
}
