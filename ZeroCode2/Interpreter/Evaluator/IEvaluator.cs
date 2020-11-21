namespace ZeroCode2.Interpreter.Evaluator
{
    public interface IEvaluator
    {
        EvaluatorResult Evaluate(IInterpreterContext context, string expression);
    }

    public enum EvaluationResultValues
    {
        True,
        False,
        Failed
    }

}
