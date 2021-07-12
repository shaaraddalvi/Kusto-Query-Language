using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class OrderBy
    {
        string output;
        Boolean isPresent = false;
        string orderType;
        List<string> orderByColumns;
        const string ascending = "asc";
        const string descending = "desc";
        public OrderBy(string output, string orderType, List<string> orderByColumns)
        {
            this.output = output;
            this.orderType = orderType;
            this.orderByColumns = orderByColumns;
            if(orderByColumns!= null )
            {
                if (orderByColumns.Count > 0) isPresent = true;
            }
        }
        public string process()
        {
            string output = "";
            if (isPresent)
            {
                output += " ORDER BY ";
                output = HelperClass.addListElements(orderByColumns, output);
                if (orderType == ascending) output += " ASC ";
                if (orderType == descending) output += " DESC ";
            }
            return output;
        }
    }
}
