using System;
using System.Collections.Generic;
using Kusto.Language;
using System.Text;

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
        Dictionary<string, List<Kusto.Language.Syntax.SyntaxElement>> hash_GroupBy = new Dictionary<string, List<Kusto.Language.Syntax.SyntaxElement>>();
        List<string> projectColumns = new List<string>();
        List<string> takeLiterals = new List<string>();
        List<string> takeMathOperators = new List<string>();
        List<List<string>> allWhereAndOr = new List<List<string>>();
        List<List<string>> allWhereExpressions = new List<List<string>>();
        List<Kusto.Language.Syntax.SyntaxElement> individualExpressions = new List<Kusto.Language.Syntax.SyntaxElement>();
        Kusto.Language.Syntax.SyntaxElement typeInfo_Join;
        Kusto.Language.Syntax.SyntaxElement otherTableInfo_Join;
        Kusto.Language.Syntax.SyntaxElement onInfo_Join;
        string tableName = "";  // there can be multiple table names also - will consider these cases later. 
        string[] aggregateFunctions = { "max", "min", "sum", "count", "avg", "stdev", "variance" };
        string processedInput = "";
        Dictionary<string, string> nestedTables = new Dictionary<string, string>();
        public void preProcessTraverseTree(string kqlQuery)
        {
            processedInput = kqlQuery;
            var query = KustoCode.ParseAndAnalyze(kqlQuery);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => 
            {
                if(n.Kind.ToString() == "FilterOperator")
                {
                    Kusto.Language.Syntax.SyntaxElement newTable = CheckInTree(n, "PipeExpression");
                    if (newTable != null)
                    {
                        processedInput = processedInput.Replace(newTable.Parent.ToString(), "newTable");
                        nestedTables.Add("newTable", newTable.ToString());
                    }
                }
                /*if (n.Kind.ToString() == "JoinOperator")
                {
                    Kusto.Language.Syntax.SyntaxElement newTable = CheckInTree(n.GetChild(2), "PipeExpression");
                    if (newTable != null)
                    {
                        processedInput = processedInput.Replace(newTable.Parent.ToString(), "(newTable)");
                        nestedTables.Add("newTable", newTable.ToString());
                    }
                }*/

            }, n => { });
            
        }

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
                    orderOperatorsSelect.Add("project");
                }
                if (n.Kind == TakeOperator | n.Kind.ToString() == "TopOperator")
                {
                    isTakeOperatorPresent = true;
                    takeLiterals = LiteralValues(n.ToString());
                    hash_Select.Add("takeLiterals", takeLiterals);
                    takeMathOperators = MathOperators(n.ToString());
                    hash_Select.Add("takeMathOperators", takeMathOperators);
                    orderOperatorsSelect.Add("takeLiterals");
                }
                if (n.Kind == WhereOperator)
                {
                    isWhereOperatorPresent = true;
                   // this will be a reverse list.
                    allWhereAndOr.Add(whereInfoAboutAndOr(n.ToString()));

                    allWhereExpressions.Add(whereInfoExpressions(n.ToString()));
                    //separateWhereHaving();
                }
                if (n.Kind == SummarizeOperator)
                {
                    isSumaarizeOperatorPresent = true;
                    string summarizeExpressions =   n.GetChild(2).ToString();
                    //List<string> individualExpressions = divideExpressionsSummarize(n.GetChild(2).ToString());
                    Kusto.Language.Syntax.SyntaxElement node= n.GetChild(2);
                    //List<Kusto.Language.Syntax.SyntaxElement> individualExpressions = new List<Kusto.Language.Syntax.SyntaxElement>();
                    int num_child = node.ChildCount;
                    for(int i = 0; i < num_child; i++)
                    {
                        individualExpressions.Add(node.GetChild(i));
                    }
                    // List<string> individualExpressions = divideExpressionsSummarizeNode((Kusto.Language.Syntax.SyntaxNode)n.GetChild(2));
                    //hash_Select.Add("summarize", individualExpressions );//--> need to make all types to be syntaxElement
                    hash_Select.Add("summarize", new List<string>());
                    orderOperatorsSelect.Add("summarize");
                    //List<Kusto.Language.Syntax.SyntaxElement> list_nodes = SeparatedElements(n.GetChild(2));
                    // Cannot use this concept as it will include separated elements of function calls as well



                }
                if (n.Kind == SummarizeByClause)
                {
                    isSumaarizeByClausePresent = true;
                    List<Kusto.Language.Syntax.SyntaxElement> list_nodes = SeparatedElements(n);
                    hash_GroupBy.Add("SeparatedExpressions", list_nodes);
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
                if(n.Kind == Kusto.Language.Syntax.SyntaxKind.JoinOperator)
                {
                    typeInfo_Join = n.GetChild(1);
                    otherTableInfo_Join = n.GetChild(2);
                    onInfo_Join = n.GetChild(3);
                    join_hash.Add("type", typeInfo_Join);
                    join_hash.Add("otherTable", otherTableInfo_Join);
                    join_hash.Add("onInfo", onInfo_Join);
                }
                if(n == (Kusto.Language.Syntax.SyntaxNode)onInfo_Join)
                {
                    //if(onInfo_Join.GetDescendants<>) 
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






       
        public Kusto.Language.Syntax.SyntaxElement CheckInTree(string input_query, string findKind)
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

        public Kusto.Language.Syntax.SyntaxElement CheckInTree(Kusto.Language.Syntax.SyntaxElement root_node, string findKind)
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
        // Need to make changes to it.
        public List<Kusto.Language.Syntax.SyntaxElement> SeparatedElements(Kusto.Language.Syntax.SyntaxElement node)
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

        public List<List<string>> onInfoJoin(Kusto.Language.Syntax.SyntaxElement node)
        {
            List<List<string>> output = new List<List<string>>();
            Kusto.Language.Syntax.SyntaxElement left ;
            Kusto.Language.Syntax.SyntaxElement right ;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes((Kusto.Language.Syntax.SyntaxNode)node, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.EqualExpression)
                {
                    left = n.GetChild(0);
                    right = n.GetChild(2);
                    output.Add(TokenNames(left.ToString()));
                    output.Add(TokenNames(right.ToString()));


                }
                //if(n == (Kusto.Language.Syntax.SyntaxNode)left)
                {

                }

            }, n => { });
            return output;
        }
        List<string> havingExpressions = new List<string>();

        public void separateWhereHaving()
        {
            for(int i = 0; i < allWhereExpressions.Count; i++ )
            {
                List<string> andOr = allWhereAndOr[i];
                List<string> whereExpression = allWhereExpressions[i];
                for(int j = 0; j < whereExpression.Count; j++)
                {
                    if(checkAggregateFunction(whereExpression[j]))
                    {
                        havingExpressions.Add(whereExpression[j]);
                        whereExpression.RemoveAt(j);
                        if(j == 0 & andOr.Count > 0) andOr.RemoveAt(0);
                        else if (andOr.Count > 1) andOr.RemoveAt(j - 1);
                        
                    }
                }
                

            }
        }

        public Boolean checkAggregateFunction(string input)
        {
            Boolean t = false;
            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            int j = 0;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if(n.Kind.ToString() == "FunctionCallExpression")
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

        public List<string> whereInfoAboutAndOr(string input)
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
            input = input.TrimStart();
            List<string> whereExpressions = new List<string>();
            /*if ((!input.Contains("and")) & (!input.Contains("or")))
            {
                string temp = input.Remove(0, 6);
                whereExpressions.Add(temp);
            }*/
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
            if(whereExpressions.Count == 0)
            {
                string temp = input.Remove(0, 6);
                whereExpressions.Add(temp);
            }
            return whereExpressions;
        }

        public string summarizeGetFunction(Kusto.Language.Syntax.SyntaxElement node)
        {
            string temp = "";
            
            Kusto.Language.Syntax.SyntaxElement.WalkNodes((Kusto.Language.Syntax.SyntaxNode)node, n => {
                if(n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                {
                    temp = n.ToString();
                }
            }, n => { });
            return temp;
        }

        public string summarizeSimpleNamedExpression(Kusto.Language.Syntax.SyntaxElement node)
        {
            string temp = "";
            
            Kusto.Language.Syntax.SyntaxElement.WalkNodes((Kusto.Language.Syntax.SyntaxNode)node, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.SimpleNamedExpression)
                {
                    temp = n.GetChild(0).ToString();
                }
            }, n => { });
            return temp;
        }

        public List<string> divideExpressionsSummarize(string input)
        {
            List<string> list = new List<string>();
            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.SeparatedElement)
                {
                    list.Add(n.ToString());
                }
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                {
                    // break;
                }

            }, n => { });
            return list;
        }

        public List<string> divideExpressionsSummarizeNode(Kusto.Language.Syntax.SyntaxNode node)
        {
            List<string> list = new List<string>();
            
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(node, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.SeparatedElement)
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
        List<string> orderOperatorsSelect = new List<string>();
        public string selectInfo()
        {
            string output = "";
            if (hash_Select.Count == 0)
            {
                output += "SELECT";
                output += " ";
                output += "*";
            }
            if (hash_Select.Count != 0)
            { 
                output += "SELECT";
                for(int i = 0;  i < orderOperatorsSelect.Count; i++)
                {
                    string str = orderOperatorsSelect[i];
                    if (str == "takeLiterals") output = takeTopLimitOperator(output);
                    else if (str == "summarize") output = summarizeOperator(output);
                }
                //if (!hash_Select.ContainsKey("project") & output.Length > 0) output = output.Remove(output.Length - 1);
                output = projectOperator(output);
                
                // add other also if possible
                if ((!hash_Select.ContainsKey("project")) & (!hash_Select.ContainsKey("summarize"))) // add other conditions if possible
                {
                    output += " * ";
                }
                
            }
            return output;
        }
        public string takeTopLimitOperator(string output)
        {
            //string output = "";
            if (hash_Select.ContainsKey("takeLiterals"))
            {
                output += " TOP ";
                List<string> take_Literals = hash_Select.GetValueOrDefault("takeLiterals");
                List<string> take_mathOperators = hash_Select.GetValueOrDefault("takeMathOperators");
                if (take_mathOperators.Count == 0)
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
            return output;
        }
        public string projectOperator(string output)
        {
            //string output = "";
            if (hash_Select.ContainsKey("project"))
            {
                List<string> project_Columns = hash_Select.GetValueOrDefault("project");
                for (int i = 0; i < projectColumns.Count; i++)
                {
                    if (output.Contains(projectColumns[i]))
                    {
                        projectColumns.RemoveAt(i); i--;
                    }
                }
                if (projectColumns.Count == 0) output = output.Remove(output.Length - 1);
                output = addListElements(project_Columns, output);
            }
            else if(output.Length > 0 & output[output.Length - 1] == ',') output = output.Remove(output.Length - 1);
            return output;
        }

        public string summarizeOperator(string output)
        {
            //string output = "";
            if (hash_Select.ContainsKey("summarize"))
            {
                // List<string> summarizeExpressions = hash_Select.GetValueOrDefault("summarize");
                List<Kusto.Language.Syntax.SyntaxElement> summarizeExpressions = individualExpressions;
                if (summarizeExpressions.Count == 0)
                {
                    output += " DISTINCT ";
                    //output = addListElements(summarizeByColumns, output);
                    List<Kusto.Language.Syntax.SyntaxElement> temp = hash_GroupBy.GetValueOrDefault("SeparatedExpressions");
                    List<string> convertedTemp = NameRefFunCall(temp);
                    for (int k = 0; k < convertedTemp.Count; k++)
                    {
                        output += convertedTemp[k];

                    }
                    //output = addListElements(convertedTemp, output);
                }
                else
                {
                    output += " ";
                    if (summarizeByColumns.Count != 0)
                    {
                        //output = addListElements(summarizeByColumns, output);
                        List<Kusto.Language.Syntax.SyntaxElement> temp = hash_GroupBy.GetValueOrDefault("SeparatedExpressions");
                        List<string> convertedTemp = NameRefFunCall(temp);
                        output = addListElements(convertedTemp, output);

                        output += ",";

                        //output = addListElements(convertedTemp, output);
                    }

                    List<string> convertedExpr = NameRefFunCall(individualExpressions);
                    output = addListElements(convertedExpr, output);
                    output += ",";

                }
                
            }
            return output;
        }
        Dictionary<string, Kusto.Language.Syntax.SyntaxElement> join_hash = new Dictionary<string, Kusto.Language.Syntax.SyntaxElement>();
        public  string fromInfo()
        {
            fromData.Add(allTokenNames[0]);
            string output = "";
            if (fromData.Count != 0)
            {
                // will look at maximum tables later 
                output += " FROM ";

                for (int i = 0; i < fromData.Count; i++)
                {
                    output += fromData[i];
                    if (i != fromData.Count - 1) output += "' ";
                }
                if(join_hash.Count != 0)
                {
                    if(join_hash.GetValueOrDefault("type").ToString().Contains("leftouter") )
                    {
                        output += " LEFT OUTER JOIN ";
                    }
                    if (join_hash.GetValueOrDefault("type").ToString().Contains("rightouter"))
                    {
                        output += " RIGHT OUTER JOIN ";
                    }
                    if (join_hash.GetValueOrDefault("type").ToString().Contains("inner"))
                    {
                        output += " INNER JOIN ";
                    }
                    if (join_hash.GetValueOrDefault("type").ToString().Contains("fullouter"))
                    {
                        output += " FULL OUTER JOIN ";
                    }
                    // add other categories as well
                    var otherTable = join_hash.GetValueOrDefault("otherTable");
                    // 
                    //TestQueries temp = new TestQueries();
                    //output += temp.gettingSqlQueryNew(otherTable.ToString());
                    List<string> otherTableNames = TokenNames(otherTable.ToString());
                    output = addListElements(otherTableNames, output);
                    if(join_hash.ContainsKey ("onInfo"))
                    {
                        List<List<string>> info = onInfoJoin(join_hash.GetValueOrDefault("onInfo"));
                        output += " ON ";
                        if(info[0][0] == "$left" | info[0][0] == " $left")
                        {
                            output += allTokenNames[0];
                            output += ".";
                            output += info[0][1];
                        }
                        output += "=";
                        if (info[1][0] == "$right" | info[1][0] == " $right")
                        {
                            output += otherTableNames[0];
                            output += ".";
                            output += info[1][1];
                        }
                    }
                    if(nestedTables.Count != 0)
                    {
                        TestQueries t = new TestQueries();
                        output.Replace("newTable", t.gettingSqlQueryNew(nestedTables.GetValueOrDefault("newTableJoin")));
                    }
                    
                }

            }
            return output;
        }

        public string whereInfo()
        {
            string output = "";
            for(int i = 0; i < allWhereExpressions.Count; i++)
            {
                if(allWhereExpressions[i].Count == 0)
                {
                    allWhereExpressions.RemoveAt(i);
                    allWhereAndOr.RemoveAt(i);
                }
            }
            if (isWhereOperatorPresent & allWhereExpressions.Count > 0)
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

        public string groupByInfo()
        {
            string output = "";
            if (hash_Select.ContainsKey("summarize"))
            {
                List<string> groupByNameDeclarations = new List<string>();
                if (individualExpressions.Count != 0 & summarizeByColumns.Count != 0)
                {
                    output += " GROUP BY ";
                    //output = addListElements(summarizeByColumns, output);
                    List<Kusto.Language.Syntax.SyntaxElement> temp = hash_GroupBy.GetValueOrDefault("SeparatedExpressions");
                    for(int i = 0; i < temp.Count; i++)
                    {
                        Kusto.Language.Syntax.SyntaxElement node = CheckInTree(temp[i], "NameDeclaration");
                        if (node != null) groupByNameDeclarations.Add(node.ToString());
                        else groupByNameDeclarations.Add(temp[i].ToString());
                    }
                    //List<string> convertedTemp = NameRefFunCall(temp);
                    output = addListElements(groupByNameDeclarations, output);

                }
            }
            return output;
        }

        public List<string> NameRefFunCall(List<Kusto.Language.Syntax.SyntaxElement> list_nodes)
        {
            List<string> res = new List<string>();
            for(int i = 0; i < list_nodes.Count; i++)
            {
                res.Add(NameRefFunCallHelper(list_nodes[i]));
            }
            return res;
        }
        
        public string NameRefFunCallHelper(Kusto.Language.Syntax.SyntaxElement node)
        {
            string temp = "";
            Kusto.Language.Syntax.SyntaxElement node_nameref = CheckInTree(node, "SimpleNamedExpression");
            if (node_nameref == null)
            {
                Kusto.Language.Syntax.SyntaxElement node_funcall = CheckInTree(node, "FunctionCallExpression");
                if (node_funcall == null)
                {
                    temp = node.ToString();
                    return temp;
                }
                else
                {
                    temp = processFunction(node_funcall.ToString());
                    return temp;
                }
            }
            string name_ref = "";
            name_ref = CheckInTree(node_nameref, "NameDeclaration").ToString();
            //groupByNameDeclarations.Add(name_ref);
            Kusto.Language.Syntax.SyntaxElement node_funCall = CheckInTree(node_nameref, "FunctionCallExpression");
            string name_funcall = "";
            if (node_funCall != null)
            {
                Dictionary<string, string> map = processFunction_(node_funCall.ToString());
                name_funcall = node_funCall.ToString();
                foreach (string key in map.Keys)
                {
                    name_funcall = name_funcall.Replace(key, map[key]);
                }
            }
            else name_funcall = CheckInTree(node_nameref, "NameReference").ToString();
            temp += name_funcall + " AS " + name_ref;
            return temp;
        }

        public string havingInfo()
        {
            // Conditions on aggregate functions in where clause - having clause
            if (havingExpressions.Count == 0) return "";
            string output = " HAVING ";
            for(int i = 0; i < havingExpressions.Count; i++)
            {
                string str = havingExpressions[i];
                Dictionary<string, string> map = processFunction_(str);
                foreach (string key in map.Keys)
                {
                    str = str.Replace(key , map[key]);
                }
                output += str;
                output = output.Replace("==", "=");
                if (i != havingExpressions.Count - 1) output += " AND ";
            }
            return output;
        }

        public string orderByInfo()
        {
            string output = "";
            if (isOrderByPresent)
            {
                output += " ORDER BY ";
                output = addListElements(orderByColumns, output);
                if (orderType == ascending) output += " ASC ";
                if (orderType == descending) output += " DESC ";
            }
            return output;
        }



        public string gettingSqlQueryNew(string kqlQuery)
        {
            preProcessTraverseTree(kqlQuery);
            traverseTree(processedInput);
            separateWhereHaving();
            string output = "";
            output += selectInfo();
            output += fromInfo();
            string wherestring = whereInfo();
            output += whereInfo();
            output += groupByInfo();
            output += havingInfo();
            output += orderByInfo();
            if (nestedTables.Count != 0)
            {
                TestQueries test_ = new TestQueries();
                
                string newTableConverted = test_.gettingSqlQueryNew(nestedTables.GetValueOrDefault("newTable"));
               
                output = output.Replace("newTable", "("+newTableConverted + ")");
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
            if (checkFunctionPresent(input))
            {
                Dictionary<string, string> map = processFunction_(input);
                foreach (string key in map.Keys)
                {
                    input = input.Replace(key, map[key]);
                }
                input = input.Replace("==", "=");
                return input;
            }

                string temp = "";
            if (CheckInTree(input, "InExpression") != null )
            {
                //replace that with IN
                Kusto.Language.Syntax.SyntaxElement node = CheckInTree(input, "InExpression");
                temp += node.GetChild(0).ToString();
                temp += " IN ";
                string str =  node.GetChild(2).ToString();
                Dictionary<string, string> map = processFunction_(str);
                foreach (string key in map.Keys)
                {
                    str = input.Replace(key, map[key]);
                }
                temp += str;
                temp = temp.Replace("==", "=");
                return temp;
            }
            if (CheckInTree(input, "NotInExpression") != null)
            {
                //replace that with NOT IN
                Kusto.Language.Syntax.SyntaxElement node = CheckInTree(input, "NotInExpression");
                temp += node.GetChild(0).ToString();
                temp += " NOT IN ";
                string str = node.GetChild(2).ToString();
                Dictionary<string, string> map = processFunction_(str);
                foreach (string key in map.Keys)
                {
                    str = input.Replace(key, map[key]);
                }
                temp += str;
                temp = temp.Replace("==", "=");
                return temp;
            }
            if (CheckInTree(input, "InCsExpression") != null)
            {
                //replace that with NOT IN
                Kusto.Language.Syntax.SyntaxElement node = CheckInTree(input, "InCsExpression");
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


        Boolean isAggregateFunction = false;
        public string processFunction(string input)
        {
            string temp = "";
            var query = KustoCode.Parse(input);
            var root = query.Syntax;
            int i = 0;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n => {
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.FunctionCallExpression)
                {
                    if(i != 0) { }
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
                                List<string> tokens_ = TokenNames(funcArument);

                                if (tokens_.Count == 1 & tokens_[0] == "") temp += "*";
                                else temp = addListElements(tokens_, temp);
                            }

                            temp += "AS FLOAT";

                        }
                        if (funcName == "max" | funcName == " max" | funcName == "min" | funcName == " min" | funcName == "sum" | funcName == " sum" |
                    funcName == "avg" | funcName == " avg" | funcName == "stdev" | funcName == " stdev" | funcName == "variance" | funcName == " variance" |
                    funcName == "count" | funcName == " count"
                    )
                        {
                            isAggregateFunction = true;
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
                            List<string> tokens = TokenNames(funcArument);
                            string temp1 = processFunction(funcArument);
                            if (temp1 != "") temp += temp1;
                            else
                            {
                                if (tokens.Count == 1 & tokens[0] == "") temp += "*";
                                else temp = addListElements(tokens, temp);
                            }
                            temp += ")";
                        }
                    
                    
                    
                       
                    }
                }
            }, n => { });
            return temp;
        }

        public Dictionary<string, string> processFunction_(string input)
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
                    funcArument = removeBrackets(funcArument).TrimStart().TrimEnd();
                    if (funcName == "tolong")
                    {
                        funcArument = removeBrackets(funcArument).TrimStart().TrimEnd();
                        temp += " CAST(";
                        Kusto.Language.Syntax.SyntaxElement node = CheckInTree(funcArument, "FunctionCallExpression");
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
                        funcArument = removeBrackets(funcArument).TrimStart().TrimEnd();
                        temp += " CAST(";
                        Kusto.Language.Syntax.SyntaxElement node = CheckInTree(funcArument, "FunctionCallExpression");
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
                    if(funcName == "iff")
                    {
                        List<Kusto.Language.Syntax.SyntaxElement> list = SeparatedElements(n.GetChild(1));
                        string condition = list[0].ToString().TrimEnd().TrimStart();
                        condition = condition.Remove(condition.Length-1);
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
                    if(funcName == "notnull")
                    {
                        Kusto.Language.Syntax.SyntaxElement node = CheckInTree(funcArument, "FunctionCallExpression");
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



                    if (String.Equals(funcName.TrimStart(), "max") | String.Equals(funcName.TrimStart(), "min") |  String.Equals(funcName.TrimStart(), "sum")  |
                        String.Equals(funcName.TrimStart(), "avg") |  String.Equals(funcName.TrimStart(), "stdev") | String.Equals(funcName.TrimStart(), "variance") | 
                        String.Equals(funcName.TrimStart(), "count"))
                    {
                         isAggregateFunction = true;
                         if (String.Equals(funcName.TrimStart(), "max") ) { temp += "MAX"; }

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
                        funcArument = removeBrackets(funcArument).TrimStart().TrimEnd();
                        Dictionary<string,string> temp1 = processFunction_(funcArument);
                        foreach (string key in temp1.Keys)
                        {
                            funcArument = funcArument.Replace(key, temp1[key]);
                        }
                        temp += funcArument;
                       
                         temp += ")";
                    }
                    map.Add(n.ToString(), temp);
                    temp = "";



                 }
                
            }, n => { });
            return map;
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

        public string removeBrackets(string input)
        {
            if (input.Length < 2) return "";
            if(input[0] == '(')
            {
                StringBuilder sb = new StringBuilder(input);
                sb.Remove(0, 1);
                input = sb.ToString();
            }
            if (input[input.Length-1] == ')')
            {
                StringBuilder sb = new StringBuilder(input);
                sb.Remove(input.Length-1, 1);
                input = sb.ToString();
            }
            return input;
        }











    }
}