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
            }, n => {
                --depth;
                Console.WriteLine(n.ToString() + "\t[" + depth + "]\t\t" + (kind?n.Kind:"") + "\t" + (refSymb?n.ReferencedSymbol:""));
            });
        }
    }
}