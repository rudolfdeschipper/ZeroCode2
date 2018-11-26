using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    class EvaluateFileCreate : IEvaluator
    {
        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            var resolver = new FilepathResolver();
            string expr = resolver.ResolvePath(context, expression);

            var fileEmitter = context.Emitter as Emitter.FileEmitter;
            if (fileEmitter != null && fileEmitter.Exists(expr))
            {
                return new EvaluatorResult(false, string.Empty);
            }
            else
            {
                context.Emitter.Open(expr);
                return new EvaluatorResult(true, string.Empty);
            }
        }
    }
}
