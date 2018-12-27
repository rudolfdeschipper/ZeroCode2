using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZeroCode2;

namespace ZeroCode2UnitTests
{
    [TestClass]
    public class LocatorTests
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
        public void TestLocateWithParameterPath()
        {
            var locator = new ZeroCode2.Models.PropertyLocator("#Parameters.appName", ModelCollector, LoopStack);

            var outp = locator.Locate();

            Assert.IsNotNull(outp);
            Assert.AreEqual(outp.GetText(), "My test application in Zero Code");
        }

        [TestMethod]
        public void TestLocateWithDirectModelPath1()
        {
            var path = "@Models.Person.ID.Name";
            var value = "ID of Person";

            var locator = new ZeroCode2.Models.PropertyLocator(path, ModelCollector, LoopStack);

            var outp = locator.Locate();

            Assert.IsNotNull(outp);
            Assert.AreEqual(outp.GetText(), value);
        }

        [TestMethod]
        public void TestLocateWithDirectModelPath2()
        {
            var path = "@Models.Stakeholder.Name.Title";
            var value = "Generic Title";

            var locator = new ZeroCode2.Models.PropertyLocator(path, ModelCollector, LoopStack);

            var outp = locator.Locate();

            Assert.IsNotNull(outp);
            Assert.AreEqual(outp.GetText(), value);
        }

        [TestMethod]
        public void TestLocateWithDirectModelPath3()
        {
            var path = "@DataDictionary.IDField.Name";
            var value = "ID";

            var locator = new ZeroCode2.Models.PropertyLocator(path, ModelCollector, LoopStack);

            var outp = locator.Locate();

            Assert.IsNotNull(outp);
            Assert.AreEqual(outp.GetText(), value);
        }

        [TestMethod]
        public void TestLocateWithDirectModelPathNotFound()
        {
            var locator = new ZeroCode2.Models.PropertyLocator("@Models.Stakeholder.Name.Nullable", ModelCollector, LoopStack);

            var outp = locator.Locate();

            Assert.IsNull(outp);
            //Assert.AreEqual(outp.Value.GetText(), "Title");
        }

        [TestMethod]
        public void TestLocateWithLoopStackSection()
        {
            var im = new ZeroCode2.Interpreter.IteratorManager();
            var it = new ZeroCode2.Models.Iterator();
            im.Iterator = it;
            im.Path = "@ViewModels";
            im.Root = (new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null)).Locate();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);
            var locator = new ZeroCode2.Models.PropertyLocator("Name.Length", ModelCollector, LoopStack);

            var outp = locator.Locate();

            LoopStack = null;

            Assert.IsNotNull(outp);
            Assert.AreEqual(outp.GetText(), "75");
        }
    }
}
