using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    public interface IEvaluator
    {
        bool Evaluate(IInterpreterContext context, string expression);
    }
}
