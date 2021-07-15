using System;
using System.Collections.Generic;
using Kusto.Language;
using System.Text;
using System.Linq;

namespace Test
{
    public class TestQueries
    {

        //const Kusto.Language.Syntax.SyntaxKind ProjectOperator = Kusto.Language.Syntax.SyntaxKind.ProjectOperator;
        const Kusto.Language.Syntax.SyntaxKind TakeOperator = Kusto.Language.Syntax.SyntaxKind.TakeOperator;
        //const Kusto.Language.Syntax.SyntaxKind WhereOperator = Kusto.Language.Syntax.SyntaxKind.FilterOperator;
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
        List<string> operatorsSupported = new List<string>();
        static string[] operators = {"ProjectOperator", "TakeOperator", "FilterOperator", "TopOperator", SummarizeOperator.ToString(),
            SummarizeByClause.ToString() ,SortOperator.ToString() , "JoinOperator" };
        List<string> operatorsList = operators.ToList<string>();

        
        List<string> dividedSubStringsPipe = new List<string>();
        List<string> translatedSubQuery = new List<string>();

        public void toCountNumPipeExpressions(string input)
        {
            var query = KustoCode.ParseAndAnalyze(input);
            var root = query.Syntax;
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n =>
            {
                if (n.Kind.ToString() == "PipeExpression")
                {
                    numPipeExpression++;
                }

            }, n => { });
        }

        public void dividePipeExpressions(string input)
        {
            toCountNumPipeExpressions(input);
            var query = KustoCode.ParseAndAnalyze(input);
            var root = query.Syntax;
            int counter = 1;
            if (numPipeExpression <= 1)
            {
                dividedSubStringsPipe.Add(input); return;
            }
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n =>
            {
                if (n.Kind.ToString() == "PipeExpression" & counter <= numPipeExpression - 1)
                {
                    if (counter != numPipeExpression - 1)
                    {
                        dividedSubStringsPipe.Add(n.GetChild(2).ToString());
                        counter++;
                    }
                    else
                    {
                        dividedSubStringsPipe.Add(n.GetChild(2).ToString());
                        dividedSubStringsPipe.Add(n.GetChild(0).ToString());
                        counter++;
                    }

                }

            }, n => { });
            dividedSubStringsPipe.Reverse();
        }



        public Dictionary<string, Kusto.Language.Syntax.SyntaxElement> allNestedTables = new Dictionary<string, Kusto.Language.Syntax.SyntaxElement>();
        public Dictionary<string, Kusto.Language.Syntax.SyntaxElement> preProcessingNew(string input)
        {
            var query = KustoCode.ParseAndAnalyze(input);
            var root = query.Syntax;
            List<Kusto.Language.Syntax.SyntaxElement> nodesIgnored = new List<Kusto.Language.Syntax.SyntaxElement>();
            Kusto.Language.Syntax.SyntaxElement.WalkNodes(root, n =>
            {
                if (nodesIgnored.Contains(n))
                {
                    int child = n.ChildCount;
                    for (int i = 0; i < child; i++)
                    {
                        nodesIgnored.Add(n.GetChild(i));
                    }
                }
                if ((!nodesIgnored.Contains(n)) & operatorsList.Contains(n.Kind.ToString()))
                {
                    preprocessingNewHelper(n);
                    int child = n.ChildCount;
                    for (int i = 0; i < child; i++)
                    {
                        nodesIgnored.Add(n.GetChild(i));
                    }
                }
            }, n => { });
            return allNestedTables;
        }
        int iNestedMap = 0;
        public void preprocessingNewHelper(Kusto.Language.Syntax.SyntaxElement node)
        {
            List<Kusto.Language.Syntax.SyntaxElement> nodesIgnored = new List<Kusto.Language.Syntax.SyntaxElement>();
            Kusto.Language.Syntax.SyntaxElement.WalkNodes((Kusto.Language.Syntax.SyntaxNode)node, n =>
            {
                if (n.Kind.ToString() == PipeSymbol)
                {
                    if (nodesIgnored.Contains(n))
                    {
                        nodesIgnored.Add(n.GetChild(0));
                        nodesIgnored.Add(n.GetChild(2));
                    }
                    if (!nodesIgnored.Contains(n))
                    {
                        nodesIgnored.Add(n.GetChild(0));
                        nodesIgnored.Add(n.GetChild(2));
                        iNestedMap++;
                        string temp = "Expression" + iNestedMap.ToString();
                        allNestedTables.Add(temp, n);
                    }
                }
            }, n => { });
        }

        public string gettingSqlQuery(string input)
        {
            string output = "";
            string temp = input;
            /*PreProcessingNew
             - will identify all the nested queries and store them in allNestedTables
             - 
            */
            //Dictionary<string, Kusto.Language.Syntax.SyntaxElement> allNestedTables = new Dictionary<string, Kusto.Language.Syntax.SyntaxElement>();
            preProcessingNew(input);

            if (allNestedTables.Count != 0)
            {
                output += "; WITH ";
            }
            foreach (string key in allNestedTables.Keys)
            {
                string val = allNestedTables.GetValueOrDefault(key).ToString().TrimStart().TrimEnd();
                TestQueries t = new TestQueries();
                output += key + " AS" + "(" + t.getSqlFromSingleKqlStatement_NoNested(val) + ")" + ",";
                temp = temp.Replace(val, key);
            }
            //if (allNestedTables.Count != 0) output = output.Remove(output.Length-1);
            //output += " \n";
            dividePipeExpressions(temp);
            if (dividedSubStringsPipe.Count == 1)
            {
                if (allNestedTables.Count != 0) output = output.Remove(output.Length - 1);
                output += (" " + getSqlFromSingleKqlStatement_NoNested(temp));

            }
            else
            {
                for (int i = 1; i < dividedSubStringsPipe.Count; i++)
                {
                    dividedSubStringsPipe[i] = "dummyTable | " + dividedSubStringsPipe[i];
                }
                for (int i = 0; i < dividedSubStringsPipe.Count; i++)
                {
                    TestQueries t = new TestQueries();
                    translatedSubQuery.Add(t.getSqlFromSingleKqlStatement_NoNested(dividedSubStringsPipe[i]));
                }
                for (int i = 1; i < translatedSubQuery.Count; i++)
                {
                    if (Tree.CheckInTree(dividedSubStringsPipe[i - 1], "JoinOperator") != null)
                    {
                        iNestedMap++; string key = "Expression" + iNestedMap.ToString();
                        output += key + " AS" + "(" + translatedSubQuery[i - 1] + ")" + ",";
                        translatedSubQuery[i] = translatedSubQuery[i].Replace("dummyTable", key);
                    } else
                        translatedSubQuery[i] = translatedSubQuery[i].Replace("dummyTable", "((" + translatedSubQuery[i - 1] + "))");
                }
                if (allNestedTables.Count != 0) output = output.Remove(output.Length - 1);
                output += (" " + translatedSubQuery[translatedSubQuery.Count - 1]);
            }

            return output;
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
                if (n.Kind.ToString() == "ProjectOperator")
                {
                    isProjectOperatorPresent = true;
                    projectColumns = Tree.TokenNames(n.ToString());
                    hash_Select.Add("project", projectColumns);
                    orderOperatorsSelect.Add("project");
                }
                if (n.Kind == TakeOperator | n.Kind.ToString() == "TopOperator")
                {
                    isTakeOperatorPresent = true;
                    takeLiterals = Tree.LiteralValues(n.ToString());
                    hash_Select.Add("takeLiterals", takeLiterals);
                    takeMathOperators = Tree.MathOperators(n.ToString());
                    hash_Select.Add("takeMathOperators", takeMathOperators);
                    orderOperatorsSelect.Add("takeLiterals");
                }
                if (n.Kind.ToString() == "FilterOperator")
                {
                    isWhereOperatorPresent = true;
                    // this will be a reverse list.
                    allWhereAndOr.Add(WhereOperator.whereInfoAboutAndOr(n.ToString()));

                    allWhereExpressions.Add(WhereOperator.whereInfoExpressions(n.ToString()));
                    //separateWhereHaving();
                }
                if (n.Kind == SummarizeOperator)
                {
                    isSumaarizeOperatorPresent = true;
                    string summarizeExpressions = n.GetChild(2).ToString();
                    //List<string> individualExpressions = divideExpressionsSummarize(n.GetChild(2).ToString());
                    Kusto.Language.Syntax.SyntaxElement node = n.GetChild(2);
                    //List<Kusto.Language.Syntax.SyntaxElement> individualExpressions = new List<Kusto.Language.Syntax.SyntaxElement>();
                    int num_child = node.ChildCount;
                    for (int i = 0; i < num_child; i++)
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
                    List<Kusto.Language.Syntax.SyntaxElement> list_nodes = Tree.SeparatedElements(n);
                    hash_GroupBy.Add("SeparatedExpressions", list_nodes);
                    nodeByClause = n.ToString();
                    //summarizeByColumns = TokenNames(n.ToString());
                }
                if (n.Kind == SortOperator)
                {
                    isOrderByPresent = true;
                    orderByColumns = Tree.TokenNames(n.ToString());
                }
                if (n.Kind == OrderingClause)
                {
                    isOrderingClausePresent = true;
                    string str = n.ToString();
                    if (str.Contains(descending)) orderType = descending;
                    if (str.Contains(ascending)) orderType = ascending;
                }
                if (n.Kind == Kusto.Language.Syntax.SyntaxKind.JoinOperator)
                {
                    typeInfo_Join = n.GetChild(1);
                    otherTableInfo_Join = n.GetChild(2);
                    onInfo_Join = n.GetChild(3);
                    join_hash.Add("type", typeInfo_Join);
                    join_hash.Add("otherTable", otherTableInfo_Join);
                    join_hash.Add("onInfo", onInfo_Join);
                }
                if (n == (Kusto.Language.Syntax.SyntaxNode)onInfo_Join)
                {
                    //if(onInfo_Join.GetDescendants<>) 
                }
                if (n.Kind == FunctionCallExpression)
                {
                    isFunctionCallPresent = true;
                    var list = Tree.TokenNames(n.ToString());
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


        List<string> havingExpressions = new List<string>();

        public void separateWhereHaving()
        {
            for (int i = 0; i < allWhereExpressions.Count; i++)
            {
                List<string> andOr = allWhereAndOr[i];
                List<string> whereExpression = allWhereExpressions[i];
                for (int j = 0; j < whereExpression.Count; j++)
                {
                    if (Tree.checkAggregateFunction(whereExpression[j]))
                    {
                        havingExpressions.Add(whereExpression[j]);
                        whereExpression.RemoveAt(j);
                        if (j == 0 & andOr.Count > 0) andOr.RemoveAt(0);
                        else if (andOr.Count > 1) andOr.RemoveAt(j - 1);

                    }
                }


            }
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
            SelectInfo select = new SelectInfo(output, isSumaarizeOperatorPresent, hash_Select, hash_GroupBy, individualExpressions);
            output = select.process();
            return output;
        }



        Dictionary<string, Kusto.Language.Syntax.SyntaxElement> join_hash = new Dictionary<string, Kusto.Language.Syntax.SyntaxElement>();
        public string fromInfo()
        {
            
            string output = "";
            FromInfo from = new FromInfo(join_hash,fromData,allTokenNames);
            output = from.process();
            return output;
        }

        public string whereInfo()
        {
            string output = "";
            WhereOperator where = new WhereOperator(isWhereOperatorPresent, allWhereExpressions, allWhereAndOr);
            output = where.process();
            return output;
        }

        public string groupByInfo()
        {
            string output = "";
            GroupBy groupBy = new GroupBy(output, hash_Select, hash_GroupBy, individualExpressions, summarizeByColumns);
            output = groupBy.process();
            return output;
        }
        public string havingInfo()
        {
            // Conditions on aggregate functions in where clause - having clause
            string output = "";
            Having having = new Having(output, havingExpressions);
            output = having.process();
            return output;
        }

        public string orderByInfo()
        {
            string output = "";
            OrderBy orderby = new OrderBy(output, orderType, orderByColumns);
            output = orderby.process();
            return output;
        }
        public string getSqlFromSingleKqlStatement_NoNested(string kqlQuery)
        {

            //preProcessTraverseTree(kqlQuery);
            traverseTree(kqlQuery);
            separateWhereHaving();
            string output = "";
            string select = selectInfo();output += select;  if(select != "") output += "\n"; ;

            string from  = fromInfo(); output += from; if (from != "") output += "\n";
            string where  = whereInfo(); output += where; if (where != "") output += "\n";
            string groupBy  = groupByInfo(); output += groupBy; if (groupBy != "") output += "\n";
            string having  = havingInfo(); output += having; if (having != "") output += "\n";
            string orderBy = orderByInfo(); output += orderBy; if (orderBy != "") output += "\n";

            return output;
        }





    }

}  

        
        
    
