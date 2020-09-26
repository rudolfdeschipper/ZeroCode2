using System.Text;

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
}
