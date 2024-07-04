namespace ZeroCode2.Interpreter.Evaluator
{
    public class EvaluateVariable : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            var variable = expression.Split('=');
            if (variable.Length == 2)
            {
                var name = variable[0];
                var value = variable[1];

                if (context.Model.Variables.ContainsKey(name))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        context.Model.Variables.Remove(name);
                    }
                    else
                    {
                        context.Model.Variables[name] = value;
                    }
                }
                else
                {
                    context.Model.Variables.Add(name, value);
                }
            }
            return new EvaluatorResult(true, string.Empty);
        }
    }
}