using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kusto.Language;

namespace Test
{
    class WhereOperator : SqlClause
    {
        Boolean isPresent = false;
        List<List<string>> allWhereExpressions;
        List<List<string>> allWhereAndOr;
        public WhereOperator(Boolean isPresent, List<List<string>> allWhereExpressions, List<List<string>> allWhereAndOr)
        {
            this.allWhereExpressions = allWhereExpressions;
            this.allWhereAndOr = allWhereAndOr;
            this.isPresent = isPresent;
        }

        public override string Process()
        {
            string output = "";
            for (int i = 0; i < allWhereExpressions.Count; i++)
            {
                if (allWhereExpressions[i].Count == 0)
                {
                    allWhereExpressions.RemoveAt(i);
                    allWhereAndOr.RemoveAt(i);
                }
            }
            if (isPresent & allWhereExpressions.Count > 0)
            {
                output += " WHERE ";
                for (int i = 0; i < allWhereAndOr.Count; i++)
                {
                    string t = where(allWhereAndOr[i], allWhereExpressions[i]);
                    output += t;
                    if (i != allWhereAndOr.Count - 1) output += " AND ";
                }
            }
            return output;
        }
        public string where(List<string> whereAndOr, List<string> WhereExpressions)
        {
            //string temp = " WHERE ";
            string temp = "";
            whereAndOr.Reverse();
            WhereExpressions.Reverse();
            for (int i = 0; i < WhereExpressions.Count; i++)
            {
                temp += whereConvertExpressions(WhereExpressions[i]);
                if (i != WhereExpressions.Count - 1)
                {
                    temp += " " + whereAndOr[i];
                }
            }
            return temp;
        }


        public string whereConvertExpressions(string input)
        {

            // for function calls in the where operator
            if (Tree.CheckInTree(input, "FunctionCallExpression") != null)
            {
                Dictionary<string, string> map = Functions.processFunction_(input);
                foreach (string key in map.Keys)
                {
                    input = input.Replace(key, map[key]);
                }
                input = input.Replace("==", "=");
                return input;
            }

            string temp = "";
            if (Tree.CheckInTree(input, "InExpression") != null)
            {
                //replace that with IN
                Kusto.Language.Syntax.SyntaxElement node = Tree.CheckInTree(input, "InExpression");
                temp += node.GetChild(0).ToString();
                temp += " IN ";
                string str = node.GetChild(2).ToString();
                Dictionary<string, string> map = Functions.processFunction_(str);
                foreach (string key in map.Keys)
                {
                    str = input.Replace(key, map[key]);
                }
                temp += str;
                temp = temp.Replace("==", "=");
                return temp;
            }
            if (Tree.CheckInTree(input, "NotInExpression") != null)
            {
                //replace that with NOT IN
                Kusto.Language.Syntax.SyntaxElement node = Tree.CheckInTree(input, "NotInExpression");
                temp += node.GetChild(0).ToString();
                temp += " NOT IN ";
                string str = node.GetChild(2).ToString();
                Dictionary<string, string> map = Functions.processFunction_(str);
                foreach (string key in map.Keys)
                {
                    str = input.Replace(key, map[key]);
                }
                temp += str;
                temp = temp.Replace("==", "=");
                return temp;
            }
            if (Tree.CheckInTree(input, "InCsExpression") != null)
            {
                //replace that with NOT IN
                Kusto.Language.Syntax.SyntaxElement node = Tree.CheckInTree(input, "InCsExpression");
                temp += node.GetChild(0).ToString();
                temp += " NOT IN ";
                temp += node.GetChild(2).ToString();
                //input = input.Replace("==", "=");
                return temp;
            }
            input = input.Replace("==", "=");
            return input;
            // Need to work more on it



        }
        public static List<string> whereInfoAboutAndOr(string input)
        {
            List<string> whereAndOr = new List<string>();
            if ((!input.Contains("and")) & (!input.Contains("or")))
            {
                string temp = input.Remove(0, 6);

            }
            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n =>
            {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.AndExpression)
                {
                    whereAndOr.Add("AND");

                }
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.OrExpression)
                {
                    whereAndOr.Add("OR");

                }
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                {

                }


            }, n => { });
            return whereAndOr;

        }
        public static  List<string> whereInfoExpressions(string input)
        {
            input = input.TrimStart();
            List<string> whereExpressions = new List<string>();

            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.AndExpression)
                {

                    whereExpressions.Add(n.GetChild(2).ToString());
                    if ((!n.GetChild(0).ToString().Contains("and")) & (!n.GetChild(0).ToString().Contains("or")))
                    {
                        whereExpressions.Add(n.GetChild(0).ToString());
                    }
                }
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.OrExpression)
                {

                    whereExpressions.Add(n.GetChild(2).ToString());
                    if ((!n.GetChild(0).ToString().Contains("and")) & (!n.GetChild(0).ToString().Contains("or")))
                    {
                        whereExpressions.Add(n.GetChild(0).ToString());
                    }
                }
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                {

                }


            }, n => { });
            if (whereExpressions.Count == 0)
            {
                string temp = input.Remove(0, 6);
                whereExpressions.Add(temp);
            }
            return whereExpressions;
        }
    }
}
