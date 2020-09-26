using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Emitter
{

    public class FilePath : IFilePath
    {
        public bool FileExists(string path)
        {
            return System.IO.File.Exists(path);
        }

        public void WriteToFile(string _uri, StringBuilder sb)
        {
            System.IO.File.WriteAllText(_uri, sb.ToString());
        }

        public void CreateDirectory(string dir)
        {
            System.IO.Directory.CreateDirectory(dir);
        }

        public bool DirectoryExists(string path)
        {
            return System.IO.Directory.Exists(path);
        }


    }
}
