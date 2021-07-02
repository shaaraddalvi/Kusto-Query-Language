using Kusto.Language;
using System;

namespace Test
{
    class Program //GGgGGGG
    {
        static string sample_query = "vw_PullRequest" +
    "| where PartitionId == 1 and 0 == TimezoneOffset and CreatedDateSK >= 20210323 and CreatedDateSK <= 20210406 and RepositorySK in (tolong(1100))" +
    "| where AuthorSK in ((ef_TeamUser" +
                        "| where 1 == UserPartitionId and TeamSK == 1098" +
                        "| project UserSK))" +

    "| where notnull(AuthorSK)" +
    "| summarize AvgTimeToOpen = avg(todouble(TimeToOpenInSeconds)), AvgTimeToMerge = avg(todouble(TimeToMergeInSeconds))," +
                "SuccessCount = sum(iff(TimeToOpenInSeconds <= 25200, 1, 0)) by CreatedDateSK" +
    "| project CreatedDateSK, AvgTimeToOpen, AvgTimeToMerge, SuccessCount" +
    "| sort by CreatedDateSK asc nulls first";
        static void Main(string[] args)// Gg T | project a,b,c,d | where e = 10
        {
            String[] queries = new String[] {      "T",  // 0
                                                    "T | project a", // 1
                                                    "T | project a,b,c", // 2
                                                    "T | take 50",    // 3
                                                    "T | take 10 + 20 ", // 4
                                                    "T | take 10 * 20 * 30 ", // 5
                                                    "T | project a,b | take 5", // 6
                                                    "T | take 5 | project a,b", // 7
                                                    "T | where a > 10", // 8
                                                    "T | where a > 10 and c < 5 or d > 6 ",  // 9
                                                    "T | project a,b | take 5 | where a > 6 and b > 7", // 10
                                                    "T | where isnotnull(resultCode)",  // 11
                                                    "T | project a | where isnotnull(resultCode)",  // 12
                                                    "T | where a > 10 | where b > 5",   // 13
                                                    "T | where a > 10 | where isnotnull(resultCode)  ", // 14
                                                    "T | where 0 == avg(col)",  // 15
                                                    "T | summarize by name, type", // 16
                                                    "T | summarize max(salary)", // 17
                                                    "T | summarize min(salary)", // 18
                                                    "T | summarize max(salary),min(stocks), max(workingHours) ", // 19
                                                    "T | summarize sum(salary) ",  // 20
                                                    "T | summarize stdev(salary) ",  // 21
                                                    //"T | summarize isdouble(salary) ",  // 22
                                                    "T | summarize max(salary),variance(salary) by state ",  // 22
                                                    "T | summarize max(salary), sum(salary), stdev(salary)", // 23
                                                    "T | summarize avg(duration) by name",  // 24
                                                    "T | project name, timestamp| order by timestamp desc nulls last",
                                                    "T | summarize sum(salary) by state | take 50 | where salary > 100 and state> 6",
                                                    "T | summarize count() | limit 5",
                                                    "T | where col in ('value1', 'value2')",
                                                    "T | where col !in ('value1', 'value2')",
                                                     "T | where a > tolong(1100)",
                                                    "T | where a in (tolong(1100)) ",
                                                     "T | where avg(todouble(a)) > 2",
                                                     "T | project a | where avg(column) > 4",
                                                    "T | project a | where avg(column) > 4 + column/2 +   max(column)",
                                                    "T | where sum(iff(TimeToOpenInSeconds <= 25200, 1, 0)) == 3",
                                                    "T | summarize AvgD = avg(duration), SumD = sum(days) by Name=operationName",
                                                    "T | summarize AvgD = avg(duration), Count = count() by Name=operationName, RollNumber",
                                                    "T | summarize AvgTimeToOpen=avg(todouble(TimeToOpenInSeconds)), AvgTimeToMerge=avg(todouble(TimeToMergeInSeconds)) | project CreatedDateSK, AvgTimeToOpen",
                                                     "T | project a | where a== 1 and b in ((Table |  where a > 5 )) | where notnull(authorSK) ",
                                                    "T | join kind =  inner (exceptions) on $left.operation_Id == $right.operation_Id",
                                                    "T | join kind = rightouter  (exceptions) on $left.operation_Id == $right.operation_Id",
                                                    "T | join kind = fullouter  (exceptions) on $left.operation_Id == $right.operation_Id",
                                                    "T | join kind = leftouter  (exceptions) on $left.operation_Id == $right.operation_Id",
                                                    Program.sample_query,
                                                    "T | limit 10 | summarize count()",









                                                         };
            int i = 6;
            TestQueries test = new TestQueries();
            string output = (test.solveNew(queries[i]));
            Console.WriteLine(output);
            // Next to work on sGG
            // Generalization done for these queriesG
            /*string input = queries[queries.Length- 4];
            Console.WriteLine(input);
            TestQueries.tree(input);
            TestQueries test = new TestQueries();
            string output = (test.gettingSqlQueryNew(input));
            Console.WriteLine(output);*/
            /*for (int i = 0; i <  queries.Length ; i++)
            {
                string input = queries[i];
                Console.WriteLine(input);
                //TestQueries.tree(input);
                TestQueries test = new TestQueries();
                string output = (test.gettingSqlQueryNew(input));
                Console.WriteLine(output);
                Console.WriteLine();
            }*/


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
/*
    vw_PullRequest
    | where PartitionId == 1 and 0 == TimezoneOffset and CreatedDateSK >= 20210323 and CreatedDateSK <= 20210406 and RepositorySK in (tolong(1100))
    | where AuthorSK in ((  ef_TeamUser
                        | where 1 == UserPartitionId and TeamSK == 1098
                        | project UserSK
                      ))
    | where notnull(AuthorSK)
    | summarize AvgTimeToOpen=avg(todouble(TimeToOpenInSeconds)), AvgTimeToMerge=avg(todouble(TimeToMergeInSeconds)),
                SuccessCount=sum(iff(TimeToOpenInSeconds <= 25200, 1, 0)) by CreatedDateSK
    | project CreatedDateSK, AvgTimeToOpen, AvgTimeToMerge, SuccessCount
    | sort  by CreatedDateSK asc nulls first

*/
/*
SELECT CreatedDateSK,
AVG( CAST(TimeToOpenInSeconds AS FLOAT)) AS  AvgTimeToOpen, 
AVG( CAST(TimeToMergeInSeconds AS FLOAT)) AS  AvgTimeToMerge, 
SUM( CASE WHEN(TimeToOpenInSeconds <= 25200) THEN 1 ELSE 0 END) AS SuccessCount 
FROM vw_PullRequest 
WHERE  RepositorySK in ( CAST(1100 as bigint )) 
   AND CreatedDateSK <= 20210406 
   AND CreatedDateSK >= 20210323 
   AND 0 = TimezoneOffset 
   AND PartitionId = 1 
   AND AuthorSK IN  
         ((SELECT UserSK FROM ef_TeamUser WHERE  TeamSK = 1098 AND 1 = UserPartitionId)) 
   AND AuthorSK IS NOT NULL  
GROUP BY  CreatedDateSK 
ORDER BY  CreatedDateSK ASC
*/








//"T | where col in~ ('value1', 'value2')", //--> Need to figure out 
//   "T | summarize Count = count() by name| take 100 by Count desc",//skipped tokens




