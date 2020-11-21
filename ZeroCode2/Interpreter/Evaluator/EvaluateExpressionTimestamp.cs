using System;

namespace ZeroCode2.Interpreter.Evaluator
{
    class TimestampExpressionEvaluator : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            return new EvaluatorResult(true, DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss"));
        }
    }
}
