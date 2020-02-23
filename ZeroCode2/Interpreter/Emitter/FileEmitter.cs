using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Emitter
{
    class FileEmitter : IEmitter
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private System.Text.StringBuilder _sb;
        private string _uri;

        public void Close()
        {
            logger.Info("Closing: " + _uri);

            System.IO.File.WriteAllText(_uri, _sb.ToString());
        }

        public void Emit(string output)
        {
            logger.Info("Emitting: " + output);
            if (_sb != null)
            {
                if (!string.IsNullOrWhiteSpace(output))
                {
                    logger.Info("Emitting: " + output);
                    _sb.Append(output);
                }
            }
            else
            {
                logger.Warn("Output: '" + output + "' was not emitted, no file open");
            }
        }

        public bool Exists(string fileName)
        {
            return System.IO.File.Exists(fileName);
        }

        public void Open(string uri)
        {
            logger.Info("Opening: " + _uri);

            _uri = uri;
            _sb = new StringBuilder();
        }
    }
}
