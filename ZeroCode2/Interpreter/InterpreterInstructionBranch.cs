namespace ZeroCode2.Interpreter
{
    class InterpreterInstructionBranch : InterpreterInstructionBase
    {
        public InterpreterInstructionBase Alternative { get; set; }
        public InterpreterInstructionBase EndBranch { get; set; }

        public InterpreterInstructionBranch(int line, int pos, string instruction, Evaluator.IEvaluator evaluator) : base(line, pos, instruction, evaluator)
        {
        }

        protected override InterpreterInstructionBase SetResult(Interpreter.Evaluator.EvaluatorResult result)
        {
            Result = result;
            // if false, jump to alternative
            if (result.Result == Interpreter.Evaluator.EvaluationResultValues.False)
            {
                return Alternative;
            }
            // if fail, jump to just after the endif / end loop
            if (result.Result == Interpreter.Evaluator.EvaluationResultValues.Failed)
            {
                if (Alternative.Instruction == "%EndIf" || Alternative.Instruction == "%/Loop")
                {
                    return Alternative;
                }
                else
                {
                    // there is an else, but neither clause must be executed now
                    return EndBranch;
                }
            }
            // if ok, jump to next
            return Next;
        }
    }
}
