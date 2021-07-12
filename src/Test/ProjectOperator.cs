using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class ProjectOperator
    {
        string output;
        List<string> project_Columns;
        Boolean isPresent = false;
        const Kusto.Language.Syntax.SyntaxKind projectOperator = Kusto.Language.Syntax.SyntaxKind.ProjectOperator;
        Dictionary<string, List<string>> hash_Select;
        public ProjectOperator(string output , List<string> project_Columns, Dictionary<string, List<string>> hash_Select)
        {
            this.output = output;
            this.project_Columns = project_Columns;
            this.hash_Select = hash_Select;
            if (project_Columns != null)
            {
                if(project_Columns.Count > 0) isPresent = true;
            }
        }

        public void start(Kusto.Language.Syntax.SyntaxElement n)
        {
            if (n.Kind == projectOperator)
            {
                isPresent = true;
                project_Columns = Tree.TokenNames(n.ToString());
                hash_Select.Add("project", project_Columns);
            }
        }

        public string process()
        {
            //string output = "";
            if (isPresent)
            {
                List<string> projectColumns = this.project_Columns;
                
                if (projectColumns.Count == 0) output = output.Remove(output.Length - 1);
                output = HelperClass.addListElements(project_Columns, output);
            }
            else if (output.Length > 0 & output[output.Length - 1] == ',') output = output.Remove(output.Length - 1);
            return output;
        }

    }
}
