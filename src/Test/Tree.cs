using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kusto.Language;

namespace Test
{
    class Tree
    {
        const string Add = "AddExpression";
        const string Subtract = "SubtractExpression";
        const string Multiply = "MultiplyExpression";
        const string Divide = "DivideExpression";
        public static void tree(string statement)
        {
            var query = KustoCode.ParseAndAnalyze(statement);
            string text = query.Text;
            //Console.WriteLine(text);
            var root = query.Syntax;
            GenTree.traverse(root, true);
        }
        public static Kusto.Language.Syntax.SyntaxElement CheckInTree(string input_query, string findKind)
        {
            // It will help me generalize for looking for any kind of operator in tree. 
            Kusto.Language.Syntax.SyntaxElement node = null;
            var query = KustoCode.Parse(input_query);
            var root = query.Syntax;
            int i = 0;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {

                if (i == 0 & n.Kind.ToString() == findKind)
                {
                    node = n;
                    i++;
                }
            }, n => { });
            return node;

        }
        public static Kusto.Language.Syntax.SyntaxElement CheckInTree(Kusto.Language.Syntax.SyntaxElement root_node, string findKind)
        {
            // It will help me generalize for looking for any kind of operator in tree. 
            Kusto.Language.Syntax.SyntaxElement node = null;
            int i = 0;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes((Kusto.Language.Syntax.SyntaxNode)root_node, n => {

                if (i == 0 & n.Kind.ToString() == findKind)
                {
                    node = n;
                    i++;
                }
            }, n => { });
            return node;

        }
        public static List<string> TokenNames(Kusto.Language.Syntax.SyntaxElement root)
        {
            List<string> tokenNames = new List<string>();
            Kusto.Language.Syntax.SyntaxElement.WalkNodes((Kusto.Language.Syntax.SyntaxNode)root, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.TokenName)
                {
                    tokenNames.Add(n.ToString());
                }
            }, n => { });
            return tokenNames;
        }
        public static List<string> TokenNames(string input_query)
        {
            List<string> tokenNames = new List<string>();
            var query = KustoCode.Parse(input_query);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.TokenName)
                {
                    tokenNames.Add(n.ToString());
                }
            }, n => { });
            return tokenNames;
        }
        public static List<string> LiteralValues(string input_query)
        {
            List<string> literalValues = new List<string>();
            var query = KustoCode.Parse(input_query);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.LongLiteralExpression)
                {
                    literalValues.Add(n.ToString());
                }
            }, n => { });
            return literalValues;
        }
        public static List<Kusto.Language.Syntax.SyntaxElement> SeparatedElements(Kusto.Language.Syntax.SyntaxElement node)
        {
            List<Kusto.Language.Syntax.SyntaxElement> list = new List<Kusto.Language.Syntax.SyntaxElement>();
            Kusto.Language.Syntax.SyntaxElement.WalkNodes((Kusto.Language.Syntax.SyntaxNode)node, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.SeparatedElement)
                {
                    list.Add(n);
                }


            }, n => { });
            return list;
        }
        public static Boolean checkAggregateFunction(string input)
        {
            string[] aggregateFunctions = { "max", "min", "sum", "count", "avg", "stdev", "variance" };
            Boolean t = false;
            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            int j = 0;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if (n.Kind.ToString() == "FunctionCallExpression")
                {
                    if (j == 0)
                    {
                        string funcName = n.GetChild(0).ToString();
                        for (int i = 0; i < aggregateFunctions.Length; i++)
                        {
                            if (funcName.TrimStart() == aggregateFunctions[i]) t = true;
                        }
                    }
                    else j++;
                }
            }, n => { });
            return t;
        }
        public static List<string> MathOperators(string input_query)
        {
            List<string> mathOperators = new List<string>();
            var query = KustoCode.Parse(input_query);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if (n.Kind.ToString() == Add) mathOperators.Add("+");
                if (n.Kind.ToString() == Subtract) mathOperators.Add("-");
                if (n.Kind.ToString() == Multiply) mathOperators.Add("*");
                if (n.Kind.ToString() == Divide) mathOperators.Add("/");
            }, n => { });
            return mathOperators;
        }
    }
}
