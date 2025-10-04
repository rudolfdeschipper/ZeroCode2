using System.Linq;

namespace ZeroCode2.Interpreter
{
    class ExpressionBuilder
    {
        public static Evaluator.IEvaluator BuildExpressionEvaluator(string expression)
        {
            string[] props = expression.Split('.');
            Evaluator.IEvaluator evalObject = (props.Last()) switch
            {
                "HasMore" => new Interpreter.Evaluator.HasMoreExpressionEvaluator(),
                "Index" => new Interpreter.Evaluator.IndexExpressionEvaluator(),
                "Ordinal" => new Interpreter.Evaluator.OrdinalExpressionEvaluator(),
                "DateStamp" => new Interpreter.Evaluator.TimestampExpressionEvaluator(),
                _ => new Interpreter.Evaluator.ExpressionEvaluator(),
            };
            return evalObject;
        }
    }
}
