using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter
{
    public class InterpreterInstructionExitLoop : InterpreterInstructionBase
    {
        public InterpreterInstructionExitLoop(int line, int pos, string instruction, Evaluator.IEvaluator evaluator) : base(line, pos, instruction, evaluator)
        {
            evaluator = new Evaluator.ExitLoopEvaluator();
        }

        protected override InterpreterInstructionBase SetResult(bool result)
        {
            return Next;
        }
    }
}
