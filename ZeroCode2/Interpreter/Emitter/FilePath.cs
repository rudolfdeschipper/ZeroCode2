using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Emitter
{
    public class NullFilePath : IFilePath
    {

        public bool AssumeDirectoryExists { get; set; } = true;
        public bool AssumeFileExists { get; set; } = true;

        public void CreateDirectory(string dir)
        {
            ;
        }

        public bool DirectoryExists(string path)
        {
            return AssumeDirectoryExists;
        }

        public bool FileExists(string path)
        {
            return AssumeFileExists;
        }

        public void WriteToFile(string _uri, StringBuilder sb)
        {
            ;
        }
    }

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
