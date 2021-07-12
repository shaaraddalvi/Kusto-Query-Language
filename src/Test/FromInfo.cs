using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class FromInfo

    {
        Dictionary<string, Kusto.Language.Syntax.SyntaxElement> join_hash;
        List<string> fromData;
        List<string> allTokenNames;
        public FromInfo(Dictionary<string, Kusto.Language.Syntax.SyntaxElement> join_hash, List<string> fromData, List<string> allTokenNames)
        {
            this.join_hash = join_hash;
            this.fromData = fromData;
            this.allTokenNames = allTokenNames;
        }
        public string process()
        {
            fromData.Add(allTokenNames[0]);
            string output = "";
            if (fromData.Count != 0)
            {
                // will look at maximum tables later 
                output += " FROM ";

                for (int i = 0; i < fromData.Count; i++)
                {
                    output += fromData[i];
                    if (i != fromData.Count - 1) output += "' ";
                }
                if (join_hash.Count != 0)
                {
                    if (join_hash.GetValueOrDefault("type").ToString().Contains("leftouter"))
                    {
                        output += " LEFT OUTER JOIN ";
                    }
                    if (join_hash.GetValueOrDefault("type").ToString().Contains("rightouter"))
                    {
                        output += " RIGHT OUTER JOIN ";
                    }
                    if (join_hash.GetValueOrDefault("type").ToString().Contains("inner"))
                    {
                        output += " INNER JOIN ";
                    }
                    if (join_hash.GetValueOrDefault("type").ToString().Contains("fullouter"))
                    {
                        output += " FULL OUTER JOIN ";
                    }
                    // add other categories as well
                    var otherTable = join_hash.GetValueOrDefault("otherTable");
                    // 
                    //TestQueries temp = new TestQueries();
                    //output += temp.gettingSqlQueryNew(otherTable.ToString());
                    List<string> otherTableNames = Tree.TokenNames(otherTable.ToString());
                    output += "((";
                    output = HelperClass.addListElements(otherTableNames, output);
                    output += "))";
                    if (join_hash.ContainsKey("onInfo"))
                    {
                        List<List<string>> info = JoinOperator.onInfoJoin(join_hash.GetValueOrDefault("onInfo"));
                        output += " ON ";
                        if (info.Count == 0)
                        {
                            Kusto.Language.Syntax.SyntaxElement node = join_hash.GetValueOrDefault("onInfo");
                            List<string> t = Tree.TokenNames(node);
                            output += fromData[0] + "." + t[0].TrimStart();
                        }
                        else
                        {
                            if (info[0][0] == "$left" | info[0][0] == " $left")
                            {
                                output += allTokenNames[0];
                                output += ".";
                                output += info[0][1];
                            }
                            output += "=";
                            if (info[1][0] == "$right" | info[1][0] == " $right")
                            {
                                output += otherTableNames[0];
                                output += ".";
                                output += info[1][1];
                            }
                        }

                    }


                }

            }
            return output;

        }
    }
}
