using Kusto.Language;
using System;

namespace Test
{
    class Program //GG
    {
        static void Main(string[] args)
        {
            String[] queries = new String[] {      "T",
                                                    "T | project a",
                                                    "T | project a,b,c",
                                                    "T | take 50",
                                                    "T | take 10 + 20 ",
                                                    "T | take 10 * 20 * 30 ",
                                                    "T | where isnotnull(resultCode)",
                                                    "T | where a > 10",
                                                    //"T | where a == 2",
                                                    "T | summarize by name, type",
                                                    "T | summarize max(salary)",
                                                    "T | summarize min(salary)",
                                                    "T | summarize max(salary),min(stocks), max(workingHours) ",
                                                    "T | summarize sum(salary) ",
                                                    "T | summarize stdev(salary) ",
                                                    "T | summarize variance(salary) by state ",
                                                    "T | summarize max(salary), sum(salary), stdev(salary)",
                                                    "T | summarize avg(duration) by name", 
                                                    "T | project name, timestamp| order by timestamp desc nulls last"
                                                    
                                                         };
            // Next to work on 
            for (int i = 0; i < queries.Length; i++)
            {
                string input = queries[i];
                Console.WriteLine(input);
                //TestQueries.tree(input);
                TestQueries test = new TestQueries();
                string output = (test.GetSqlQuery(input));
                Console.WriteLine(output);
            }
            /*string input = queries[7];
            Console.WriteLine(input);
            TestQueries.tree(input);
            TestQueries test = new TestQueries();
            string output = (test.GetSqlQuery(input));
            Console.WriteLine(output);*/

        }
    }
}         
      




        
    
