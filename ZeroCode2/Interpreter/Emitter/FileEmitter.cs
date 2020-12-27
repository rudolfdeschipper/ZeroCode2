using System.IO;
using System.Linq;
using System.Text;

namespace ZeroCode2.Interpreter.Emitter
{
    public class FileEmitter : IEmitter
    {
        readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private System.Text.StringBuilder _sb;
        private string _uri;

        private string FullPath = "";

        public string OutputPath { get; set; }

        public IFilePath FilePath { get; set; } = new FilePath();

        public bool EnsurePathExists(string uri, bool doCreate = true)
        {
            bool retVal = true;

            logger.Trace("EnsurePathExists Create paths: {0}. Full path: {1}", doCreate, OutputPath);
            if (!OutputPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                OutputPath += Path.DirectorySeparatorChar.ToString();
            }
            string fullPath = OutputPath + uri;

            string[] pathParts = fullPath.Split(Path.DirectorySeparatorChar);

            // extract file
            string file = pathParts.Last();
            pathParts[pathParts.Length - 1] = "";

            string currentPath = "";

            foreach (var item in pathParts.Where(s => s.Length > 0))
            {
                currentPath += item + Path.DirectorySeparatorChar.ToString();
                if (!FilePath.DirectoryExists(currentPath))
                {
                    if (doCreate)
                    {
                        logger.Trace("Create subdirectory {0}", currentPath);
                        FilePath.CreateDirectory(currentPath);
                    }
                    else
                    {
                        logger.Trace("Subdirectory {0} does not exist, but will not be created.", currentPath);
                        retVal = false;
                        break;
                    }
                }
            }

            FullPath = Path.Combine(currentPath, file);

            return retVal;
        }

        public void Close()
        {
            logger.Info("Closing: " + _uri);

            // create path if not there yet
            EnsurePathExists(_uri, true);

            FilePath.WriteToFile(FullPath, _sb);
        }

        public void Emit(string output)
        {
            logger.Trace("Emitting: " + output);
            if (_sb != null)
            {
                if (!logger.IsEnabled(NLog.LogLevel.Trace))
                {
                    logger.Debug("Emitting: " + output);
                }
                _sb.Append(output);
            }
            else
            {
                logger.Warn("Output: '" + output + "' was not emitted, no file open");
            }
        }

        public bool Exists(string fileName)
        {
            if (!EnsurePathExists(fileName, false))
            {
                // directory path did not even exist, so for sure the file doesn't either
                return false;
            }
            return FilePath.FileExists(FullPath);
        }

        public void Open(string uri)
        {
            _uri = uri;

            logger.Info("Opening: " + _uri);

            _sb = new StringBuilder();
        }
    }
}
