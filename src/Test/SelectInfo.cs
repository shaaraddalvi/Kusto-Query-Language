using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kusto.Language;

namespace Test
{
    class SelectInfo : SqlClause
    {
        string output;
        Boolean isPresent;
        Dictionary<string, List<string>> hash_Select;
        Dictionary<string, List<Kusto.Language.Syntax.SyntaxElement>> hash_GroupBy;
        List<Kusto.Language.Syntax.SyntaxElement> individualExpressions;
        public SelectInfo(string output, Boolean isPresent, Dictionary<string, List<string>> hash_Select, Dictionary<string, List<Kusto.Language.Syntax.SyntaxElement>> hash_GroupBy, List<Kusto.Language.Syntax.SyntaxElement> individualExpressions)
        {
            this.output = output;
            this.isPresent = isPresent;
            this.hash_Select = hash_Select;
            this.hash_GroupBy = hash_GroupBy;
            this.individualExpressions = individualExpressions;
        }
        public override string Process()
        {
            string output = "";
            if (hash_Select.Count == 0)
            {
                output += "SELECT * ";
            }
            if (hash_Select.Count != 0)
            {
                output += "SELECT";
                TakeTopLimitOperator take = new TakeTopLimitOperator(output, hash_Select.GetValueOrDefault("takeLiterals"), hash_Select.GetValueOrDefault("takeMathOperators"));
                output = take.Process();
                SummarizeOperator summarize = new SummarizeOperator(isPresent, output, individualExpressions, hash_GroupBy.GetValueOrDefault("SeparatedExpressions"));
                output = summarize.Process();
                ProjectOperator project = new ProjectOperator(output, hash_Select.GetValueOrDefault("project"), hash_Select);
                output = project.Process();

                // add other also if possible
                if ((!hash_Select.ContainsKey("project")) & (!hash_Select.ContainsKey("summarize"))) // add other conditions if possible
                {
                    output += " * ";
                }

            }
            return output;

        }
    }
}
