using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class HelperClass
    {
        public static string addListElements(List<string> list, string output)
        {
            string temp = output;
            for (int i = 0; i < list.Count; i++)
            {
                temp += list[i];
                if (i != list.Count - 1) temp += ", ";
            }
            return temp;
        }
        public static string removeBrackets(string input)
        {
            if (input.Length < 2) return input;
            if (input[0] == '(')
            {
                StringBuilder sb = new StringBuilder(input);
                sb.Remove(0, 1);
                input = sb.ToString();
            }
            if (input[input.Length - 1] == ')')
            {
                StringBuilder sb = new StringBuilder(input);
                sb.Remove(input.Length - 1, 1);
                input = sb.ToString();
            }
            return input;
        }

    }
}
