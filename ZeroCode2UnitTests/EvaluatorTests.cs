using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ZeroCode2;

namespace ZeroCode2UnitTests
{
    [TestClass]
    public class EvaluatorTests
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
        public void TestIfWithQuestionMarkValueQ()
        {
            var IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("#Parameters.debug?");
            var context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            var res = IfEval.Evaluate(context, "");

            Assert.IsTrue(res.Value == string.Empty);
            Assert.IsFalse(res.Result == ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.False);
        }

        [TestMethod]
        public void TestIfWithQuestionMarkNoValueQ()
        {
            var IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("#Parameters.Debug?");
            var context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            var res = IfEval.Evaluate(context, "");

            Assert.IsTrue(res.Result == ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.False);
        }

        [TestMethod]
        public void TestIfWithQuestionMarkValueTrue()
        {
            var IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("#Parameters.debug=on");
            var context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            var res = IfEval.Evaluate(context, "");

            Assert.IsTrue(res.Value == string.Empty);
            Assert.IsFalse(res.Result == ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.False);
        }


        [TestMethod]
        public void TestIfWithQuestionMarkValueFalse()
        {
            var IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("#Parameters.debug=off");
            var context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            var res = IfEval.Evaluate(context, "");

            Assert.IsTrue(res.Value == string.Empty);
            Assert.IsFalse(res.Result == ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True);
        }

        [TestMethod]
        public void TestIfWithQuestionMarkNegativeValueTrue()
        {
            var IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("!#Parameters.debug=off");
            var context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            var res = IfEval.Evaluate(context, "");

            Assert.IsTrue(res.Value == string.Empty);
            Assert.IsFalse(res.Result == ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.False);
        }

        [TestMethod]
        public void TestIfWithQuestionMarkNegativeValueFalse()
        {
            var IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("!#Parameters.debug=on");
            var context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            var res = IfEval.Evaluate(context, "");

            Assert.IsTrue(res.Value == string.Empty);
            Assert.IsFalse(res.Result == ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True);
        }

        [TestMethod]
        public void TestEvalWithReference()
        {
            var exprEval = new ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator();
            var context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            var res = exprEval.Evaluate(context, "@Models.Person.[#Parameters.reference].[#Parameters.[#Parameters.reference1]2]ame");

            Assert.IsTrue(res.Value == "Name");
            Assert.IsTrue(res.Result == ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True);
        }

        [TestMethod]
        public void TestEvalWithReferenceInResult()
        {
            var exprEval = new ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator();
            var context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            var res = exprEval.Evaluate(context, "@Models.Person.CodeField");

            Assert.IsTrue(res.Value == "<Input Type='string' >");
            Assert.IsTrue(res.Result == ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True);
        }
    }
}
