using System.Collections.Generic;
using System.Linq;


namespace ZeroCode2.Models.Graph
{
    class ObjectPathStack : Stack<string>
    {
        public string GetPath()
        {
            string fullpath = "";
            IEnumerable<string> stackReverse = this.Reverse();
            stackReverse.Aggregate(fullpath, (f, run) => fullpath += run + ".");
            return fullpath.Substring(0, fullpath.Length - 1);
        }
    }
}
