namespace ZeroCode2.Interpreter.Evaluator
{
    class IndexExpressionEvaluator : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            return new EvaluatorResult(true, context.EvaluateLoop(expression).Index.ToString());
        }
    }
}
