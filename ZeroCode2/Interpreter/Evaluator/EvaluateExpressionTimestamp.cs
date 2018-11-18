using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    class TimestampExpressionEvaluator : IEvaluator
    {
        public bool Evaluate(IInterpreterContext context, string expression)
        {
            context.SetResult(DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss"));

            return true;
        }
    }
}
