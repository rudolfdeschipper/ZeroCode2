using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter
{
    class ExpressionBuilder
    {
        public static Evaluator.IEvaluator BuildExpressionEvaluator(string expression)
        {
            Evaluator.IEvaluator evalObject;

            var props = expression.Split('.');

            switch (props.Last())
            {
                case "HasMore":
                    evalObject = new Interpreter.Evaluator.HasMoreExpressionEvaluator();
                    break;
                case "Index":
                    evalObject = new Interpreter.Evaluator.IndexExpressionEvaluator();
                    break;
                case "Ordinal":
                    evalObject = new Interpreter.Evaluator.OrdinalExpressionEvaluator();
                    break;
                case "DateStamp":
                    evalObject = new Interpreter.Evaluator.TimestampExpressionEvaluator();
                    break;
                default:
                    evalObject = new Interpreter.Evaluator.ExpressionEvaluator();
                    break;
            }
            return evalObject;
        }
    }
}
