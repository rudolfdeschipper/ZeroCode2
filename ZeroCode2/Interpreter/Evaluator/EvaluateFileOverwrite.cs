using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    class EvaluateFileOverwrite : IEvaluator
    {
        public bool Evaluate(IInterpreterContext context, string expression)
        {
            var resolver = new FilepathResolver();
            string expr = resolver.ResolvePath(context, expression);

            context.Emitter.Open(expr);
            return true;
        }
    }
}
