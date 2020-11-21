namespace ZeroCode2.Interpreter.Evaluator
{
    class OrdinalExpressionEvaluator : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            return new EvaluatorResult(true, (context.EvaluateLoop(expression).Index + 1).ToString());
        }
    }
}
