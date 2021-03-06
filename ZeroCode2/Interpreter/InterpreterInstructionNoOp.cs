﻿namespace ZeroCode2.Interpreter
{
    class InterpreterInstructionNoOp : InterpreterInstructionBase
    {
        public InterpreterInstructionNoOp(int line, int pos, string instruction, Evaluator.IEvaluator evaluator) : base(line, pos, instruction, evaluator)
        {
            //evaluator = new Evaluator.NoOpEvaluator();
        }

        protected override InterpreterInstructionBase SetResult(Interpreter.Evaluator.EvaluatorResult result)
        {
            Result = result;
            return Next;
        }
    }
}
