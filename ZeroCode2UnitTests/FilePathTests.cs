using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZeroCode2;
using ZeroCode2.Interpreter;
using ZeroCode2.Interpreter.Emitter;

namespace ZeroCode2UnitTests
{

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

    }
}
