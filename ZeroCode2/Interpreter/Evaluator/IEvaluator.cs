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

    public enum EvaluationResultValues
    {
        True,
        False,
        Failed
    }

}
