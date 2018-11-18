using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    class HasMoreExpressionEvaluator : IEvaluator
    {
        public bool Evaluate(IInterpreterContext context, string expression)
        {
            context.SetResult(context.EvaluateLoop(expression).Iterator.HasMore.ToString());

            return true;
        }
    }
}
