using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Emitter
{
    class NullEmitter : IEmitter
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private string _uri;

        public void Close()
        {
            logger.Info("Closing null file: " + _uri);
            // nothing
        }

        public void Emit(string output)
        {
            // do nothing
            logger.Info("Emitting: " + output);
        }

        public void Open(string uri)
        {
            // nothing
            _uri = uri;
            logger.Info("Opening null file: " + _uri);
        }
    }
}
