using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    public class FilepathResolver
    {
        public List<string> list = new List<string>();

        public string ResolvePath(IInterpreterContext context, string expression)
        {
            string path = "";
            // split in string and expressions
            int start = 0, xprIndex, xprEnd;

            while(start < expression.Length)
            { 
                string walk = expression.Substring(start);
                xprIndex = walk.IndexOf("=<");
                xprEnd = walk.IndexOf(">");
                if (xprIndex >= 0 && xprEnd > xprIndex)
                {
                    if (xprIndex != 0)
                    {
                        // before the expression
                        list.Add(walk.Substring(0, xprIndex));
                    }
                    // expression itself
                    xprIndex += 2;
                    // evaluate expression
                    var result = context.EvaluateProperty(walk.Substring(xprIndex, xprEnd - xprIndex));

                    list.Add(result);
                    start += xprEnd + 1;
                }
                else
                {
                    list.Add(walk);
                    break;
                }
            }
            // reconstruct result string
            path = list.Aggregate(path, (c, s) => c += s);
            return path;
        }
    }
}
