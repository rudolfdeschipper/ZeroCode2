using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZeroCode2;

namespace ZeroCode2UnitTests
{
    [TestClass]
    public class LocatorTests
    {
        public static ModelCollector ModelCollector { get; set; }
        public static Stack<ZeroCode2.Interpreter.IteratorManager> LoopStack { get; set; }

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            var parser = new ModelParser();

            parser.ParseInputFile("ZeroCodeTestInput.txt");

            ModelCollector = parser.ModelCollector;

            LoopStack = null;
        }

        [TestMethod]
        public void TestEnsureSingleModelPathGivesCorrectLoopID()
        {
            var im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels"
            };
            var locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            Assert.IsNotNull(im.LoopID);
            Assert.AreNotEqual(im.LoopID, im.Path);
            Assert.AreEqual("ViewModels", im.LoopID);

            LoopStack = null;
        }

        [TestMethod]
        public void TestLocateWithParameterPath()
        {
            var locator = new ZeroCode2.Models.PropertyLocator("#Parameters.appName", ModelCollector, LoopStack);
            locator.Locate();
            var outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("My test application in Zero Code", outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelPath1()
        {
            var path = "@Models.Person.ID.Name";
            var value = "ID of Person";

            var locator = new ZeroCode2.Models.PropertyLocator(path, ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual(value, outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelPath2()
        {
            var path = "@Models.Stakeholder.Name.Title";
            var value = "Generic Title";

            var locator = new ZeroCode2.Models.PropertyLocator(path, ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual(value, outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelPath3()
        {
            var path = "@DataDictionary.IDField.Name";
            var value = "ID";

            var locator = new ZeroCode2.Models.PropertyLocator(path, ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual(value, outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelPathInheritedOnly()
        {
            var path = "@Models.Person.InheritedField.Title";
            var value = "Generic Title";

            var locator = new ZeroCode2.Models.PropertyLocator(path, ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual(value, outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelPathNotFound()
        {
            var locator = new ZeroCode2.Models.PropertyLocator("@Models.Stakeholder.Name.Nullable", ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            Assert.IsNull(outp);
            //Assert.AreEqual(outp.Value.GetText(), "Title");
        }

        [TestMethod]
        public void TestLocateWithDirectModelInheritedValue()
        {
            var locator = new ZeroCode2.Models.PropertyLocator("@Models.Stakeholder.Name.Length", ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("95", outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelDoubleInheritedValue()
        {
            var locator = new ZeroCode2.Models.PropertyLocator("@Models.Person.DoubleInheritance.Name", ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("Name", outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelDoubleInheritedRemovedValue()
        {
            var locator = new ZeroCode2.Models.PropertyLocator("@Models.Person.InheritedField.MetaProperties.Type", ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            Assert.IsNull(outp);
            locator = new ZeroCode2.Models.PropertyLocator("@Models.Person.InheritedField.MetaProperties.Name", ModelCollector, LoopStack);

            locator.Locate();
            outp = locator.LocatedProperty();
            Assert.IsNotNull(outp);
            Assert.AreEqual("Name", outp.GetText());
        }


        [TestMethod]
        public void TestLocateWithLoopStackPeekTop()
        {
            var im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels"
            };
            var locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);
            locator = new ZeroCode2.Models.PropertyLocator("Name.Length", ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            LoopStack = null;

            Assert.IsNotNull(outp);
            Assert.AreEqual("75", outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithLoopX()
        {
            var im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels"
            };
            var locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);
            locator = new ZeroCode2.Models.PropertyLocator("Loop0.Name.Length", ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            LoopStack = null;

            Assert.IsNotNull(outp);
            Assert.AreEqual("75", outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithLoopExplicit()
        {
            var im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels"
            };
            var locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);
            locator = new ZeroCode2.Models.PropertyLocator("ViewModels.Name.Length", ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            LoopStack = null;

            Assert.IsNotNull(outp);
            Assert.AreEqual("75", outp.GetText());
        }

        [TestMethod]
        public void TestLocateLoopOrdering()
        {

            string[] fields = { "Name", "Title", "InheritedField", "CodeField", "DoubleInheritance", "Test" };
            var im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels.Person"
            };
            var locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);

            int i = 0;
            do
            {
                im.Iterate();
                Assert.IsTrue(i < fields.Length);

                locator = new ZeroCode2.Models.PropertyLocator("$", ModelCollector, LoopStack);

                locator.Locate();
                var outp = locator.LocatedProperty();

                Assert.IsNotNull(outp);
                Assert.AreEqual(fields[i], outp.Name);

                i++;
            } while (im.HasMore);

            Assert.IsTrue(i == fields.Length);

            LoopStack = null;

        }

        [TestMethod]
        public void TestLocateLoopOrderingWithOrderingStatement()
        {

            string[] fields = { "ID", "Code", "Description", "Budget", "Startdate", "Enddate", "Status" };
            var im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@Datamodel.Project.Properties"
            };
            var locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);

            int i = 0;
            do
            {
                im.Iterate();
                Assert.IsTrue(i < fields.Length);

                locator = new ZeroCode2.Models.PropertyLocator("$", ModelCollector, LoopStack);

                locator.Locate();
                var outp = locator.LocatedProperty();

                Assert.IsNotNull(outp);
                Assert.AreEqual(fields[i], outp.Name);

                i++;
            } while (im.HasMore);

            Assert.IsTrue(i == fields.Length);

            LoopStack = null;

        }


        [TestMethod]
        public void TestLocateWithLoopExplicitIteratorName()
        {
            var im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels"
            };
            var locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);
            locator = new ZeroCode2.Models.PropertyLocator("ViewModels.$", ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            LoopStack = null;

            Assert.IsNotNull(outp);
            Assert.AreEqual("Person", outp.Name);
        }

        [TestMethod]
        public void TestLocateWithLoopImplicitIteratorName()
        {
            var im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels"
            };
            var locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);
            locator = new ZeroCode2.Models.PropertyLocator("$", ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("Person", outp.Name);

            locator = new ZeroCode2.Models.PropertyLocator("$.Test.Title", ModelCollector, LoopStack);
            locator.Locate();
            outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("hello", outp.GetText());

            LoopStack = null;

        }

        [TestMethod]
        public void TestIterationWithLoopExplicit()
        {
            var im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@Models"
            };
            var locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);

            locator = new ZeroCode2.Models.PropertyLocator("Models.Name.Title", ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("Name", outp.GetText());

            im.Iterate();

            locator = new ZeroCode2.Models.PropertyLocator("Models.Name.Title", ModelCollector, LoopStack);

            locator.Locate();
            outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("Generic Title", outp.GetText());

            var hasMore = im.Iterate();

            locator = new ZeroCode2.Models.PropertyLocator("Models.Name.Title", ModelCollector, LoopStack);

            locator.Locate();
            outp = locator.LocatedProperty();

            Assert.IsNull(outp);
            Assert.IsFalse(hasMore);

            LoopStack = null;
        }
        [TestMethod]

        public void TestIterationWithLoopImplicit()
        {
            var im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@Models"
            };
            var locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);

            locator = new ZeroCode2.Models.PropertyLocator("Name.Title", ModelCollector, LoopStack);

            locator.Locate();
            var outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("Name", outp.GetText());

            im.Iterate();

            locator = new ZeroCode2.Models.PropertyLocator("Name.Title", ModelCollector, LoopStack);
            locator.Locate();
            outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("Generic Title", outp.GetText());

            var hasMore = im.Iterate();

            locator = new ZeroCode2.Models.PropertyLocator("Models.Name.Title", ModelCollector, LoopStack);
            locator.Locate();
            outp = locator.LocatedProperty();

            Assert.IsNull(outp);
            Assert.IsFalse(hasMore);

            LoopStack = null;
        }
    }
}
