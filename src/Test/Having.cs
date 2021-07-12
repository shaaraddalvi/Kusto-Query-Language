using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Having
    {
        string output;
        List<string> havingExpressions;
        public Having(string output , List<string> havingExpressions)
        {
            this.output = output;
            this.havingExpressions = havingExpressions;
        }
        public string process()
        {
            // Conditions on aggregate functions in where clause - having clause
            if (havingExpressions.Count == 0) return "";
            string output = " HAVING ";
            for (int i = 0; i < havingExpressions.Count; i++)
            {
                string str = havingExpressions[i];
                Dictionary<string, string> map = Functions.processFunction_(str);
                foreach (string key in map.Keys)
                {
                    str = str.Replace(key, map[key]);
                }
                output += str;
                output = output.Replace("==", "=");
                if (i != havingExpressions.Count - 1) output += " AND ";
            }
            return output;
        }
    }
}
