using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class GenericSQL
    {
        // wants to return output format
        // SELECT
        // FROM
        // WHERE
        // GROUP BY
       
        List<string> selectData = new List<string>();
       // Dictionary<string, List<string> > hash_Select = 
        List<string> fromData = new List<string>();
        List<string> whereData = new List<string>();
        List<string> groupByData = new List<string>();
        public string gettingSqlQuery(string kqlQuery)
        {
            string output = "";
            if(selectData.Count == 0)
            {
                output += "SELECT";
                output += " ";
                // no specified columns to select => select all columns
                output += "*";
            }
            if (selectData.Count != 0)
            {
                // * can be there in case of take operator
                output += "SELECT";
                for(int  i = 0; i < selectData.Count; i++ )
                {
                    output += selectData[i];
                    if (i != selectData.Count - 1) output += "' ";
                }
            }
            if (fromData.Count != 0)
            {
                output += " FROM ";
                for (int i = 0; i < fromData.Count; i++)
                {
                    output += fromData[i];
                    if (i != fromData.Count - 1) output += "' ";
                }

            }
            if (whereData.Count != 0)
            {
                output += " WHERE ";
                for (int i = 0; i < whereData.Count; i++)
                {
                    output += whereData[i];
                    if (i != whereData.Count - 1) output += "' ";
                }
            }
            return output;
        }

    }
}
