using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter
{
    public class InterpreterInstructionEnterLoop : InterpreterInstructionBase
    {
        public InterpreterInstructionEnterLoop(int line, int pos, string instruction, Evaluator.IEvaluator evaluator) : base(line, pos, instruction, evaluator)
        {
            evaluator = new Evaluator.EnterLoopEvaluator();
        }

        protected override InterpreterInstructionBase SetResult(bool result)
        {
            return Next;
        }
    }
}
