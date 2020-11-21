namespace ZeroCode2.Interpreter.Evaluator
{
    class EnterLoopEvaluator : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            context.EnterLoop(expression);
            return new EvaluatorResult(true, string.Empty);
        }
    }
}
