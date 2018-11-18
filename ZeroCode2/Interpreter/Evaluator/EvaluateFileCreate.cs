using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    class EvaluateFileCreate : IEvaluator
    {
        public bool Evaluate(IInterpreterContext context, string expression)
        {
            var resolver = new FilepathResolver();
            string expr = resolver.ResolvePath(context, expression);

            var fileEmitter = context.Emitter as Emitter.FileEmitter;
            if (fileEmitter != null && fileEmitter.Exists(expr))
            {
                return false;
            }
            else
            {
                context.Emitter.Open(expr);
                return true;
            }
        }
    }
}
