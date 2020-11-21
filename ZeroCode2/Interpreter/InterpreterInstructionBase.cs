using System;

namespace ZeroCode2.Interpreter
{
    public abstract class InterpreterInstructionBase
    {
        public InterpreterInstructionBase Next { get; set; }
        public string Instruction { get; set; }
        public int Line { get; set; }
        public int Position { get; set; }
        public Evaluator.EvaluatorResult Result { get; set; }

        public InterpreterInstructionBase(int line, int pos, string instruction, Evaluator.IEvaluator evaluator)
        {
            Line = line;
            Position = pos;
            Instruction = instruction;
            Evaluator = evaluator;
        }

        public Evaluator.IEvaluator Evaluator { get; private set; }

        /// <summary>
        /// Sets the pointer to the Next instruction based on the result passed in
        /// </summary>
        protected abstract InterpreterInstructionBase SetResult(Evaluator.EvaluatorResult result);

        public InterpreterInstructionBase Execute(InterpreterContext context)
        {

            try
            {
                var result = Evaluator.Evaluate(context, Instruction);

                return SetResult(result);
            }
            catch (Exception ex)
            {
                var res = new Evaluator.EvaluatorResult(ex);
                return SetResult(res);
            }
        }
    }
}
