using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter
{
    public class InterpreterProgram
    {
        private List<Interpreter.InterpreterInstructionBase> _instructions;
        //public string InputFile { get; set; }

        // Logging
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public InterpreterProgram()
        {
            Instructions = new List<Interpreter.InterpreterInstructionBase>();
            //InputFile = inFile;
        }

        public InterpreterProgram(List<InterpreterInstructionBase> instructions)
        {
            Instructions = instructions;
            //InputFile = inFile;
        }

        public List<InterpreterInstructionBase> Instructions { get => _instructions; set => _instructions = value; }

        private void AddInstruction(Interpreter.InterpreterInstructionBase instruction)
        {
            if (Instructions.Count() > 0 )
            {
                var lastNonLinked = Instructions.LastOrDefault(i => i.Next == null);
                if (lastNonLinked != null)
                {
                    lastNonLinked.Next = instruction;
                }
            }
            Instructions.Add(instruction);
        }

        public void AddLiteral(int line, int pos, string value)
        {
            var instruction = new Interpreter.InterpreterInstructionValue(line, pos, value, new Interpreter.Evaluator.LiteralEvaluator());

            AddInstruction(instruction);

            DebugInstruction("Literal", instruction);

        }

        public void AddFileCreate(int line, int pos, string value)
        {
            var instruction = new Interpreter.InterpreterInstructionBranch(line, pos, value, new Interpreter.Evaluator.EvaluateFileCreate());

            AddInstruction(instruction);

            DebugInstruction("FileCreate", instruction);

        }


        public void AddFileOverwrite(int line, int pos, string value)
        {
            var instruction = new Interpreter.InterpreterInstructionBranch(line, pos, value, new Interpreter.Evaluator.EvaluateFileOverwrite());

            AddInstruction(instruction);

            DebugInstruction("FileOverwrite", instruction);

        }

        public void AddLoop(int line, int pos, string value)
        {
            var instruction1 = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.EnterLoopEvaluator());
            var instruction2 = new Interpreter.InterpreterInstructionBranch(line, pos, value, new Interpreter.Evaluator.LoopEvaluator());

            AddInstruction(instruction1);
            AddInstruction(instruction2);

            DebugInstruction("Loop", instruction2);

        }

        public void AddEndFile(int line, int pos, string value)
        {
            var instruction = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.EvaluateEndFile());

            AddInstruction(instruction);

            DebugInstruction("EndFile", instruction);

        }

        public void AddEndLoop(int line, int pos, string value)
        {
            var instruction1 = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.NoOpEvaluator());
            var instruction2 = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.ExitLoopEvaluator());

            AddInstruction(instruction1);
            AddInstruction(instruction2);

            var closestLoop = Instructions.LastOrDefault(i => i._evaluator.GetType() == typeof(Interpreter.Evaluator.LoopEvaluator) && i.GetType() == typeof(Interpreter.InterpreterInstructionBranch) && ((Interpreter.InterpreterInstructionBranch)i)?.Alternative == null) as Interpreter.InterpreterInstructionBranch;

            if (closestLoop != null)
            {
                instruction1.Next = closestLoop;
                closestLoop.Alternative = instruction2;
            }
            else
            {
                logger.Error("End loop without matching Loop on line {0} - {1} ({2})", line, pos, value);
            }


            DebugInstruction("EndLoop", instruction1);

        }

        public void AddExpression(int line, int pos, string value)
        {
            Evaluator.IEvaluator evalObject = ExpressionBuilder.BuildExpressionEvaluator(value);

            var instruction = new Interpreter.InterpreterInstructionValue(line, pos, value, evalObject);

            AddInstruction(instruction);

            DebugInstruction("Expression", instruction);
        }

        public void AddIf(int line, int pos, string value)
        {
            var evaluator = new Interpreter.Evaluator.IfEvaluator(value);

            var instruction = new Interpreter.InterpreterInstructionBranch(line, pos, value, evaluator);

            AddInstruction(instruction);

            DebugInstruction("If", instruction);

        }

        public void AddElse(int line, int pos, string value)
        {
            var instruction = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.NoOpEvaluator());
            var instruction2 = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.NoOpEvaluator());

            AddInstruction(instruction);
            AddInstruction(instruction2);

            var prevIf = Instructions.LastOrDefault(i => i._evaluator.GetType() == typeof(Interpreter.Evaluator.IfEvaluator) && i.GetType() == typeof(Interpreter.InterpreterInstructionBranch) && ((Interpreter.InterpreterInstructionBranch)i).Alternative == null);
            if (prevIf != null)
            {
                ((Interpreter.InterpreterInstructionBranch)prevIf).Alternative = instruction2;
            }
            else
            {
                logger.Error("Else without matching If on line {0} - {1} ({2})", line, pos, value);
            }
            // set to null so that the next instruction will link to this one and not to instruction2
            instruction.Next = null;

            DebugInstruction("Else", instruction);

        }


        public void AddEndif(int line, int pos, string value)
        {
            var instruction = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.NoOpEvaluator());

            // determine if there was a previous else - if so set its first instruction's next to here
            // if there was no else, set the alternative of the closest if to this instruction
            // so need to find the closest if that has an open alternative or the closest else that has no next yet
            var closestIfOrElse = Instructions.LastOrDefault(i =>
               (i._evaluator.GetType() == typeof(Interpreter.Evaluator.IfEvaluator) && i.GetType() == typeof(Interpreter.InterpreterInstructionBranch) && ((Interpreter.InterpreterInstructionBranch)i).Alternative == null)
               ||
               (i.GetType() == typeof(Interpreter.InterpreterInstructionNoOp) && i.Instruction == "%Else" && i.Next == null)
            );
            if (closestIfOrElse != null)
            {
                if (closestIfOrElse.GetType() == typeof(Interpreter.InterpreterInstructionBranch))
                {
                    // this was an if without else
                    (closestIfOrElse as Interpreter.InterpreterInstructionBranch).Alternative = instruction;
                }
                else
                {
                    // this was an else
                    closestIfOrElse.Next = instruction;
                }
            }
            else
            {
                logger.Error("EndIf without matching If or Else on line {0} - {1} ({2})", line, pos, value);
            }

            AddInstruction(instruction);

            DebugInstruction("EndIf", instruction);

        }


        private void DebugInstruction(string type, Interpreter.InterpreterInstructionBase instruction)
        {
            logger.Debug("{0} {1} - {2}: '{3}'", type, instruction.Line, instruction.Position, instruction.Instruction);
        }

    }
}
