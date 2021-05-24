using System;
using System.Collections.Generic;
using Kusto.Language;

namespace Test
{
    class TestQueries
    {
        static String[] statements =new String[] {"T | project a = a + b | where a > 10.0",
                                        "T | project c = a + b | where c > 10.0",
                                        "T | where a > 10",
                                        "T | where a > 10 and b < 20",
                                        ".show table | count",
                                        "T | where a > 10; T1 | where b > 10",
                                        "T | summarize temp = count() by a | project p = a+temp",
                                        "T | where a memberof([\"@T1/b\"])",
                                        "T | where a == 0 or b == 0"
        };
        static string statement = statements[7];
        public static void tree()
        {
            var query = KustoCode.Parse(statement);
            var root = query.Syntax;
            GenTree.traverse(root, true);
        }

        public static void analyze()
        {

            var table =  new Kusto.Language.Symbols.TableSymbol("T", 
                new Kusto.Language.Symbols.ColumnSymbol("a",Kusto.Language.Symbols.ScalarTypes.Real),
                new Kusto.Language.Symbols.ColumnSymbol("b",Kusto.Language.Symbols.ScalarTypes.Real));
            var table1 =  new Kusto.Language.Symbols.TableSymbol("T1", 
                new Kusto.Language.Symbols.ColumnSymbol("a",Kusto.Language.Symbols.ScalarTypes.Real),
                new Kusto.Language.Symbols.ColumnSymbol("b",Kusto.Language.Symbols.ScalarTypes.Real));
            var database = new Kusto.Language.Symbols.DatabaseSymbol("db", table, table1);

            var globals = GlobalState.Default.WithDatabase(database);

            var query = KustoCode.ParseAndAnalyze(statement, globals);

            var root = query.Syntax;
            GenTree.traverse(root,true,true);
            Console.WriteLine("Result Type: " + query.ResultType);

            var dx = query.GetDiagnostics();
            int count = 0;
            foreach(Diagnostic dg in dx)
            {
                Console.WriteLine(++count + ": " + dg.Code + "\n" +  dg.Description + "\n" + (dg.HasLocation ? "\n" + dg.Start + " " + (dg.End) : "") + "\n");
            }
        }
    }
}