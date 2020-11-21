namespace ZeroCode2.Interpreter.Evaluator
{
    class NoOpEvaluator : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            return new EvaluatorResult(true, string.Empty);
        }
    }
}
