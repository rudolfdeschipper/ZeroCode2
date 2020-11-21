namespace ZeroCode2.Interpreter.Evaluator
{
    class HasMoreExpressionEvaluator : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            return new EvaluatorResult(true, context.EvaluateLoop(expression).HasMore.ToString());
        }
    }
}
