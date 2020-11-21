namespace ZeroCode2.Interpreter.Evaluator
{
    class EvaluateFileCreate : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            var resolver = new FilepathResolver();
            string expr = resolver.ResolvePath(context, expression);

            if (context.Emitter.Exists(expr))
            {
                return new EvaluatorResult(false, string.Empty);
            }
            else
            {
                context.Emitter.Open(expr);
                return new EvaluatorResult(true, string.Empty);
            }
        }
    }
}
