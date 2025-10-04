using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeroCode2.Interpreter
{
    public class InterpreterProgram
    {
        // Logging
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Stack<(InterpreterInstructionBranch, bool)> loopStack = new Stack<(InterpreterInstructionBranch, bool)>();
        private readonly Stack<InterpreterInstructionBase> ifElseStack = new Stack<InterpreterInstructionBase>();
        private InterpreterInstructionBranch LastFileCreateInstruction = null;

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
            if (Instructions.Count() > 0)
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

            if (LastFileCreateInstruction != null)
            {
                logger.Error("Line {0} - cannnot have a FileCreate before ending a another FileCreate instruction.", line);
            }
            LastFileCreateInstruction = instruction;

            AddInstruction(instruction);

            DebugInstruction("FileCreate", instruction);
        }


        public void AddFileOverwrite(int line, int pos, string value)
        {
            var instruction = new Interpreter.InterpreterInstructionBranch(line, pos, value, new Interpreter.Evaluator.EvaluateFileOverwrite());

            if (LastFileCreateInstruction != null)
            {
                logger.Error("Line {0} - cannnot have a FileOverWrite before ending a FileCreate instruction.", line);
            }
            AddInstruction(instruction);

            DebugInstruction("FileOverwrite", instruction);
        }

        public void AddLoop(int line, int pos, string value)
        {
            var filter = string.Empty;
            if (value.Contains('|'))
            {
                // there is a filter - remove it from the value and store it separately
                filter = value.Split('|')[1].Trim();
                value = value.Split('|')[0].Trim();
            }
            var instruction1 = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.EnterLoopEvaluator());
            var instruction2 = new Interpreter.InterpreterInstructionBranch(line, pos, value, new Interpreter.Evaluator.LoopEvaluator());

            AddInstruction(instruction1);
            AddInstruction(instruction2);

            loopStack.Push((instruction2, !string.IsNullOrEmpty(filter)));

            if (!string.IsNullOrEmpty(filter))
            {
                // add an if
                AddIf(line, pos, filter);
                // TODO: handle the endif - need to remember to add it in the EndLoop
            }

            DebugInstruction("Loop", instruction2);
        }

        public void AddEndFile(int line, int pos, string value)
        {
            var instruction = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.EvaluateEndFile());

            if (LastFileCreateInstruction != null)
            {
                LastFileCreateInstruction.Alternative = instruction;
                LastFileCreateInstruction = null;
            }
            AddInstruction(instruction);

            DebugInstruction("EndFile", instruction);
        }

        public void AddEndLoop(int line, int pos, string value)
        {
            var instruction1 = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.NoOpEvaluator());
            var instruction2 = new Interpreter.InterpreterInstructionNoOp(line, pos, value, new Interpreter.Evaluator.ExitLoopEvaluator());

            var closestLoop = loopStack.Count > 0 ? loopStack.Pop() : (null, false);

            if (closestLoop.Item1 != null)
            {
                if (closestLoop.Item2 == true)
                {
                    // need to add an endif for the if that was added for the filter
                    AddEndif(line, pos, "%EndIf");
                }
                AddInstruction(instruction1);
                AddInstruction(instruction2);

                instruction1.Next = closestLoop.Item1;
                closestLoop.Item1.Alternative = instruction2;
                closestLoop.Item1.EndBranch = instruction2;

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

                prevIf.Alternative = instruction2;

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
                errors.Add(string.Format("Loop {0} starting at line {1} is not terminated by a %/Loop statement.", item.Item1.Instruction, item.Item1.Line));
            }
            foreach (var item in ifElseStack)
            {
                errors.Add(string.Format("If/Endif {0} starting at line {1} is not terminated by a %EndIf statement.", item.Instruction, item.Line));
            }

            return errors;
        }

        public void AddVar(int line, int pos, string value)
        {
            var evaluator = new Interpreter.Evaluator.EvaluateVariable();

            var instruction = new Interpreter.InterpreterInstructionNoOp(line, pos, value, evaluator);

            AddInstruction(instruction);

            DebugInstruction("Expression", instruction);
        }
    }
}
