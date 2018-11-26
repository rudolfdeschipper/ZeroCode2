using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    public interface IEvaluator
    {
        EvaluatorResult Evaluate(IInterpreterContext context, string expression);
    }

    public class EvaluatorResult
    {
        public bool Result { get; set; }
        public string Value { get; set; }

        public EvaluatorResult(bool res, string val)
        {
            Result = res;
            Value = val;
        }

        public EvaluatorResult()
        {

        }
    }

}
