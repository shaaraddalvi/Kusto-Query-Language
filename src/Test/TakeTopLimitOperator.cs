using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class TakeTopLimitOperator
    {
        string output;
        Boolean isPresent = false;
        List<string> take_Literals;
        List<string> take_mathOperators;

        public TakeTopLimitOperator(string output , List<string> take_Literals, List<string> take_mathOperators)
        {
            this.output = output;
            this.take_Literals = take_Literals;
            this.take_mathOperators = take_mathOperators;
            if (take_Literals != null)
            {
                if(take_Literals.Count > 0) isPresent = true;
            }
                
        }

        public string process()
        {
            //string output = "";
            if (isPresent)
            {
                output += " TOP ";
                if (take_mathOperators.Count == 0)
                {
                    output = HelperClass.addListElements(take_Literals, output);
                }
                else
                {
                    for (int i = 0; i < take_Literals.Count; i++)
                    {
                        if (i != take_Literals.Count - 1)
                            output += take_Literals[i] + take_mathOperators[i];
                        else output += take_Literals[i];
                    }
                }
            }
            return output;
        }
    }
}
