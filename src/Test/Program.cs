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
                                                   // "T | take 10 + 20 ",
                                                   // "T | take 10 * 20 * 30 ",
                                                    "T | where isnotnull(resultCode)",
                                                    "T | where a > 10",
                                                    "T | where a == 2"
                                                         };

            for(int i = 0; i < queries.Length; i++)
            {
                string input = queries[i];
                Console.WriteLine(input);
                //TestQueries.tree(input);

                if (TestQueries.checkNoKeyword(input)) { TestQueries.noKeyword(input); }
                if (TestQueries.checkProjectOperator(input)) { TestQueries.project_query(input); }

                if (TestQueries.checkTakeOperator(input)) { TestQueries.take_query(input); }
                if (TestQueries.checkWhereOperator(input)) { TestQueries.where_query(input); }
                Console.WriteLine();
            }
            



        }
    }
}