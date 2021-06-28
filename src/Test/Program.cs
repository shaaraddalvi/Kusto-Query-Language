using Kusto.Language;
using System;

namespace Test
{
    class Program //GGgGGGG
    {
        static void Main(string[] args)
        {
            String[] queries = new String[] {      "T",
                                                    "T | project a",
                                                    "T | project a,b,c",
                                                    "T | take 50",
                                                    "T | take 10 + 20 ",
                                                    "T | take 10 * 20 * 30 ",
                                                    "T | project a,b | take 5",
                                                    "T | where a > 10",
                                                    "T | where a > 10 and c < 5 or d > 6 ",
                                                    "T | project a,b | take 5 | where a > 6 and b > 7",
                                                    "T | where isnotnull(resultCode)",
                                                    "T | project a | where isnotnull(resultCode)",
                                                    "T | where a > 10 | where b > 5",
                                                    "T | where a > 10 | where isnotnull(resultCode)  ",
                                                    "T | summarize by name, type",
                                                    "T | summarize max(salary)",
                                                    "T | summarize min(salary)",
                                                    "T | summarize max(salary),min(stocks), max(workingHours) ",
                                                    "T | summarize sum(salary) ",
                                                    "T | summarize stdev(salary) ",
                                                    "T | summarize isdouble(salary) ",
                                                    "T | summarize max(salary),variance(salary) by state ",
                                                    "T | summarize max(salary), sum(salary), stdev(salary)",
                                                    "T | summarize avg(duration) by name",
                                                    "T | project name, timestamp| order by timestamp desc nulls last",
                                                    "T | summarize sum(salary) by state | take 50 | where salary > 100 and state> 6",
                                                    "T | where col in ('value1', 'value2')",
                                                    "T | where col !in ('value1', 'value2')",
                                                    //"T | where col in~ ('value1', 'value2')", //--> Need to figure out 
                                                    "T | summarize AvgD = avg(duration), SumD = sum(days) by Name=operationName",
                                                    "T | summarize AvgD = avg(duration), Count = count() by Name=operationName, RollNumber",
                                                    "T | join kind =  inner (exceptions) on $left.operation_Id == $right.operation_Id",
                                                    "T | join kind = rightouter  (exceptions) on $left.operation_Id == $right.operation_Id",
                                                    "T | join kind = fullouter  (exceptions) on $left.operation_Id == $right.operation_Id",
                                                    "T | join kind = leftouter  (exceptions) on $left.operation_Id == $right.operation_Id",
                                                    "T | project a | where avg(column) > 4",
                                                    "T | project a | where avg(column) > 4 + column/2 +   max(column)",
                                                  //   "T | summarize Count = count() by name| take 100 by Count desc",//skipped tokens

                                                         };
            // Next to work on s
            // Generalization done for these queriesG
            /*string input = queries[14];
            Console.WriteLine(input);
            TestQueries.tree(input);
            TestQueries test = new TestQueries();
            string output = (test.gettingSqlQueryNew(input));
            Console.WriteLine(output);*/
            for (int i = 0; i <  queries.Length; i++)
            {
                string input = queries[i];
                Console.WriteLine(input);
                //TestQueries.tree(input);
                TestQueries test = new TestQueries();
                string output = (test.gettingSqlQueryNew(input));
                Console.WriteLine(output);
                Console.WriteLine();
            }


            /*
             * queries - b > var(column) --> either using visitor pattern or something direct traversal method
             * dependencies | summarize AvgD = avg(duration) by Name=operationName  --> name convention
              --> hash_select - make all types as nodes from string. 
              --> do this convention for summarize By -> same concept
             * dependencies| project name, timestamp | order by timestamp asc nulls last  --> what is null first and null last
             * Functions to add --> todouble()  , count()--> COUNT(*)
             * Skipped tokens in last query
             * Nested queries -> generally in join queries 
                Problem -> operators in second tables are geeting added in first table also. 
                How to solve --> check if its ancestor 
            */


        }
    }
}         
      




        
    
