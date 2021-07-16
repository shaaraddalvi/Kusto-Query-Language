using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class SummarizeOperator : Operator
    {
        string output;
        Boolean isPresent;
        List<Kusto.Language.Syntax.SyntaxElement> summarizeExpressions;
        List<Kusto.Language.Syntax.SyntaxElement> temp = new List<Kusto.Language.Syntax.SyntaxElement>();
        public SummarizeOperator(Boolean isPresent , string output, List<Kusto.Language.Syntax.SyntaxElement> summarizeExpressions,
            List<Kusto.Language.Syntax.SyntaxElement> temp
            )
        {
            this.isPresent = isPresent;
            this.output = output;
            this.summarizeExpressions = summarizeExpressions;
            this.temp = temp;
        }

        public override string Process()
        {
            if (isPresent)
            {
                
                if (summarizeExpressions.Count == 0)
                {
                    output += " DISTINCT ";
                    List<string> convertedTemp = NameRefFunCall(temp);
                    /*for (int k = 0; k < convertedTemp.Count; k++)
                    {
                        output += convertedTemp[k];

                    }*/
                    output = HelperClass.addListElements(convertedTemp, output);
                }
                else
                {
                    output += " ";
                    if (temp != null )
                    {
                        if(temp.Count != 0)
                        {
                            List<string> convertedTemp = NameRefFunCall(temp);
                            output = HelperClass.addListElements(convertedTemp, output);

                            output += ",";

                            //output = addListElements(convertedTemp, output);
                        }

                    }

                    List<string> convertedExpr = NameRefFunCall(summarizeExpressions);
                    output = HelperClass.addListElements(convertedExpr, output);
                    output += ",";

                }

            }
            return output;
        }
        public List<string> NameRefFunCall(List<Kusto.Language.Syntax.SyntaxElement> list_nodes)
        {
            List<string> res = new List<string>();

            for (int i = 0; i < list_nodes.Count; i++)
            {
                res.Add(NameRefFunCallHelper(list_nodes[i]));
            }
            return res;
        }
        public string NameRefFunCallHelper(Kusto.Language.Syntax.SyntaxElement node)
        {
            string temp = "";
            Kusto.Language.Syntax.SyntaxElement node_nameref = Tree.CheckInTree(node, "SimpleNamedExpression");
            if (node_nameref == null)
            {
                Kusto.Language.Syntax.SyntaxElement node_funcall = Tree.CheckInTree(node, "FunctionCallExpression");
                if (node_funcall == null)
                {
                    temp = node.ToString().TrimStart().TrimEnd();
                    if (temp[temp.Length - 1] == ',') temp = temp.Remove(temp.Length - 1);
                    return temp;
                }
                else
                {
                    temp = node.ToString().TrimStart().TrimEnd();
                    if (temp[temp.Length - 1] == ',') temp = temp.Remove(temp.Length - 1);
                    Dictionary<string, string> map = Functions.processFunction_(temp);
                    foreach (string key in map.Keys)
                    {
                        temp = temp.Replace(key, map[key]);
                    }
                    return temp;
                }
            }
            string name_ref = "";
            name_ref = Tree.CheckInTree(node_nameref, "NameDeclaration").ToString();
            //groupByNameDeclarations.Add(name_ref);
            Kusto.Language.Syntax.SyntaxElement node_funCall = Tree.CheckInTree(node_nameref, "FunctionCallExpression");
            string name_funcall = "";
            if (node_funCall != null)
            {
                Dictionary<string, string> map = Functions.processFunction_(node_funCall.ToString());
                name_funcall = node_funCall.ToString();
                foreach (string key in map.Keys)
                {
                    name_funcall = name_funcall.Replace(key, map[key]);
                }
            }
            else name_funcall = Tree.CheckInTree(node_nameref, "NameReference").ToString();
            temp += name_funcall + " AS " + name_ref;
            return temp;
        }
    }
}
