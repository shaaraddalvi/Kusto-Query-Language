using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kusto.Language;

namespace Test
{
    class Functions
    {
        public static Dictionary<string, string> processFunction_(string input)
        {

            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            //int i = 0;
            Dictionary<string, string> map = new Dictionary<string, string>();

            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n =>
            {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                {
                    string temp = "";
                    string funcName = n.GetChild(0).ToString().TrimStart().TrimEnd();
                    string funcArument = n.GetChild(1).ToString().TrimStart().TrimEnd();
                    funcArument = HelperClass.removeBrackets(funcArument).TrimStart().TrimEnd();
                    if (funcName == "tolong")
                    {
                        funcArument = HelperClass.removeBrackets(funcArument).TrimStart().TrimEnd();
                        temp += " CAST(";
                        Kusto.Language.Syntax.SyntaxElement node = Tree.CheckInTree(funcArument, "FunctionCallExpression");
                        if (node == null)
                        {
                            temp += funcArument + " as bigint ";
                        }
                        else
                        {
                            temp += processFunction(funcArument);
                            temp += " as bigint";
                        }
                        temp += ")";
                    }
                    if (funcName == "todouble")
                    {
                        funcArument = HelperClass.removeBrackets(funcArument).TrimStart().TrimEnd();
                        temp += " CAST(";
                        Kusto.Language.Syntax.SyntaxElement node = Tree.CheckInTree(funcArument, "FunctionCallExpression");
                        if (node == null)
                        {
                            temp += funcArument + " AS FLOAT ";
                        }
                        else
                        {
                            temp += processFunction(funcArument);
                            temp += " AS FLOAT ";
                        }
                        temp += ")";
                    }
                    if (funcName == "iff")
                    {
                        List<Kusto.Language.Syntax.SyntaxElement> list = Tree.SeparatedElements(n.GetChild(1));
                        string condition = list[0].ToString().TrimEnd().TrimStart();
                        condition = condition.Remove(condition.Length - 1);
                        Dictionary<string, string> temp1 = processFunction_(condition);
                        foreach (string key in temp1.Keys)
                        {
                            condition = condition.Replace(key, temp1[key]);
                        }
                        string thenStatement = list[1].ToString().TrimStart().TrimEnd();
                        thenStatement = thenStatement.Remove(thenStatement.Length - 1);
                        Dictionary<string, string> temp2 = processFunction_(thenStatement);
                        foreach (string key in temp2.Keys)
                        {
                            thenStatement = thenStatement.Replace(key, temp2[key]);
                        }
                        string elseStatement = list[2].ToString().TrimEnd().TrimStart();
                        Dictionary<string, string> temp3 = processFunction_(elseStatement);
                        foreach (string key in temp3.Keys)
                        {
                            elseStatement = elseStatement.Replace(key, temp3[key]);
                        }
                        temp += " CASE WHEN(" + condition + ")" + " THEN " + thenStatement + " ELSE " + elseStatement + " END";

                    }
                    if (funcName == "isnotnull" | funcName == "notnull")
                    {
                        Kusto.Language.Syntax.SyntaxElement node = Tree.CheckInTree(funcArument, "FunctionCallExpression");
                        if (node == null)
                        {
                            temp += funcArument + " IS NOT NULL ";
                        }
                        else
                        {
                            temp += processFunction(funcArument);
                            temp += " IS NOT NULL ";
                        }
                    }
                    if (String.Equals(funcName.TrimStart(), "max") | String.Equals(funcName.TrimStart(), "min") | String.Equals(funcName.TrimStart(), "sum") |
                        String.Equals(funcName.TrimStart(), "avg") | String.Equals(funcName.TrimStart(), "stdev") | String.Equals(funcName.TrimStart(), "variance") |
                        String.Equals(funcName.TrimStart(), "count"))
                    {

                        if (String.Equals(funcName.TrimStart(), "max")) { temp += "MAX"; }

                        if (String.Equals(funcName.TrimStart(), "min")) { temp += "MIN"; }

                        if (String.Equals(funcName.TrimStart(), "sum")) { temp += "SUM"; }

                        if (String.Equals(funcName.TrimStart(), "avg")) { temp += "AVG"; }

                        if (String.Equals(funcName.TrimStart(), "stdev")) { temp += "STDEV"; }

                        if (String.Equals(funcName.TrimStart(), "variance")) { temp += "VAR"; }

                        if (String.Equals(funcName.TrimStart(), "count"))
                        {
                            temp += "COUNT";
                        }
                        temp += "(";
                        //List<string> tokens = TokenNames(funcArument);
                        funcArument = HelperClass.removeBrackets(funcArument).TrimStart().TrimEnd();
                        Dictionary<string, string> temp1 = processFunction_(funcArument);
                        foreach (string key in temp1.Keys)
                        {
                            funcArument = funcArument.Replace(key, temp1[key]);
                        }
                        if (funcArument.TrimStart().TrimEnd() == "") temp += "*";
                        else temp += funcArument;

                        temp += ")";
                    }
                    map.Add(n.ToString(), temp);
                    temp = "";



                }
            }, n => { });
            return map;
        }

        public static string processFunction(string input)
        {
            string temp = "";
            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            int i = 0;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n =>
            {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                {
                    if (i != 0) { }
                    else
                    {
                        i++;
                        string funcName = n.GetChild(0).ToString();
                        string funcArument = n.GetChild(1).ToString();
                        if (funcName == "isdouble" | funcName == " isdouble")
                        {
                            string temp_ = processFunction(funcArument);
                            if (temp_ != "")
                            {
                                // temp += "(";
                                temp += temp_;
                                // temp += ")";
                            }
                            if (temp_ == "")
                            {
                                List<string> tokens_ = Tree.TokenNames(funcArument);

                                if (tokens_.Count == 1 & tokens_[0] == "") temp += "*";
                                else temp = HelperClass.addListElements(tokens_, temp);
                            }

                            temp += "AS FLOAT";

                        }
                        if (funcName == "max" | funcName == " max" | funcName == "min" | funcName == " min" | funcName == "sum" | funcName == " sum" |
                            funcName == "avg" | funcName == " avg" | funcName == "stdev" | funcName == " stdev" | funcName == "variance" | funcName == " variance" |
                            funcName == "count" | funcName == " count"
                            )
                        {
                            if (funcName == "max" | funcName == " max") { temp += "MAX"; }

                            if (funcName == "min" | funcName == " min") { temp += "MIN"; }

                            if (funcName == "sum" | funcName == " sum") { temp += "SUM"; }

                            if (funcName == "avg" | funcName == " avg") { temp += "AVG"; }

                            if (funcName == "stdev" | funcName == " stdev") { temp += "STDEV"; }

                            if (funcName == "variance" | funcName == " variance") { temp += "VAR"; }

                            if (funcName == "count" | funcName == " count")
                            {
                                temp += "COUNT";
                            }
                            temp += "(";
                            List<string> tokens = Tree.TokenNames(funcArument);
                            string temp1 = processFunction(funcArument);
                            if (temp1 != "") temp += temp1;
                            else
                            {
                                if (tokens.Count == 1 & tokens[0] == "") temp += "*";
                                else temp = HelperClass.addListElements(tokens, temp);
                            }
                            temp += ")";
                        }

                    }
                }
            }, n => { });
            return temp;
        }
    }
}



