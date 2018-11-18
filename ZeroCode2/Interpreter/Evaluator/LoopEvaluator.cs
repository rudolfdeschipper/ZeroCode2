using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    class LoopEvaluator : IEvaluator
    {
        public bool Evaluate(IInterpreterContext context, string expression)
        {
            bool result = false;

            var loop = context.EvaluateLoop(expression);

            result = loop.Iterator.HasMore || loop.CurrentModel != null;

            return result;
        }
    }
}
