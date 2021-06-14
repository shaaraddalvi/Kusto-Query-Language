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
                // Need to add a condition that only these two are present and no other operator present
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
                if (isSumaarizeOperatorPresent & isSumaarizeByClausePresent & isFunctionCallPresent)
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
                }
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



        /* public static Boolean checkProjectOperator(string input_string)
         {
             var query = KustoCode.Parse(input_string);
             var root = query.Syntax;
             Boolean isprojectQuery = false;
             Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                 if (n.Kind == Kusto.Language.Syntax.SyntaxKind.ProjectOperator)
                 {
                     isprojectQuery = true;
                 }
                 }, n => {});
             return isprojectQuery;
         }

         public static Boolean checkTakeOperator(string input_string)
         {
             var query = KustoCode.Parse(input_string);
             var root = query.Syntax;
             Boolean isTakeQuery = false;
             Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                 if (n.Kind == Kusto.Language.Syntax.SyntaxKind.TakeOperator)
                 {
                     isTakeQuery = true;
                 }
             }, n => { });
             return isTakeQuery;
         }

         public static Boolean checkWhereOperator(string input_string)
         {
             var query = KustoCode.Parse(input_string);
             var root = query.Syntax;
             Boolean isWhereQuery = false;
             Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                 if (n.Kind == Kusto.Language.Syntax.SyntaxKind.FilterOperator)
                 {
                     isWhereQuery = true;
                 }
             }, n => { });
             return isWhereQuery;
         } */

        /* public static string noKeyword(string input_string)
         {
             var query = KustoCode.Parse(input_string);
             var root = query.Syntax;
             string tableName = "";
             string output = "";
             Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                 if (n.Kind == Kusto.Language.Syntax.SyntaxKind.TokenName)
                 {
                     tableName = n.ToString();
                 }
             }, n => { });
             output += "SELECT * FROM " + tableName;
             Console.WriteLine(output);
             return output;
         }

         public static string project_query(string statement)
         {
             //SELECT T, a FROM
             // traverse tree using walkNodes in that put if condition that if kind is project operator, then put a
             // boolean flag , if flag true then search for table names and column names- by looking at their kinds.
             // Need to add a condition that number of operators is just one.

             var query = KustoCode.Parse(statement);
             string text = query.Text;
             //Console.WriteLine(text);
             var root = query.Syntax;
             Boolean isprojectQuery = false;
             Boolean first_tokenName = true;
             string tableName = "";
             List<string> columnNames = new List<string>();
             Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                 if (n.Kind == Kusto.Language.Syntax.SyntaxKind.ProjectOperator)
                 {
                     isprojectQuery = true;
                 }
                 if(n.Kind == Kusto.Language.Syntax.SyntaxKind.TokenName)
                 {
                     if(first_tokenName)
                     {
                         tableName = n.ToString();
                         first_tokenName = false;
                     }
                     else
                     {
                         columnNames.Add(n.ToString());
                     }


                 }


             },n => {

             });
             if (!isprojectQuery) { return "Not a project operator"; }
             else
             {
                 string columns = "";
                 for(int i = 0; i < columnNames.Count; i++)
                 {
                     if(i != columnNames.Count-1)
                     {
                         columns += columnNames[i];
                         columns += ",";
                     }
                     else
                     {
                         columns += columnNames[i];
                     }

                 }
                 string output = "SELECT" + columns + " FROM " + tableName;
                 Console.WriteLine(output);
                 return output;
                 // better to create a string and return it. 
             } 



         }
         public static void take_query(string statement)
         {
             //T | take 50 //AddExpression, 
             var query = KustoCode.Parse(statement);
             string text = query.Text;
             //Console.WriteLine(text);
             var root = query.Syntax;
             Boolean isTakeQuery = false;
             string tableName = "";
             Boolean isOperator = false;
             string output = "";
             List<String> operators = new List<string>();
             List<string> literals = new List<string>();
             Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                 if (n.Kind == Kusto.Language.Syntax.SyntaxKind.TakeOperator)
                 {
                     isTakeQuery = true;
                 }
                 if (n.Kind == Kusto.Language.Syntax.SyntaxKind.TokenName)
                 {
                     tableName = n.ToString();
                 }
                 if (n.Kind == Kusto.Language.Syntax.SyntaxKind.LongLiteralExpression)
                 {

                     literals.Add(n.ToString());
                 }
                 if(n.Kind.ToString()== "AddExpression" | n.Kind.ToString() == "SubtractExpression"| n.Kind.ToString() == "MultiplyExpression"
                 | n.Kind.ToString() == "DivideExpression")
                 {
                     if(n.Kind.ToString() == "AddExpression")  operators.Add("+");
                     else if (n.Kind.ToString() == "SubtractExpression") operators.Add("-");
                     else if (n.Kind.ToString() == "MultiplyExpression") operators.Add("*");
                     else if (n.Kind.ToString() == "DivideExpression") operators.Add("/");

                     isOperator = true;
                 }


             }, n => {

             });
             if(!isOperator)
             {
                 output += "SELECT TOP " + literals[0] + " * FROM " + tableName;
                 Console.WriteLine(output);
             }
             if(isOperator)
             {
                 String str = "";
                 for(int i = 0; i < literals.Count; i++)
                 {
                     if (i != literals.Count - 1) str += literals[i] +" "+ operators[i];
                     else str += literals[i];
                 }
                 output += "SELECT TOP " + str + " * FROM " + tableName;
                 Console.WriteLine(output);
             }


         } */

        /* public static string where_query(string input)
         {
             var query = KustoCode.Parse(input);

             var root = query.Syntax;
             string tableName = "";
             Boolean isDoubleEqualPunctuation = false;
             Boolean isGreaterThanExpression = false;
             string columnName = "";
             Boolean isColumn = false;
             string longLiteralExpression = "";
             Boolean isLiteral = false;
             Boolean firstToken = true;
             Boolean secondToken = true;
             String output = "";
             List<string> functionsList = new List<string>();
             List<string> columns = new List<string>();
             Boolean isFunction = false;
             //string timeSpanLiteralExpression = "";

             Kusto.Language.Syntax.SyntaxElement.WalkNodes(
                 root,
                 fnBefore: n =>
                 {
                     if (n.Kind.ToString() == "EqualExpression")
                     {
                         isDoubleEqualPunctuation = true;

                     }
                     if (n.Kind.ToString() == "GreaterThanExpression")
                     {
                         isGreaterThanExpression = true;

                     }
                     if (n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                     {
                         isFunction = true;


                     }


                     if (n.Kind == Kusto.Language.Syntax.SyntaxKind.TokenName)
                     {
                         if (firstToken)
                         {
                             tableName = n.ToString();
                             firstToken = false;
                         }
                         else if(!isFunction)
                         {
                             columnName = n.ToString(); isColumn = true;
                         }
                         else if(isFunction)
                         {
                             if (secondToken)
                             {
                                 functionsList.Add(n.ToString()); secondToken = false;
                             }
                             else columns.Add(n.ToString());

                         }
                     }
                     if (n.Kind == Kusto.Language.Syntax.SyntaxKind.LongLiteralExpression)
                     {
                         longLiteralExpression = n.ToString(); isLiteral = true;
                     }


                 },
                 fnAfter: n => { } );

             if (isDoubleEqualPunctuation & isColumn & isLiteral )
             {
                 output += "SELECT * FROM " + tableName + " WHERE " + columnName + " == " + longLiteralExpression;
                 Console.WriteLine(output);
             }
             if (isGreaterThanExpression & isColumn & isLiteral)
             {
                 output += "SELECT * FROM " + tableName + " WHERE " + columnName + " >" + longLiteralExpression;
                 Console.WriteLine(output);
             }
             if (isFunction & functionsList.Count == 1 & columns.Count == 1)
             {
                 if(functionsList[0] == " isnotnull")
                 {
                     output += "SELECT * FROM " + tableName + " WHERE " + columns[0] + " IS NOT NULL";
                     Console.WriteLine(output);
                 }


             }
             return output;

         } */

        /* public static string SummarizeQueries(string input_query)
         {
             Boolean isSummarizePresent = CheckInTree(input_query, "SummarizeOperator");
             Boolean isSummarizeByPresent = CheckInTree(input_query, "SummarizeByClause");
             var tokenNames = TokenNames(input_query);
             string tableName = "";
             string output = "";
             if (tokenNames.Count >= 1) tableName = tokenNames[0];
             // Need to add a condition that only these two are present and no other operator present
             if(isSummarizePresent & isSummarizeByPresent)
             {
                 output = "SELECT DISTINCT ";
                 for(int i = 1; i < tokenNames.Count; i++)
                 {
                     output += tokenNames[i];
                     if (i != tokenNames.Count - 1) output += ",";
                 }
                 output += " FROM " + tableName;
             }
             if(isSummarizePresent & isSummarizeByPresent & )
             return output;
         }*/








    }
}