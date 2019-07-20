using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZeroCode2;


namespace ZeroCode2UnitTests
{
    [TestClass]
    public class InheritanceTests
    {
        public ModelCollector ModelCollector { get; set; }
        public Stack<ZeroCode2.Interpreter.IteratorManager> LoopStack { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var parser = new ModelParser();

            parser.ParseInputFile("ZeroCodeTestInput.txt");

            ModelCollector = parser.modelCollector;

            LoopStack = null;
        }

        [TestMethod]
        public void TestGraphwalker()
        {
            var resolver = new ZeroCode2.Models.InheritanceResolver();
            var outp = true;

            ModelCollector.SingleModels.ForEach(m => outp &= resolver.ResolveInheritance(m, ModelCollector));

            Assert.IsTrue(outp);
            Assert.AreEqual(resolver.Errors.Count, 0);
        }


    }
}
