namespace ZeroCode2.Interpreter.Evaluator
{
    class LiteralEvaluator : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            return new EvaluatorResult(true, expression);
        }
    }
}
