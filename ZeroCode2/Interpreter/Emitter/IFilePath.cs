using System.Text;

namespace ZeroCode2.Interpreter.Emitter
{
    public interface IFilePath
    {
        void CreateDirectory(string dir);
        bool DirectoryExists(string path);
        bool FileExists(string path);
        void WriteToFile(string _uri, StringBuilder sb);
    }
}