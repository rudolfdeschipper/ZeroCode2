using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    class OrdinalExpressionEvaluator : IEvaluator
    {
        public bool Evaluate(IInterpreterContext context, string expression)
        {
            context.SetResult((context.EvaluateLoop(expression).Iterator.Index + 1).ToString());

            return true;
        }
    }
}
