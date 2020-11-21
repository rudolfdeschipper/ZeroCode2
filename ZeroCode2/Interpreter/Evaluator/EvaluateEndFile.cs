namespace ZeroCode2.Interpreter.Evaluator
{
    class EvaluateEndFile : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            context.Emitter.Close();
            return new EvaluatorResult(true, string.Empty);
        }
    }
}
