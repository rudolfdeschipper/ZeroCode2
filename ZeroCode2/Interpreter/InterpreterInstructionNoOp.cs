using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter
{
    class InterpreterInstructionNoOp : InterpreterInstructionBase
    {
        public InterpreterInstructionNoOp(int line, int pos, string instruction, Evaluator.IEvaluator evaluator) : base(line, pos, instruction, evaluator)
        {
            //evaluator = new Evaluator.NoOpEvaluator();
        }

        protected override InterpreterInstructionBase SetResult(bool result)
        {
            return Next;
        }
    }
}
