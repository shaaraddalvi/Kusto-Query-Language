using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class GroupBy
    {
        string output;
        Dictionary<string, List<string>> hash_Select;
        Dictionary<string, List<Kusto.Language.Syntax.SyntaxElement>> hash_GroupBy;
        List<Kusto.Language.Syntax.SyntaxElement> individualExpressions;
        List<string> summarizeByColumns;
        public GroupBy(string output, Dictionary<string, List<string>> hash_Select, Dictionary<string, List<Kusto.Language.Syntax.SyntaxElement>> hash_GroupBy, List<Kusto.Language.Syntax.SyntaxElement> individualExpressions, List<string> summarizeByColumns)
        {
            this.output = output;
            this.hash_Select = hash_Select;
            this.hash_GroupBy = hash_GroupBy;
            this.individualExpressions = individualExpressions;
            this.summarizeByColumns = summarizeByColumns;
        }

        public string process()
        {
            string output = "";
           
            if (hash_Select.ContainsKey("summarize"))
            {
                List<string> groupByNameReferences = new List<string>();
                if (individualExpressions.Count != 0 & summarizeByColumns.Count != 0)
                {
                    output += " GROUP BY ";
                    //output = addListElements(summarizeByColumns, output);
                    List<Kusto.Language.Syntax.SyntaxElement> temp = hash_GroupBy.GetValueOrDefault("SeparatedExpressions");
                    for (int i = 0; i < temp.Count; i++)
                    {
                        Kusto.Language.Syntax.SyntaxElement node = Tree.CheckInTree(temp[i], "NameReference");
                        if (node != null) groupByNameReferences.Add(node.ToString());
                        else groupByNameReferences.Add(temp[i].ToString());
                    }
                    //List<string> convertedTemp = NameRefFunCall(temp);
                    output = HelperClass.addListElements(groupByNameReferences, output);

                }
            }
            return output;
        }
    }
}
