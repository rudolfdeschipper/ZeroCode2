using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter
{
    public class InterpreterProgram
    {
        // Logging
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Stack<InterpreterInstructionBranch> loopStack = new Stack<InterpreterInstructionBranch>();
        private readonly Stack<InterpreterInstructionBase> ifElseStack = new Stack<InterpreterInstructionBase>();

        public InterpreterProgram()
        {
            Instructions = new List<Interpreter.InterpreterInstructionBase>();
        }

        public InterpreterProgram(List<InterpreterInstructionBase> instructions)
        {
            Instructions = instructions;
        }

        public List<InterpreterInstructionBase> Instructions { get; set; }

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

            loopStack.Push(instruction2);

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

            var closestLoop = loopStack.Count > 0 ? loopStack.Pop() : null;

            if (closestLoop != null)
            {
                AddInstruction(instruction1);
                AddInstruction(instruction2);

                instruction1.Next = closestLoop;
                closestLoop.Alternative = instruction2;
                closestLoop.EndBranch = instruction2;

                DebugInstruction("EndLoop", instruction1);
            }
            else
            {
                logger.Error("End loop without matching Loop on line {0} - {1} ({2})", line, pos, value);
            }
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

            ifElseStack.Push(instruction);

            AddInstruction(instruction);

            DebugInstruction("If", instruction);
        }

        public void AddElse(int line, int pos, string value)
        {
            var instruction = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.NoOpEvaluator());
            var instruction2 = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.NoOpEvaluator());

            var prevIf = ifElseStack.Count > 0 ? ifElseStack.Pop() as InterpreterInstructionBranch : null;
            if (prevIf != null)
            {
                AddInstruction(instruction);
                AddInstruction(instruction2);

                ((Interpreter.InterpreterInstructionBranch)prevIf).Alternative = instruction2;

                ifElseStack.Push(instruction);

                // set to null so that the next instruction will link to this one and not to instruction2
                instruction.Next = null;

                DebugInstruction("Else", instruction);
            }
            else
            {
                logger.Error("Else without matching If on line {0} - {1} ({2})", line, pos, value);
            }
        }


        public void AddEndif(int line, int pos, string value)
        {
            var instruction = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.NoOpEvaluator());

            // determine if there was a previous else - if so set its first instruction's next to here
            // if there was no else, set the alternative of the closest if to this instruction
            // so need to find the closest if that has an open alternative or the closest else that has no next yet

            var closestIfOrElse = ifElseStack.Count > 0 ? ifElseStack.Pop() : null;

            if (closestIfOrElse != null)
            {
                if (closestIfOrElse.GetType() == typeof(Interpreter.InterpreterInstructionBranch))
                {
                    // this was an if without else
                    (closestIfOrElse as Interpreter.InterpreterInstructionBranch).Alternative = instruction;
                    (closestIfOrElse as Interpreter.InterpreterInstructionBranch).EndBranch = instruction;
                }
                else
                {
                    // this was an else
                    closestIfOrElse.Next = instruction;
                }

                AddInstruction(instruction);

                DebugInstruction("EndIf", instruction);
            }
            else
            {
                logger.Error("EndIf without matching If or Else on line {0} - {1} ({2})", line, pos, value);
            }
        }

        public void AddLogInstruction(int line, int pos, string logType, string value)
        {
            var evaluator = new Interpreter.Evaluator.EvaluateLogging(logType);

            var instruction = new Interpreter.InterpreterInstructionNoOp(line, pos, value, evaluator);

            AddInstruction(instruction);

            DebugInstruction(logType, instruction);
        }

        private void DebugInstruction(string type, Interpreter.InterpreterInstructionBase instruction)
        {
            logger.Trace("{0} {1} - {2}: '{3}'", type, instruction.Line, instruction.Position, instruction.Instruction);
        }

        public List<string> Errors()
        {
            var errors = new List<string>();

            foreach (var item in loopStack)
            {
                errors.Add(string.Format("Loop {0} starting at line {1} is not terminated by a %/Loop statement.", item.Instruction, item.Line));
            }
            foreach (var item in ifElseStack)
            {
                errors.Add(string.Format("If/Endif {0} starting at line {1} is not terminated by a %EndIf statement.", item.Instruction, item.Line));
            }

            return errors;
        }

    }
}
