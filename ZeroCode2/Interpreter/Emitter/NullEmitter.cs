using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Emitter
{
    class NullEmitter : FileEmitter
    {
        public new IFilePath FilePath { get; set; } = new NullFilePath();
    }
}
