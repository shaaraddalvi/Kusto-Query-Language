using System;
using System.Collections.Generic;
using Kusto.Language;

namespace Test
{
    class TestQueries
    {
        public static void tree(string statement)
        {
            var query = KustoCode.ParseAndAnalyze(statement);
            string text = query.Text;
            Console.WriteLine(text);
            var root = query.Syntax;
            GenTree.traverse(root, true);
        }
        /// <summary>
        /// Suggestions --> 
        /// 1. Can make a general function(input , typeKind) return list 
        /// </summary>
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
        const string Multiply = "MultiplyExpression";
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
        Dictionary<string, List<string>> hash_Select = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> hash_From = new Dictionary<string, List<string>>();
        List<string> projectColumns = new List<string>();
        List<string> takeLiterals = new List<string>();
        List<string> takeMathOperators = new List<string>();
        List<List<string>> allWhereAndOr = new List<List<string>>();
        List<List<string>> allWhereExpressions = new List<List<string>>();


        string tableName = "";  // there can be multiple table names also - will consider these cases later. 
        
       

        public void traverseTree(string kqlQuery)
        {
            var query = KustoCode.ParseAndAnalyze(kqlQuery);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {

                if (n.Kind.ToString() == PipeSymbol)
                {
                    numPipeExpression++;
                }
                if (n.Kind == ProjectOperator)
                {
                    isProjectOperatorPresent = true;
                    projectColumns = TokenNames(n.ToString());
                    hash_Select.Add("project", projectColumns);
                }
                if (n.Kind == TakeOperator)
                {
                    isTakeOperatorPresent = true;
                    takeLiterals = LiteralValues(n.ToString());
                    hash_Select.Add("takeLiterals", takeLiterals);
                    takeMathOperators = MathOperators(n.ToString());
                    hash_Select.Add("takeMathOperators", takeMathOperators);

                }
                if (n.Kind == WhereOperator)
                {
                    isWhereOperatorPresent = true;
                   // this will be a reverse list.
                    allWhereAndOr.Add(whereInfo(n.ToString()));

                    allWhereExpressions.Add(whereInfoExpressions(n.ToString()));

                }
                if (n.Kind == SummarizeOperator)
                {
                    isSumaarizeOperatorPresent = true;
                    hash_Select.Add("summarize", summarizeGetFunctions(n.ToString()) );// list of all fucntion calls
                }
                if (n.Kind == SummarizeByClause)
                {
                    isSumaarizeByClausePresent = true;
                    nodeByClause = n.ToString();
                    summarizeByColumns = TokenNames(n.ToString());
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
                /*if (n.Kind.ToString() == Add | n.Kind.ToString() == Subtract | n.Kind.ToString() == Multiply | n.Kind.ToString() == Divide)
                {
                    isMathOperatorsPresent = true;
                    if (n.Kind.ToString() == Add) mathOperators.Add("+");
                    else if (n.Kind.ToString() == Subtract) mathOperators.Add("-");
                    else if (n.Kind.ToString() == Multiply) mathOperators.Add("*");
                    else if (n.Kind.ToString() == Divide) mathOperators.Add("/");
                }*/
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






       
        public Boolean CheckInTree(string input_query, string findKind)
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
        public List<string> TokenNames(string input_query)
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
        public List<string> LiteralValues(string input_query)
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

        public List<string> whereInfo(string input)
        {
            List<string> whereAndOr = new List<string>();
            if((!input.Contains("and")) & (!input.Contains("or")))
            {
                string temp = input.Remove(0 , 6);
               
            }
            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.AndExpression)
                {
                    whereAndOr.Add("AND");
                    
                }
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.OrExpression)
                {
                    whereAndOr.Add("OR");
                  
                }
                if(n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                {

                }
                
                
            }, n => { });
            return whereAndOr;
        }
        public List<string> whereInfoExpressions(string input)
        {
            List<string> whereExpressions = new List<string>();
            if ((!input.Contains("and")) & (!input.Contains("or")))
            {
                string temp = input.Remove(0, 6);
                whereExpressions.Add(temp);
            }
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
            return whereExpressions;
        }

        public List<string> summarizeGetFunctions(string input)
        {
            List<string> list = new List<string>();
            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if(n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                {
                    list.Add(n.ToString());
                }
            }, n => { });
            return list;
        }
        
        public List<string> MathOperators(string input_query)
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


        List<string> selectData = new List<string>();
        // Dictionary<string, List<string> > hash_Select = 
        List<string> fromData = new List<string>();
        List<string> whereData = new List<string>();
        List<string> groupByData = new List<string>();
        public string gettingSqlQueryNew(string kqlQuery)
        {
            traverseTree(kqlQuery);
            fromData.Add(allTokenNames[0]);
            string output = "";
            if (hash_Select.Count == 0)
            {
                output += "SELECT";
                output += " ";
                // no specified columns to select => select all columns G
                output += "*";
            }
            if (hash_Select.Count != 0)
            {
                // * can be there in case of take operator // G
                output += "SELECT";
                if (hash_Select.ContainsKey("takeLiterals"))
                {
                    output += " TOP ";
                    List<string> take_Literals = hash_Select.GetValueOrDefault("takeLiterals");
                    List<string> take_mathOperators = hash_Select.GetValueOrDefault("takeMathOperators");
                    if(take_mathOperators.Count == 0)
                    {
                        output = addListElements(take_Literals, output);
                    }
                    else
                    {
                        for (int i = 0; i < take_Literals.Count; i++)
                        {
                            if (i != take_Literals.Count - 1)
                                output += take_Literals[i] + take_mathOperators[i];
                            else output += take_Literals[i];
                        }
                    }   
                    
                
                }
                if (hash_Select.ContainsKey("project"))
                {
                    List<string> project_Columns = hash_Select.GetValueOrDefault("project");
                    output = addListElements(project_Columns, output);
                    
                }
                if(hash_Select.ContainsKey("summarize"))
                {
                    List<string> summarizeFunctions = hash_Select.GetValueOrDefault("summarize");
                    // check if by is present - if not --> "DISTINCT", else simply add functions
                    // for now I am using summarizeBy columns list, I can change if iI want later
                    if(summarizeFunctions.Count == 0)
                    {
                        output += " DISTINCT ";
                        output = addListElements(summarizeByColumns, output);
                    }
                    else
                    {
                        output += " ";
                        if (summarizeByColumns.Count != 0)output = addListElements(summarizeByColumns, output);
                        
                        for(int i = 0; i < summarizeFunctions.Count; i++ )
                        {
                            output += ",";
                            output += processFunction(summarizeFunctions[i]);
                            
                        }
                        /*if(summarizeByColumns.Count != 0)
                        {
                            output += " GROUP BY ";
                            output = addListElements(summarizeByColumns, output);
                        }*/
                        
                    }
                }
                // add other also if possible
                if((!hash_Select.ContainsKey("project")) & (!hash_Select.ContainsKey("summarize"))) // add other conditions if possible
                {
                    output += " * ";
                }


            }
            if (fromData.Count != 0)
            {
                // will look at maximum tables later 
                output += " FROM ";
                for (int i = 0; i < fromData.Count; i++)
                {
                    output += fromData[i];
                    if (i != fromData.Count - 1) output += "' ";
                }

            }
            //if (whereData.Count != 0) // where a > 6 and b < 5 or c >2
            if(isWhereOperatorPresent)
            {
                output += " WHERE ";
                for(int i = 0; i < allWhereAndOr.Count; i++)
                {
                    string t = where(allWhereAndOr[i],allWhereExpressions[i] );
                    output += t;
                    if (i != allWhereAndOr.Count - 1) output += " AND ";
                }
            }
            if(hash_Select.ContainsKey("summarize"))
            {
                if (hash_Select.GetValueOrDefault("summarize").Count != 0 & summarizeByColumns.Count != 0)
                {
                    output += " GROUP BY ";
                    output = addListElements(summarizeByColumns, output);
                }
            }

            
            if(isOrderByPresent)
            {
                output += " ORDER BY ";
                output = addListElements(orderByColumns, output);
                if (orderType == ascending) output += " ASC ";
                if (orderType == descending) output += " DESC ";
            }

            return output;
        }

        public string where(List<string> whereAndOr, List<string> WhereExpressions )
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
            if(checkFunctionPresent(input)) return functionCheckwhere(input);
            else return input;
            // Need to work more on it

        }

        public string processFunction(string input)
        {
            string temp = "";
            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                {
                    string funcName = n.GetChild(0).ToString();
                    string funcArument = n.GetChild(1).ToString();
                    if(funcName == "max" | funcName == " max")
                    {
                        temp += "MAX";
                        
                    }
                    if (funcName == "min" | funcName == " min")
                    {
                        temp += "MIN";
                    }
                    if (funcName == "sum" | funcName == " sum")
                    {
                        temp += "SUM";
                    }
                    if (funcName == "avg" | funcName == " avg")
                    {
                        temp += "AVG";
                    }
                    if (funcName == "stdev" | funcName == " stdev")
                    {
                        temp += "STDEV";
                    }
                    if (funcName == "variance" | funcName == " variance")
                    {
                        temp += "VAR";
                    }
                    temp += "(";
                    List<string> tokens = TokenNames(funcArument);
                    string temp1 = processFunction(funcName);
                    if (temp1 != "") temp += temp1;
                    else
                    {
                        temp = addListElements(tokens, temp);
                    }

                    temp += ")";
                }
            }, n => { });
            return temp;
        }


        public string functionCheckwhere(string input)
        {
            string output = "";
            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if(n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                {
                    if(n.GetChild(0).ToString() == " isnotnull")
                    {
                        List<string> tokenColumns = TokenNames(n.GetChild(1).ToString());
                        output = addListElements(tokenColumns, output);
                        output += " IS NOT NULL ";
                    }
                }
            }, n => { });
            return output;
        }
        public Boolean checkFunctionPresent(string input)
        {
            Boolean ifPresent = false;
            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                {
                    ifPresent = true;
                }
            }, n => { });
            return ifPresent;
        }



        public string addListElements(List<string> list, string output)
        {
            string temp = output;
            for(int i = 0; i < list.Count; i++)
            {
                temp += list[i];
                if (i != list.Count - 1) temp += ", ";
            }
            return temp;
        }











    }
}