using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using System.Collections.Generic;
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
            NLog.LogManager.GlobalThreshold = LogLevel.Warn;
            ModelParser parser = new ModelParser();

            parser.ParseInputFile("ZeroCodeTestInput.txt");

            ModelCollector = parser.ModelCollector;

            LoopStack = null;
        }

        [TestMethod]
        public void TestEnsureSingleModelPathGivesCorrectLoopID()
        {
            ZeroCode2.Interpreter.IteratorManager im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels"
            };
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
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
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator("#Parameters.appName", ModelCollector, LoopStack);
            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("My test application in Zero Code", outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelPath1()
        {
            string path = "@Models.Person.ID.Name";
            string value = "ID of Person";

            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(path, ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual(value, outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelPath2()
        {
            string path = "@Models.Stakeholder.Name.Title";
            string value = "Generic Title";

            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(path, ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual(value, outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelPath3()
        {
            string path = "@DataDictionary.IDField.Name";
            string value = "ID";

            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(path, ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual(value, outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelPathInheritedOnly()
        {
            string path = "@Models.Person.InheritedField.Title";
            string value = "Generic Title";

            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(path, ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual(value, outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelPathNotFound()
        {
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator("@Models.Stakeholder.Name.Nullable", ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            Assert.IsNull(outp);
            //Assert.AreEqual(outp.Value.GetText(), "Title");
        }

        [TestMethod]
        public void TestLocateWithDirectModelInheritedValue()
        {
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator("@Models.Stakeholder.Name.Length", ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("95", outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelDoubleInheritedValue()
        {
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator("@Models.Person.DoubleInheritance.Name", ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("Name", outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithDirectModelDoubleInheritedRemovedValue()
        {
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator("@Models.Person.InheritedField.MetaProperties.Type", ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

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
            ZeroCode2.Interpreter.IteratorManager im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels"
            };
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);
            locator = new ZeroCode2.Models.PropertyLocator("Name.Length", ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            LoopStack = null;

            Assert.IsNotNull(outp);
            Assert.AreEqual("75", outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithLoopX()
        {
            ZeroCode2.Interpreter.IteratorManager im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels"
            };
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);
            locator = new ZeroCode2.Models.PropertyLocator("Loop0.Name.Length", ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            LoopStack = null;

            Assert.IsNotNull(outp);
            Assert.AreEqual("75", outp.GetText());
        }

        [TestMethod]
        public void TestLocateWithLoopExplicit()
        {
            ZeroCode2.Interpreter.IteratorManager im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels"
            };
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);
            locator = new ZeroCode2.Models.PropertyLocator("ViewModels.Name.Length", ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            LoopStack = null;

            Assert.IsNotNull(outp);
            Assert.AreEqual("75", outp.GetText());
        }

        [TestMethod]
        public void TestLocateLoopOrdering()
        {

            string[] fields = { "Name", "Title", "InheritedField", "CodeField", "DoubleInheritance", "Test" };
            ZeroCode2.Interpreter.IteratorManager im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels.Person"
            };
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);

            int i = 0;
            do
            {
                im.Iterate();
                Assert.IsLessThan(fields.Length, i);

                locator = new ZeroCode2.Models.PropertyLocator("$", ModelCollector, LoopStack);

                locator.Locate();
                ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

                Assert.IsNotNull(outp);
                Assert.AreEqual(fields[i], outp.Name);

                i++;
            } while (im.HasMore);

            Assert.AreEqual(fields.Length, i);

            LoopStack = null;

        }

        [TestMethod]
        public void TestLocateLoopOrderingWithOrderingStatement()
        {

            string[] fields = { "ID", "Code", "Description", "Budget", "Startdate", "Enddate", "Status" };
            ZeroCode2.Interpreter.IteratorManager im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@Datamodel.Project.Properties"
            };
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);

            int i = 0;
            do
            {
                im.Iterate();
                Assert.IsLessThan(fields.Length, i);

                locator = new ZeroCode2.Models.PropertyLocator("$", ModelCollector, LoopStack);

                locator.Locate();
                ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

                Assert.IsNotNull(outp);
                Assert.AreEqual(fields[i], outp.Name);

                i++;
            } while (im.HasMore);

            Assert.AreEqual(fields.Length, i);

            LoopStack = null;

        }


        [TestMethod]
        public void TestLocateWithLoopExplicitIteratorName()
        {
            ZeroCode2.Interpreter.IteratorManager im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels"
            };
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);
            locator = new ZeroCode2.Models.PropertyLocator("ViewModels.$", ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            LoopStack = null;

            Assert.IsNotNull(outp);
            Assert.AreEqual("Person", outp.Name);
        }

        [TestMethod]
        public void TestLocateWithLoopImplicitIteratorName()
        {
            ZeroCode2.Interpreter.IteratorManager im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@ViewModels"
            };
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);
            locator = new ZeroCode2.Models.PropertyLocator("$", ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

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
            ZeroCode2.Interpreter.IteratorManager im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@Models"
            };
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);

            locator = new ZeroCode2.Models.PropertyLocator("Models.Name.Title", ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("Name", outp.GetText());

            im.Iterate();

            locator = new ZeroCode2.Models.PropertyLocator("Models.Name.Title", ModelCollector, LoopStack);

            locator.Locate();
            outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("Generic Title", outp.GetText());

            bool hasMore = im.Iterate();

            locator = new ZeroCode2.Models.PropertyLocator("Models.Name.Title", ModelCollector, LoopStack);

            locator.Locate();
            outp = locator.LocatedProperty();

            Assert.IsNull(outp);
            Assert.IsFalse(hasMore);

            LoopStack = null;
        }

        [TestMethod]
        public void TestVarDecl()
        {
            ZeroCode2.Interpreter.Evaluator.EvaluateVariable exprEval = new ZeroCode2.Interpreter.Evaluator.EvaluateVariable();

            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            exprEval.Evaluate(context, "Var=Value");

            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator("Var", ModelCollector, null);
            locator.Locate();
            ZeroCode2.Models.IModelObject res = locator.LocatedProperty();

            Assert.AreEqual("Value", res.GetText());
        }

        [TestMethod]
        public void TestVarDeclRemove()
        {
            ZeroCode2.Interpreter.Evaluator.EvaluateVariable exprEval = new ZeroCode2.Interpreter.Evaluator.EvaluateVariable();

            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector,
            }; exprEval.Evaluate(context, "Var=Value");
            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();

            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator("Var", ModelCollector, LoopStack);
            locator.Locate();
            ZeroCode2.Models.IModelObject res = locator.LocatedProperty();

            Assert.AreEqual("Value", res.GetText());


            exprEval.Evaluate(context, "Var=Value2");

            locator = new ZeroCode2.Models.PropertyLocator("Var", ModelCollector, LoopStack);
            locator.Locate();
            res = locator.LocatedProperty();

            Assert.AreEqual("Value2", res.GetText());

            // remove the variable
            exprEval.Evaluate(context, "Var=");
            locator = new ZeroCode2.Models.PropertyLocator("Var", ModelCollector, LoopStack);
            locator.Locate();
            res = locator.LocatedProperty();

            Assert.IsNull(res);
        }

        [TestMethod]

        public void TestIterationWithLoopImplicit()
        {
            ZeroCode2.Interpreter.IteratorManager im = new ZeroCode2.Interpreter.IteratorManager(new ZeroCode2.Models.Iterator())
            {
                Path = "@Models"
            };
            ZeroCode2.Models.PropertyLocator locator = new ZeroCode2.Models.PropertyLocator(im.Path, ModelCollector, null);
            locator.Locate();
            im.Root = locator.LocatedProperty();

            LoopStack = new Stack<ZeroCode2.Interpreter.IteratorManager>();
            LoopStack.Push(im);

            locator = new ZeroCode2.Models.PropertyLocator("Name.Title", ModelCollector, LoopStack);

            locator.Locate();
            ZeroCode2.Models.IModelObject outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("Name", outp.GetText());

            im.Iterate();

            locator = new ZeroCode2.Models.PropertyLocator("Name.Title", ModelCollector, LoopStack);
            locator.Locate();
            outp = locator.LocatedProperty();

            Assert.IsNotNull(outp);
            Assert.AreEqual("Generic Title", outp.GetText());

            bool hasMore = im.Iterate();

            locator = new ZeroCode2.Models.PropertyLocator("Models.Name.Title", ModelCollector, LoopStack);
            locator.Locate();
            outp = locator.LocatedProperty();

            Assert.IsNull(outp);
            Assert.IsFalse(hasMore);

            LoopStack = null;
        }
    }
}
