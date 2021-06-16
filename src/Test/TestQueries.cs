using System;
using System.Collections.Generic;
using Kusto.Language;

namespace Test
{
    class TestQueries
    {
        public static void tree(string statement)
        {
            var query = KustoCode.Parse(statement);
            string text = query.Text;
            Console.WriteLine(text);
            var root = query.Syntax;
            GenTree.traverse(root, true);
        }
        const Kusto.Language.Syntax.SyntaxKind ProjectOperator = Kusto.Language.Syntax.SyntaxKind.ProjectOperator;
        const Kusto.Language.Syntax.SyntaxKind TakeOperator = Kusto.Language.Syntax.SyntaxKind.TakeOperator;
        const Kusto.Language.Syntax.SyntaxKind WhereOperator = Kusto.Language.Syntax.SyntaxKind.FilterOperator;
        const Kusto.Language.Syntax.SyntaxKind SummarizeOperator = Kusto.Language.Syntax.SyntaxKind.SummarizeOperator;
        const Kusto.Language.Syntax.SyntaxKind SummarizeByClause = Kusto.Language.Syntax.SyntaxKind.SummarizeByClause;
        const Kusto.Language.Syntax.SyntaxKind SortOperator = Kusto.Language.Syntax.SyntaxKind.SortOperator;
        const Kusto.Language.Syntax.SyntaxKind OrderingClause = Kusto.Language.Syntax.SyntaxKind.OrderingClause;
        const Kusto.Language.Syntax.SyntaxKind FunctionCallExpression = Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression;
        const Kusto.Language.Syntax.SyntaxKind LongLiteralExpression = Kusto.Language.Syntax.SyntaxKind.LongLiteralExpression;
        const string Add = "AddExpression";
        const string Subtract = "SubtractExpression";
        const string Multiply = "MutiplyExpression";
        const string Divide = "DivideExpression";
        const string Equal = "EqualExpression";
        const string GreaterThan = "GreaterThanExpression";
        const string ascending = "asc";
        const string descending = "desc";
        const string PipeSymbol = "PipeExpression";
        int numPipeExpression = 0;
        Boolean isProjectOperatorPresent = false;
        Boolean isTakeOperatorPresent = false;
        Boolean isWhereOperatorPresent = false;
        Boolean isSumaarizeOperatorPresent = false;
        Boolean isSumaarizeByClausePresent = false;
        Boolean isOrderByPresent = false;
        Boolean isOrderingClausePresent = false;
        Boolean isFunctionCallPresent = false;
        Boolean isPunctuationPresent = false;
        Boolean isLongLiteralPresent = false;
        Boolean isMathOperatorsPresent = false;
        string orderType = "";
        string nodeByClause = "";
        string nodeOrderByClause = "";
        List<string> allTokenNames = new List<string>();
        List<string> onlyFunctionNames = new List<string>();
        List<List<string>> allFunctioCallNames = new List<List<string>>();
        List<string> allPunctuationNames = new List<string>();
        List<string> allLongLiteralsNames = new List<string>();
        List<string> mathOperators = new List<string>();
        List<string> summarizeByColumns = new List<string>();
        List<string> orderByColumns = new List<string>();
        


        string tableName = "";  // there can be multiple table names also - will consider these cases later. 
        public string GetSqlQuery(string kqlQuery)
        {
            traverseTree(kqlQuery);
            if (allTokenNames.Count == 0) return "";
            tableName = allTokenNames[0]; // assuming single table names for now. 
            string output = "";
            if (numPipeExpression == 0)
            {
                // no keyword present eg -> "Table_name"
                output += "SELECT * FROM " + tableName;
                return output;
            }
            if (numPipeExpression == 1)
            {
                // sinle keyword present  -> need to look if more than one keywords possible in this - will see later
                if (isProjectOperatorPresent)
                {
                    string columns = "";
                    for (int i = 1; i < allTokenNames.Count; i++)
                    {
                        if (i != allTokenNames.Count - 1)
                        {
                            columns += allTokenNames[i];
                            columns += ",";
                        }
                        else
                        {
                            columns += allTokenNames[i];
                        }
                    }
                    output = "SELECT" + columns + " FROM " + tableName;
                    return output;
                }
                if (isTakeOperatorPresent)
                {
                    if (!isMathOperatorsPresent)
                    {
                        output += "SELECT TOP " + allLongLiteralsNames[0] + " * FROM " + tableName;
                    }
                    if (isMathOperatorsPresent)
                    {
                        String str = "";
                        for (int i = 0; i < allLongLiteralsNames.Count; i++)
                        {
                            if (i != allLongLiteralsNames.Count - 1) str += allLongLiteralsNames[i] + " " + mathOperators[i];
                            else str += allLongLiteralsNames[i];
                        }
                        output += "SELECT TOP " + str + " * FROM " + tableName;
                        return output;
                    }
                }
                if (isWhereOperatorPresent)
                {
                    if (allPunctuationNames.Contains(GreaterThan) & allTokenNames.Count == 2 & isLongLiteralPresent) // second condition is to check if column name is present to compare
                    {
                        output += "SELECT * FROM " + tableName + " WHERE " + allTokenNames[1] + " >" + allLongLiteralsNames[0];
                        return output;
                    }
                    if (isFunctionCallPresent & allFunctioCallNames.Count == 1 & allTokenNames.Count == 3)
                    {
                        if (allFunctioCallNames[0][0] == " isnotnull")
                        {
                            output += "SELECT * FROM " + tableName + " WHERE " + allFunctioCallNames[0][1] +" " + SQL.notNull;
                            return output;
                        }
                        // can add other functions
                    }
                    // need to add double equal punctuation case. 
                }
                
                if(isSumaarizeOperatorPresent & isFunctionCallPresent)
                {
                    output += "SELECT ";
                    if(onlyFunctionNames.Contains(" max") | onlyFunctionNames.Contains(" min") | onlyFunctionNames.Contains(" sum") | onlyFunctionNames.Contains(" stdev") | onlyFunctionNames.Contains(" variance"))
                    {
                        for(int i = 0; i < onlyFunctionNames.Count; i++)
                        {
                            if(onlyFunctionNames[i] == " max" | onlyFunctionNames[i] == "max")
                            {
                                output += "MAX" + "(" + allFunctioCallNames[i][1] + ") ";
                                if (i != onlyFunctionNames.Count - 1) output += ",";
                            }
                            if (onlyFunctionNames[i] == " min" | onlyFunctionNames[i] == "min")
                            {
                                output += "MIN" + "(" + allFunctioCallNames[i][1] + ") ";
                                if (i != onlyFunctionNames.Count - 1) output += ",";
                            }
                            if (onlyFunctionNames[i] == " sum" | onlyFunctionNames[i] == "sum")
                            {
                                output += "SUM" + "(" + allFunctioCallNames[i][1] + ") ";
                                if (i != onlyFunctionNames.Count - 1) output += ",";
                            }
                            if (onlyFunctionNames[i] == " stdev" | onlyFunctionNames[i] == "stdev")
                            {
                                output += "STDEV" + "(" + allFunctioCallNames[i][1] + ") ";
                                if (i != onlyFunctionNames.Count - 1) output += ",";
                            }
                            if (onlyFunctionNames[i] == " variance" | onlyFunctionNames[i] == "variance")
                            {
                                output += "VAR" + "(" + allFunctioCallNames[i][1] + ") ";
                                if (i != onlyFunctionNames.Count - 1) output += ",";
                            }


                        }
                        

                        //int index = onlyFunctionNames.IndexOf(" max");
                        //output += "MAX" + "(" + allFunctioCallNames[index][1] + ") " + " FROM " + tableName;
                    }
                    if (onlyFunctionNames.Contains(" avg"))
                    {
                        int index = onlyFunctionNames.IndexOf(" avg");
                        for (int i = 0; i < summarizeByColumns.Count; i++)
                        {
                            if (i == summarizeByColumns.Count - 1)
                            {
                                output += summarizeByColumns[i];
                            }
                            else output += summarizeByColumns[i] + ",";
                        }
                        var list = allFunctioCallNames[index];
                        output += " ," + SQL.average + "(";
                        for (int i = 1; i < list.Count; i++)
                        {
                            if (i == list.Count - 1)
                            {
                                output += list[i];
                            }
                            else output += list[i] + ",";
                        }
                        output += ")";
                    }
                    output += " FROM " + tableName;
                    if (isSumaarizeByClausePresent)
                    {
                        output += " GROUP BY ";
                        for(int i = 0; i < summarizeByColumns.Count; i++)
                        {
                            output += summarizeByColumns[i];
                            if (i != summarizeByColumns.Count - 1) output += ",";
                        }
                    }
                    return output;
                }
                if (isSumaarizeOperatorPresent & isSumaarizeByClausePresent & !isFunctionCallPresent)
                {
                    output += "SELECT DISTINCT ";
                    for (int i = 1; i < allTokenNames.Count; i++)
                    {
                        output += allTokenNames[i];
                        if (i != allTokenNames.Count - 1) output += ",";
                    }
                    output += " FROM " + tableName;
                    return output;
                }
                /*if (isSumaarizeOperatorPresent & isSumaarizeByClausePresent & isFunctionCallPresent)
                {
                    output += "SELECT ";
                    if (onlyFunctionNames.Contains(" avg"))
                    {
                        int index = onlyFunctionNames.IndexOf(" avg");
                        for (int i = 0; i < summarizeByColumns.Count; i++)
                        {
                            if (i == summarizeByColumns.Count - 1)
                            {
                                output += summarizeByColumns[i];
                            }
                            else output += summarizeByColumns[i] + ",";
                        }
                        var list = allFunctioCallNames[index];
                        output += " ," +  SQL.average + "(";
                        for (int i = 1; i < list.Count; i++)
                        {
                            if (i == list.Count - 1)
                            {
                                output += list[i];
                            }
                            else output += list[i] + ",";
                        }
                        output += ") FROM " + tableName;
                        output += " GROUP BY ";
                        for (int i = 0; i < summarizeByColumns.Count; i++)
                        {
                            if (i == summarizeByColumns.Count - 1)
                            {
                                output += summarizeByColumns[i];
                            }
                            else output += summarizeByColumns[i] + ",";
                        }
                    }
                    return output;
                }*/
            }
            if (numPipeExpression == 2)
            {
                if (isProjectOperatorPresent & isOrderByPresent)
                {
                    // project query part is called
                    int result = kqlQuery.LastIndexOf('|');// find index position of second '|',then create a substring of it, and call project query
                    string subProjectPart = kqlQuery.Substring(0, result);
                    TestQueries t = new TestQueries();
                    output += t.GetSqlQuery(subProjectPart);
                    output += " ORDER BY ";
                    for (int i = 0; i < orderByColumns.Count; i++)
                    {
                        if (i == orderByColumns.Count - 1)
                        {
                            output += orderByColumns[i];
                        }
                        else output += orderByColumns[i] + ",";
                    }
                    if (orderType == descending) output += " DESC";
                    if (orderType == ascending) output += " ASC";
                    return output;
                }
            }
            return output;
        }
        public void traverseTree(string kqlQuery)
        {
            var query = KustoCode.Parse(kqlQuery);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {

                if (n.Kind.ToString() == PipeSymbol)
                {
                    numPipeExpression++;
                }
                if (n.Kind == ProjectOperator)
                {
                    isProjectOperatorPresent = true;
                }
                if (n.Kind == TakeOperator)
                {
                    isTakeOperatorPresent = true;
                }
                if (n.Kind == WhereOperator)
                {
                    isWhereOperatorPresent = true;
                }
                if (n.Kind == SummarizeOperator)
                {
                    isSumaarizeOperatorPresent = true;
                }
                if (n.Kind == SummarizeByClause)
                {
                    isSumaarizeByClausePresent = true;
                    nodeByClause = n.ToString();
                    //summarizeByColumns = TokenNames(n.ToString());
                }
                if (n.Kind == SortOperator)
                {
                    isOrderByPresent = true;
                    orderByColumns = TokenNames(n.ToString());
                }
                if (n.Kind == OrderingClause)
                {
                    isOrderingClausePresent = true;
                    string str = n.ToString();
                    if (str.Contains(descending)) orderType = descending;
                    if (str.Contains(ascending)) orderType = ascending;
                }
                if (n.Kind == FunctionCallExpression)
                {
                    isFunctionCallPresent = true;
                    var list = TokenNames(n.ToString());
                    allFunctioCallNames.Add(list);
                    onlyFunctionNames.Add(list[0]);
                }
                if (n.Kind.ToString() == Equal | n.Kind.ToString() == GreaterThan)
                {
                    isPunctuationPresent = true;
                    allPunctuationNames.Add(n.Kind.ToString());
                }
                if (n.Kind == LongLiteralExpression)
                {
                    isLongLiteralPresent = true;
                    allLongLiteralsNames.Add(n.ToString());
                }
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.TokenName)
                {
                    allTokenNames.Add(n.ToString());
                }
                if (n.Kind.ToString() == Add | n.Kind.ToString() == Subtract | n.Kind.ToString() == Multiply | n.Kind.ToString() == Divide)
                {
                    isMathOperatorsPresent = true;
                    if (n.Kind.ToString() == Add) mathOperators.Add("+");
                    else if (n.Kind.ToString() == Subtract) mathOperators.Add("-");
                    else if (n.Kind.ToString() == Multiply) mathOperators.Add("*");
                    else if (n.Kind.ToString() == Divide) mathOperators.Add("/");
                }
            }, n => { });
            var tokens = Kusto.Language.Parsing.TokenParser.ParseTokens(nodeByClause, false);
            for (int i = 0; i < tokens.Length; i++)
            {
                string temp = " " + tokens[i].Text.ToString();
                if (allTokenNames.Contains(temp))
                {
                    summarizeByColumns.Add(tokens[i].Text.ToString());
                }
            }

        }






        /* public static Boolean checkNoKeyword(string input_string)
         {
             var query = KustoCode.Parse(input_string);
             var root = query.Syntax;
             Boolean noKeywordPresent = true;
             Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {

                 if (n.Kind.ToString() == "PipeExpression" )
                 {
                     noKeywordPresent = false;

                 }
             }, n => { });
             return noKeywordPresent;

         }*/

        public static Boolean CheckInTree(string input_query, string findKind)
        {
            // It will help me generalize for looking for any kind of operator in tree. 
            var query = KustoCode.Parse(input_query);
            var root = query.Syntax;
            Boolean checkIfPresent = false;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {

                if (n.Kind.ToString() == findKind)
                {
                    checkIfPresent = true;

                }
            }, n => { });
            return checkIfPresent;

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



 






    }
}