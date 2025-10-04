namespace ZeroCode2.Interpreter.Evaluator
{
    class LoopEvaluator : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            IteratorManager loop = context.EvaluateLoop(expression);

            bool result = loop.HasMore || loop.CurrentModel != null;
            return new EvaluatorResult(result, string.Empty);
        }
    }
}
