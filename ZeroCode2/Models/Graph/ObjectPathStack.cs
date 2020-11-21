﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ZeroCode2.Models.Graph
{
    class ObjectPathStack : Stack<string>
    {
        public string GetPath()
        {
            var fullpath = "";
            var stackReverse = this.Reverse();
            stackReverse.Aggregate(fullpath, (f, run) => fullpath += run + ".");
            return fullpath.Substring(0,fullpath.Length-1);
        }
    }
}
