using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class JoinOperator
    {
        public static List<List<string>> onInfoJoin(Kusto.Language.Syntax.SyntaxElement node)
        {
            List<List<string>> output = new List<List<string>>();
            Kusto.Language.Syntax.SyntaxElement left;
            Kusto.Language.Syntax.SyntaxElement right;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes((Kusto.Language.Syntax.SyntaxNode)node, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.EqualExpression)
                {
                    left = n.GetChild(0);
                    right = n.GetChild(2);
                    output.Add(Tree.TokenNames(left.ToString()));
                    output.Add(Tree.TokenNames(right.ToString()));


                }
                

            }, n => { });
            return output;
        }
    }
}
