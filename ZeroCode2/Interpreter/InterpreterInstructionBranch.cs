using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter
{
    class InterpreterInstructionBranch : InterpreterInstructionBase
    {
        public InterpreterInstructionBase Alternative { get; set; }

        public InterpreterInstructionBranch(int line, int pos, string instruction, Evaluator.IEvaluator evaluator) : base(line, pos, instruction, evaluator)
        {
        }

        protected override InterpreterInstructionBase SetResult(bool result)
        {
            return result ? Next : Alternative;
        }
    }
}
