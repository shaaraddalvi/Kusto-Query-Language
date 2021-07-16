using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test;
using Kusto.Language;
using System;
using System.Collections.Generic;






namespace UnitTestProject
   
{
    [TestClass]
    public class KustoTests
    {
        [TestMethod]
        public void TestSelectInfo_NoOperator()
        {
            string kqlQuery = "T";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.selectInfo().TrimStart().TrimEnd();
            string expectedSqlQuery = "SELECT *";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }
        [TestMethod]
        public void TestSelectInfo_ProjectOperator()
        {
            string kqlQuery = "T | project a , b";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.selectInfo().TrimStart().TrimEnd();  // "SELECT a,  b"
            string expectedSqlQuery = "SELECT a,  b";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }
        [TestMethod]
        public void TestSelectInfo_TakeOperator()
        {
            string kqlQuery = "T | take 5";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.selectInfo().TrimStart().TrimEnd();
            string expectedSqlQuery = "SELECT TOP  5 *";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }
        [TestMethod]
        public void TestSelectInfo_SummarizeDistinct()
        {
            string kqlQuery = "T | summarize by col1 , col2";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.selectInfo().TrimStart().TrimEnd();// "SELECT DISTINCT col1 , col2"
            string expectedSqlQuery = "SELECT DISTINCT col1 , col2";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }
        [TestMethod]
        public void TestSelectInfoSummarize()
        {
            string kqlQuery = "T | summarize avg(column) by state";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.selectInfo().TrimStart().TrimEnd();
            string expectedSqlQuery = "SELECT state,AVG(column)";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }
        [TestMethod]
        public void TestSelectInfo_SummarizeNameReference()
        {
            string kqlQuery = "T | summarize AvgD = avg(duration), SumD = sum(days) by Name = operationName";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.selectInfo().TrimStart().TrimEnd();//SELECT  operationName AS  Name,AVG(duration) AS  AvgD, SUM(days) AS  SumD
            string expectedSqlQuery = "SELECT  operationName AS  Name,AVG(duration) AS  AvgD, SUM(days) AS  SumD";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }


        [TestMethod]
        public void TestFromInfo()
        {
            string kqlQuery = "T ";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.fromInfo().TrimStart().TrimEnd();
            string expectedSqlQuery = "FROM T";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }
        [TestMethod]
        public void TestFromInfoJoin()
        {
            string kqlQuery = "T | join kind = fullouter  (exceptions) on $left.operation_Id == $right.operation_Id";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.fromInfo().TrimStart().TrimEnd();
            string expectedSqlQuery = "FROM T FULL OUTER JOIN ((exceptions)) ON T.operation_Id=exceptions.operation_Id";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }

        [TestMethod]
        public void TestWhereInfo_FunctionCall_isnotnull()
        {
            string kqlQuery = "T | where isnotnull(resultCode)";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.whereInfo().TrimStart().TrimEnd();
            string expectedSqlQuery = "WHERE resultCode IS NOT NULL";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }

        [TestMethod]
        public void TestWhereInfo_MultipleExpressions_AndOr()
        {
            string kqlQuery = "T | where a > 10 and c < 5 or d > 6 ";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.whereInfo().TrimStart().TrimEnd();
            string expectedSqlQuery = "WHERE  a > 10 AND c < 5 OR d > 6";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }

        [TestMethod]
        public void TestWhereInfo_InOperator()
        {
            string kqlQuery = "T | where col in ('value1', 'value2')";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.whereInfo().TrimStart().TrimEnd();
            string expectedSqlQuery = "WHERE col IN  ('value1', 'value2')";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }

        [TestMethod]
        public void TestWhereInfo_FunctionCall_tolong()
        {
            string kqlQuery = "T | where a > tolong(1100)";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.whereInfo().TrimStart().TrimEnd();
            string expectedSqlQuery = "WHERE a > CAST(1100 as bigint )";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }

        



        
        // "T | summarize avg(duration) by name"
        [TestMethod]
        public void TestGroupByInfo_NoNameReference()
        {
            string kqlQuery = "T | summarize avg(duration) by name";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.groupByInfo().TrimStart().TrimEnd(); // "GROUP BY  name"
            string expectedSqlQuery = "GROUP BY  name";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }
        [TestMethod]
        public void TestGroupByInfo_NameReference()
        {
            string kqlQuery = "T | summarize AvgD = avg(duration), SumD = sum(days) by Name=operationName, RollNumber";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.groupByInfo().TrimStart().TrimEnd();// "GROUP BY operationName,  RollNumber"
            string expectedSqlQuery = "GROUP BY operationName,  RollNumber";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }




        [TestMethod]
        public void TestHavingInfo_AggregateFunctions()
        {
            string kqlQuery = "T | summarize count(Orders) by State| where count(Orders) > 5";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            test.separateWhereHaving();
            string outputSqlQuery = test.havingInfo().TrimStart().TrimEnd();
            string expectedSqlQuery = "HAVING COUNT(Orders) > 5";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }

        [TestMethod]
        public void TestOrderByInfo_descending()
        {
            string kqlQuery = "T | project name, timestamp| order by timestamp desc nulls last";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.traverseTree(kqlQuery);
            string outputSqlQuery = test.orderByInfo().TrimStart().TrimEnd();
            string expectedSqlQuery = "ORDER BY  timestamp DESC";
            Assert.IsTrue(outputSqlQuery.Equals(expectedSqlQuery));
        }
        [TestMethod]
        public void TestPreprocessingInfo()
        {
            string kqlQuery = "T | project a | where a == 1 and b in ((Table |  where a > 5 )) ";
            KqltoSqlTranslatorClass test = new KqltoSqlTranslatorClass();
            test.gettingSqlQuery(kqlQuery);
            Dictionary<string, Kusto.Language.Syntax.SyntaxElement> allNestedTables = test.allNestedTables;
            //Dictionary<string, Kusto.Language.Syntax.SyntaxElement> expectedNestedTables = new Dictionary<string, Kusto.Language.Syntax.SyntaxElement>();
            List<string> expectedNestedTables = new List<string>();
            expectedNestedTables.Add("Table |  where a > 5");
            int i = 0;
            foreach(string key in allNestedTables.Keys)
            {
                string val = allNestedTables.GetValueOrDefault(key).ToString();
                Assert.IsTrue(val.Equals(expectedNestedTables[i]));  i++;
            }


        }





    }
}
