using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZeroCode2;
using ZeroCode2.Models.Graph;

namespace ZeroCode2UnitTests
{
    [TestClass]
    public class InheritanceTests
    {
        public static ModelCollector ModelCollector { get; set; }
        public static Stack<ZeroCode2.Interpreter.IteratorManager> LoopStack { get; set; }

        public static Dictionary<string, GraphElement> GraphElements { get; set; }

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            var parser = new ModelParser();

            parser.ParseInputFile("ZeroCodeTestInput.txt");

            ModelCollector = parser.ModelCollector;

            GraphElements = parser.GraphElements;

            LoopStack = null;
        }

        [TestMethod]
        public void TestGraphwalker()
        {
            var resolver = new ZeroCode2.Models.Graph.InheritanceGraphBuilder();
            var outp = true;

            resolver.Elements = GraphElements;

            outp = resolver.BuildGraph();

            Assert.IsTrue(outp);
            Assert.AreEqual(resolver.Errors.Count, 0);
        }


    }
}
