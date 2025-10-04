using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
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
            NLog.LogManager.GlobalThreshold = LogLevel.Warn;
            ModelParser parser = new ModelParser();

            parser.ParseInputFile("ZeroCodeTestInput.txt");

            ModelCollector = parser.ModelCollector;

            LoopStack = null;
        }

        [TestMethod]
        public void TestIfWithQuestionMarkValueQ()
        {
            ZeroCode2.Interpreter.Evaluator.IfEvaluator IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("#Parameters.debug?");
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = IfEval.Evaluate(context, "");

            Assert.AreEqual(string.Empty, res.Value);
            Assert.AreNotEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.False, res.Result);
        }

        [TestMethod]
        public void TestIfWithQuestionMarkNoValueQ()
        {
            ZeroCode2.Interpreter.Evaluator.IfEvaluator IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("#Parameters.Debug?");
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = IfEval.Evaluate(context, "");

            Assert.AreEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.False, res.Result);
        }

        [TestMethod]
        public void TestIfWithQuestionMarkValueTrue()
        {
            ZeroCode2.Interpreter.Evaluator.IfEvaluator IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("#Parameters.debug=on");
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = IfEval.Evaluate(context, "");

            Assert.AreEqual(string.Empty, res.Value);
            Assert.AreNotEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.False, res.Result);
        }


        [TestMethod]
        public void TestIfWithQuestionMarkValueFalse()
        {
            ZeroCode2.Interpreter.Evaluator.IfEvaluator IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("#Parameters.debug=off");
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = IfEval.Evaluate(context, "");

            Assert.AreEqual(string.Empty, res.Value);
            Assert.AreNotEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True, res.Result);
        }

        [TestMethod]
        public void TestIfWithNumerical()
        {
            ZeroCode2.Interpreter.Evaluator.IfEvaluator IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("#Parameters.number=42");
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = IfEval.Evaluate(context, "");

            Assert.AreEqual(string.Empty, res.Value);
            Assert.AreEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True, res.Result);
        }


        [TestMethod]
        public void TestIfWithQuestionMarkNegativeValueTrue()
        {
            ZeroCode2.Interpreter.Evaluator.IfEvaluator IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("!#Parameters.debug=off");
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = IfEval.Evaluate(context, "");

            Assert.AreEqual(string.Empty, res.Value);
            Assert.AreNotEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.False, res.Result);
        }

        [TestMethod]
        public void TestIfWithQuestionMarkNegativeValueFalse()
        {
            ZeroCode2.Interpreter.Evaluator.IfEvaluator IfEval = new ZeroCode2.Interpreter.Evaluator.IfEvaluator("!#Parameters.debug=on");
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = IfEval.Evaluate(context, "");

            Assert.AreEqual(string.Empty, res.Value);
            Assert.AreNotEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True, res.Result);
        }

        [TestMethod]
        public void TestEvalWithReference()
        {
            ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator exprEval = new ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator();
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = exprEval.Evaluate(context, "@Models.Person.[#Parameters.reference].[#Parameters.[#Parameters.reference1]2]ame");

            Assert.AreEqual("Name", res.Value);
            Assert.AreEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True, res.Result);
        }

        [TestMethod]
        public void TestEvalWithReferenceInResult()
        {
            ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator exprEval = new ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator();
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = exprEval.Evaluate(context, "@Models.Person.CodeField");

            Assert.AreEqual("<Input Type='string' >", res.Value);
            Assert.AreEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True, res.Result);
        }

        [TestMethod]
        public void TestEscapeCharacters()
        {
            ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator exprEval = new ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator();
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = exprEval.Evaluate(context, "@Models.Quotes.QuoteField");

            Assert.AreEqual("This is a quote: \"", res.Value);
            Assert.AreEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True, res.Result);
        }

        [TestMethod]
        public void TestTabCharacters()
        {
            ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator exprEval = new ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator();
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = exprEval.Evaluate(context, "@Models.Quotes.EscapedTabField");

            Assert.AreEqual("This is a tab: \\b", res.Value);
            Assert.AreEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True, res.Result);
        }
        [TestMethod]
        public void TestBackslashCharacters()
        {
            ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator exprEval = new ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator();
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = exprEval.Evaluate(context, "@Models.Quotes.EscapedBackslashField");

            Assert.AreEqual("This is a backslash: \\", res.Value);
            Assert.AreEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True, res.Result);
        }

        [TestMethod]
        public void TestEscapedQuoteCharacters()
        {
            ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator exprEval = new ZeroCode2.Interpreter.Evaluator.ExpressionEvaluator();
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            ZeroCode2.Interpreter.Evaluator.EvaluatorResult res = exprEval.Evaluate(context, "@Models.Quotes.EscapedQuoteField");

            Assert.AreEqual("This is an escaped quote: \\\"", res.Value);
            Assert.AreEqual(ZeroCode2.Interpreter.Evaluator.EvaluationResultValues.True, res.Result);
        }
        [TestMethod]
        public void TestOptionalLoop()
        {
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            context.EnterLoop("@Screen.Person.Limbs?");
            ZeroCode2.Interpreter.IteratorManager result = context.EvaluateLoop("@Screen.Person.Limbs");
            bool res = result.Iterate();

            Assert.IsFalse(res);
        }

        [TestMethod]
        public void TestNonExistingLoopGivesError()
        {
            ZeroCode2.Interpreter.InterpreterContext context = new ZeroCode2.Interpreter.InterpreterContext
            {
                Model = ModelCollector
            };

            Assert.ThrowsExactly<System.ApplicationException>(() =>
            {
                context.EnterLoop("@Screen.Person.Limbs");
            });
        }

    }
}
