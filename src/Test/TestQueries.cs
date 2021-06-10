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
        public static Boolean checkNoKeyword(string input_string)
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

        }


        public static Boolean checkProjectOperator(string input_string)
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
        }

        public static string noKeyword(string input_string)
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


        }

        public static string where_query(string input)
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

        }

                

           
        
    }
}