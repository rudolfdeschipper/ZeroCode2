using System;
using System.Collections.Generic;

namespace ZeroCode2
{
    public class ModelExecutor
    {
        readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public List<string> Errors { get; set; } = new List<string>();
        public bool HasErrors { get; set; } = false;

        public void ExecuteProgram(ModelParser modelParser, TemplateParser templateParser, Interpreter.Emitter.IEmitter emitter)
        {
            var instructions = templateParser.Program.Instructions;
            var context = new Interpreter.InterpreterContext
            {
                Model = modelParser.ModelCollector,
                Emitter = emitter
            };

            var PC = instructions[0];
            Interpreter.InterpreterInstructionBase next = null;

            while (PC != null)
            {
                try
                {
                    logger.Trace("Executing {0} on Line {1} Pos {2}", PC.Instruction, PC.Line, PC.Position);

                    next = PC.Execute(context);
                    if (PC.Result.Result == Interpreter.Evaluator.EvaluationResultValues.Failed)
                    {
                        string error = string.Format("Error during execution: {0} Line {1}, Pos {2}: {3}", PC.Result.Value, PC.Line, PC.Position, PC.Instruction);

                        HasErrors = true;
                        Errors.Add(error);
                        logger.Error(error);
                    }
                    else
                    {
                        context.EmitResult(PC.Result.Value);
                    }

                }
                catch (Exception e)
                {
                    Errors.Add(string.Format("Error during execution: {0} Line {1}, Pos {2}: {3}", e.Message, PC.Line, PC.Position, PC.Instruction));
                    HasErrors = true;
                    //break;
                }

                PC = next;
            }

        }
    }


}
