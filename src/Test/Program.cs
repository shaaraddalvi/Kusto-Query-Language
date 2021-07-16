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
            String[] queries = new String[] {       "T",  
                                                    "T | project a,b,c", 
                                                    "T | take 50",   
                                                    "T | where a > 10",
                                                    "T | where a > 10 and c < 5 or d > 6 ",
                                                    "T | where isnotnull(resultCode)",
                                                    "T | where a > 10 | where b > 5",
                                                    "T | where col in ('value1', 'value2')", 
                                                    "T | where col !in ('value1', 'value2')", 
                                                    "T | where a in (tolong(1100)) ", 
                                                    "T | project a,b | take 5 | where a > 6 and b > 7",
                                                    "T | summarize count(Orders) by State| where count(Orders) > 5",
                                                    "T | summarize by name, type", 
                                                    "T | summarize max(salary),min(stocks), max(workingHours) ", 
                                                    "T | summarize max(salary),variance(salary) by state ",  
                                                    "T | summarize max(salary), sum(salary), stdev(salary)", 
                                                    "T | summarize AvgD = avg(duration), SumD = sum(days) by Name=operationName",  
                                                    "T | summarize AvgD = avg(duration), Count = count() by Name=operationName, RollNumber", 
                                                    "T | summarize AvgTimeToOpen=avg(todouble(TimeToOpenInSeconds)), AvgTimeToMerge=avg(todouble(TimeToMergeInSeconds))", 
                                                    "T | project name, timestamp| order by timestamp desc nulls last", 
                                                    "T | summarize sum(salary) by state | take 50 | where salary > 100 and state> 6", 
                                                    "T | project a | where a == 1 and b in ((Table |  where a > 5 )) ", 
                                                    "T | join kind =  inner ((exceptions)) on $left.operation_Id == $right.operation_Id",  
                                                    "T | join kind = rightouter  (exceptions) on $left.operation_Id == $right.operation_Id", 
                                                    "T | join kind = fullouter  (exceptions) on $left.operation_Id == $right.operation_Id", 
                                                    "T | join kind = leftouter  (exceptions) on $left.operation_Id == $right.operation_Id",  
                                                    "T | join kind = inner (exceptions | project operation_Id) on $left.operation_Id == $right.operation_Id", 
                                                    "T | join kind = inner(exceptions | project a ) on a | join kind = inner(Table | project a) on a",
                                                    "T | join kind = inner(Table1 | project a) on $left.a == $right.a | join kind = inner(Table2 | project a) on $left.a == $right.a",
                                                    
                                                   










                                                         };
            /*int i = 44;
            TestQueries test = new TestQueries();
            string output = test.solveNew(queries[queries.Length-2]);*/

            //Console.WriteLine(output);
            //SELECT * FROM ((SELECT * FROM ((SELECT a FROM ((SELECT * FROM T)))) WHERE  b IN  ((SELECT * FROM Table WHERE a > 5)) AND a= 1)) WHERE

            // Next to work on sGG
            // Generalization done for these queriesR_
           /* string input = queries[49];
            Console.WriteLine(input);
            Tree.tree(input);
            TestQueries test = new TestQueries();
            string output = (test.solveNewNew(input)); 
            Console.WriteLine(output);*/
            
            
            for (int i = 0; i <  queries.Length ; i++)
            {
                string input = queries[i];
                Console.WriteLine(input);
                //TestQueries.tree(input);
                KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
                string output = (test.gettingSqlQuery(input));
                Console.WriteLine(output);
                Console.WriteLine();
            }


            /*
             * 
             * dependencies | summarize AvgD = avg(duration) by Name=operationName  --> name convention
              --> hash_select - make all types as nodes from string. 
              
             * 
          
           
             * Nested queries -> generally in join queries 
                
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
/*
SELECT * FROM 
((SELECT CreatedDateSK,  AvgTimeToOpen,  AvgTimeToMerge,  SuccessCount FROM 
((SELECT CreatedDateSK,AVG( CAST(TimeToOpenInSeconds AS FLOAT )) AS  AvgTimeToOpen, 
AVG( CAST(TimeToMergeInSeconds AS FLOAT )) AS  AvgTimeToMerge, SUM( CASE WHEN(TimeToOpenInSeconds <= 25200) 
THEN 1 ELSE 0 END) AS SuccessCount FROM 
((SELECT * FROM 
((SELECT * FROM ((SELECT * FROM vw_PullRequest WHERE  RepositorySK in ( CAST(1100 as bigint )) AND 
CreatedDateSK <= 20210406 AND CreatedDateSK >= 20210323 AND 0 = TimezoneOffset AND PartitionId = 1)) 
WHERE AuthorSK IN  ((SELECT UserSK FROM ((SELECT * FROM ef_TeamUser WHERE  TeamSK = 1098 AND 1 = UserPartitionId))))))
WHERE AuthorSK IS NOT NULL )) 
GROUP BY  CreatedDateSK)))) 
ORDER BY  CreatedDateSK ASC
 */








//"T | where col in~ ('value1', 'value2')", //--> Need to figure out 
//   "T | summarize Count = count() by name| take 100 by Count desc",//skipped tokens

/*
 * Limitations of project - take 1 + 2
 *   "T | summarize count() | limit 5",  // 27
 *   "T | project a | where avg(column) > 4 + column/2 +   max(column)",  // 34
                                                    "T | where sum(iff(TimeToOpenInSeconds <= 25200, 1, 0)) == 3",  // 35
*/



