using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    class ExitLoopEvaluator : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            context.ExitLoop(expression);
            return new EvaluatorResult(true, string.Empty);
        }
    }
}
