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
                                                    //"T | where a == 2",
                                                    "T | where a > 10 | where b > 5",
                                                    "T | where a > 10 | where isnotnull(resultCode)  ",
                                                    "T | summarize by name, type",// 12
                                                    "T | summarize max(salary)",
                                                    "T | summarize min(salary)",
                                                    "T | summarize max(salary),min(stocks), max(workingHours) ",
                                                    "T | summarize sum(salary) ",
                                                    "T | summarize stdev(salary) ",
                                                    "T | summarize max(salary),variance(salary) by state ",
                                                    "T | summarize max(salary), sum(salary), stdev(salary)",
                                                    "T | summarize avg(duration) by name", 
                                                    "T | project name, timestamp| order by timestamp desc nulls last",
                                                    "T | summarize sum(salary) by state | take 50 | where salary = 100 and state> 6"
                                                    
                                                   
                                                         };
            // Next to work on 
            // Generalization done for these queriesG
           /* string input = queries[13];
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
            }
            




        }
    }
}         
      




        
    
