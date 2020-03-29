using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZeroCode2;
using ZeroCode2.Interpreter;
using ZeroCode2.Interpreter.Emitter;

namespace ZeroCode2UnitTests
{
    class TestFilePath : IFilePath
    {
        public bool AssumeDirectoryExists { get; set; } = true;
        public bool AssumeFileExists { get; set; } = true;

        public System.Collections.Generic.Stack<string> CreatedDirectories { get; set; } = new System.Collections.Generic.Stack<string>();
        public string Uri { get; set; }
        public string Contents { get; set; }

        public void CreateDirectory(string dir)
        {
            CreatedDirectories.Push(dir);
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
            Uri = _uri;
            Contents = sb.ToString();
        }
    }


    class UTInterpreterContext : ZeroCode2.Interpreter.IInterpreterContext
    {
        public IEmitter Emitter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ModelCollector Model { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void EnterLoop(string expression)
        {
            throw new NotImplementedException();
        }

        public IteratorManager EvaluateLoop(string expression)
        {
            throw new NotImplementedException();
        }

        public string EvaluateProperty(string expression)
        {
            return expression;
        }

        public void ExitLoop(string expression)
        {
            throw new NotImplementedException();
        }

        public void EmitResult(string Result)
        {
            throw new NotImplementedException();
        }

        public bool PropertyExists(string expression)
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class FilePathResolverUnitTest
    {
        [TestMethod]
        public void TestPathStringOnly()
        {
            ZeroCode2.Interpreter.Evaluator.FilepathResolver filepathResolver = new ZeroCode2.Interpreter.Evaluator.FilepathResolver();

            const string Expression = "c:\\documents\\test\\file.cs";
            var res = filepathResolver.ResolvePath(new UTInterpreterContext(), Expression);
            var list = filepathResolver.list;
            Assert.IsTrue(list.Count == 1);
            Assert.AreEqual(list[0], Expression);
            Assert.AreEqual(res, "c:\\documents\\test\\file.cs");
        }

        [TestMethod]
        public void TestPathOneExpression()
        {
            ZeroCode2.Interpreter.Evaluator.FilepathResolver filepathResolver = new ZeroCode2.Interpreter.Evaluator.FilepathResolver();

            const string Expression = "c:\\documents\\=<test>\\file.cs";
            var res = filepathResolver.ResolvePath(new UTInterpreterContext(), Expression);
            var list = filepathResolver.list;
            Assert.IsTrue(list.Count == 3);
            Assert.AreNotEqual(list[0], Expression);
            Assert.AreEqual(list[1], "test");
            Assert.AreEqual(res, "c:\\documents\\test\\file.cs");

        }
        [TestMethod]
        public void TestPathMultipleExpressions()
        {
            ZeroCode2.Interpreter.Evaluator.FilepathResolver filepathResolver = new ZeroCode2.Interpreter.Evaluator.FilepathResolver();

            const string Expression = "c:\\documents\\=<test>\\=<file>.cs";
            var res = filepathResolver.ResolvePath(new UTInterpreterContext(), Expression);
            var list = filepathResolver.list;
            Assert.IsTrue(list.Count == 5);
            Assert.AreNotEqual(list[0], Expression);
            Assert.AreEqual(list[1], "test");
            Assert.AreEqual(list[3], "file");
            Assert.AreEqual(list[4], ".cs");
            Assert.AreEqual(res, "c:\\documents\\test\\file.cs");
        }

        [TestMethod]
        public void TestPathExpressionOnly()
        {
            ZeroCode2.Interpreter.Evaluator.FilepathResolver filepathResolver = new ZeroCode2.Interpreter.Evaluator.FilepathResolver();

            const string Expression = "=<test>";
            var res = filepathResolver.ResolvePath(new UTInterpreterContext(), Expression);
            var list = filepathResolver.list;
            Assert.IsTrue(list.Count == 1);
            Assert.AreNotEqual(list[0], Expression);
            Assert.AreEqual(list[0], "test");
            Assert.AreEqual(res, "test");
        }
        [TestMethod]
        public void TestPathEmptyString()
        {
            ZeroCode2.Interpreter.Evaluator.FilepathResolver filepathResolver = new ZeroCode2.Interpreter.Evaluator.FilepathResolver();

            const string Expression = "";
            var res = filepathResolver.ResolvePath(new UTInterpreterContext(), Expression);
            var list = filepathResolver.list;
            Assert.IsTrue(list.Count == 0);
            Assert.AreEqual(res, "");
        }

        [TestMethod]
        public void TestFilePathClassWithPathEnding()
        {

            TestFilePath fp = new TestFilePath();
            FileEmitter fe = new FileEmitter
            {
                FilePath = fp
            };

            fp.AssumeDirectoryExists = false;
            fp.AssumeFileExists = true;

            fe.OutputPath = @"C:\temp\a\b\";

            fe.EnsurePathExists();

            Assert.AreEqual(4, fp.CreatedDirectories.Count);
            Assert.AreEqual(fe.OutputPath, fp.CreatedDirectories.Peek());
        }

        [TestMethod]
        public void TestFilePathClassWithPathNotEnding()
        {

            TestFilePath fp = new TestFilePath();
            FileEmitter fe = new FileEmitter
            {
                FilePath = fp
            };

            fp.AssumeDirectoryExists = false;
            fp.AssumeFileExists = true;

            fe.OutputPath = @"C:\temp\a\b";

            fe.EnsurePathExists();

            Assert.AreEqual(4, fp.CreatedDirectories.Count);
            Assert.IsTrue(fp.CreatedDirectories.Peek().EndsWith(@"b\"));
        }

        [TestMethod]
        public void TestFilePathClassOutput()
        {

            TestFilePath fp = new TestFilePath();
            FileEmitter fe = new FileEmitter
            {
                FilePath = fp
            };

            fp.AssumeDirectoryExists = false;
            fp.AssumeFileExists = true;

            fe.OutputPath = @"C:\temp\a\b";

            fe.Open("output.txt");
            fe.Emit("output");
            fe.Close();

            Assert.AreEqual(4, fp.CreatedDirectories.Count);
            Assert.IsTrue(fp.CreatedDirectories.Peek().EndsWith(@"b\"));
            Assert.AreEqual(@"C:\temp\a\b\output.txt", fp.Uri);
            Assert.AreEqual(@"output", fp.Contents);
        }

        [TestMethod]
        public void TestFilePathClassOutputWithPathInFile()
        {

            TestFilePath fp = new TestFilePath();
            FileEmitter fe = new FileEmitter
            {
                FilePath = fp
            };

            fp.AssumeDirectoryExists = false;
            fp.AssumeFileExists = true;

            fe.OutputPath = @"C:\temp\a\b\";

            fe.Open(@"generated\component\output.txt");
            fe.Emit("output");
            fe.Close();

            Assert.AreEqual(6, fp.CreatedDirectories.Count);
            Assert.IsTrue(fp.CreatedDirectories.Peek().EndsWith(@"component\"));
            Assert.AreEqual(@"C:\temp\a\b\generated\component\output.txt", fp.Uri);
            Assert.AreEqual(@"output", fp.Contents);
        }
    }
}
