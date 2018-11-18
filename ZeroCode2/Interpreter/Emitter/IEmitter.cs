using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Emitter
{
    public interface IEmitter
    {
        void Open(string uri);
        void Emit(string output);
        void Close();
    }
}
