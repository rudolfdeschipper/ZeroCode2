namespace ZeroCode2.Interpreter.Evaluator
{
    class ExitLoopEvaluator : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            context.ExitLoop(expression);
            return new EvaluatorResult(true, string.Empty);
        }
    }
}
