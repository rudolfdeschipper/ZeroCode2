namespace ZeroCode2.Interpreter
{
    class InterpreterInstructionValue : InterpreterInstructionBase
    {
        public InterpreterInstructionValue(int line, int pos, string instruction, Evaluator.IEvaluator evaluator) : base(line, pos, instruction, evaluator)
        {
        }

        protected override InterpreterInstructionBase SetResult(Interpreter.Evaluator.EvaluatorResult result)
        {
            Result = result;
            return Next;
        }
    }
}
