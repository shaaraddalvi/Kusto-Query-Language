using System;
using Kusto.Language;

namespace Test
{
    class GenTree
    {
        public static void traverse(Kusto.Language.Syntax.SyntaxNode root, bool kind = false, bool refSymb = false)
        {
            int depth = 0;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                ++depth;
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.AndExpression)
                {
                    Console.WriteLine("Preeti" + n.GetChild(0) + " " + n.GetChild(1) + " " + n.GetChild(2));
                }
            }, n => {
                --depth;
                Console.WriteLine(n.ToString() + "\t[" + depth + "]\t\t" + (kind ? n.Kind : "") + "\t" +n.ChildCount+
                    "\t" +" " + (refSymb ? n.ReferencedSymbol : ""));
                if(n.ChildCount == 4)
                {
                    Console.WriteLine( n.GetChild(0) + "First "+ " "+ n.GetChild(1)+"Mid" + n.GetChild(2) + n.GetChild(3));
                }
                
                

            });
        }
    }
}