using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    class HasMoreExpressionEvaluator : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            return new EvaluatorResult(true, context.EvaluateLoop(expression).Iterator.HasMore.ToString());
        }
    }
}
