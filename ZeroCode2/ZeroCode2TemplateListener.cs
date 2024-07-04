using Antlr4.Runtime.Misc;
using ZeroCode2.Grammars;

namespace ZeroCode2
{
    class ZeroCode2TemplateListener : ZeroCode2TemplateBaseListener
    {
        // Logging
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public Interpreter.InterpreterProgram Program { get; set; }

        public override void ExitLiteralCommand([NotNull] ZeroCode2Template.LiteralCommandContext context)
        {
            Program.AddLiteral(context.start.Line, context.start.StartIndex, context.GetText());

            base.ExitLiteralCommand(context);
        }

        public override void ExitFileCreateCommand([NotNull] ZeroCode2Template.FileCreateCommandContext context)
        {
            string instrVal = context.FILECREATE().GetText().Substring("%FileCreate:".Length);

            instrVal = RemoveLineEnd(instrVal);

            Program.AddFileCreate(context.start.Line, context.start.StartIndex, instrVal);

            base.ExitFileCreateCommand(context);
        }

        public override void ExitFileOverwriteCommand([NotNull] ZeroCode2Template.FileOverwriteCommandContext context)
        {
            string instrVal = context.FILEOVERWRITE().GetText().Substring("%FileOverwrite:".Length);

            instrVal = RemoveLineEnd(instrVal);

            Program.AddFileOverwrite(context.start.Line, context.start.StartIndex, instrVal);

            base.ExitFileOverwriteCommand(context);
        }

        public override void ExitLoopCommand([NotNull] ZeroCode2Template.LoopCommandContext context)
        {
            string loopArgument = context.GetText().Substring("%Loop:".Length);

            loopArgument = RemoveLineEnd(loopArgument).Trim();

            Program.AddLoop(context.start.Line, context.start.StartIndex, loopArgument);

            base.ExitLoopCommand(context);
        }

        public override void ExitEndFileCommand([NotNull] ZeroCode2Template.EndFileCommandContext context)
        {
            var instrVal = RemoveLineEnd(context.GetText());

            Program.AddEndFile(context.start.Line, context.start.StartIndex, instrVal);

            base.ExitEndFileCommand(context);
        }

        public override void ExitEndLoopCommand([NotNull] ZeroCode2Template.EndLoopCommandContext context)
        {
            var instrVal = RemoveLineEnd(context.GetText());

            Program.AddEndLoop(context.start.Line, context.start.StartIndex, instrVal);

            base.ExitEndLoopCommand(context);
        }

        public override void ExitIncludeCommand([NotNull] ZeroCode2Template.IncludeCommandContext context)
        {
            var instrVal = RemoveLineEnd(context.GetText());

            instrVal = instrVal.Substring("%Include:".Length); ;

            logger.Info("Including {0}", instrVal);

            var newParser = new TemplateParser(Program);

            newParser.ParseTemplateFile(instrVal);

            logger.Info("End Including {0}", instrVal);

            base.ExitIncludeCommand(context);
        }

        public override void ExitExprCommand([NotNull] ZeroCode2Template.ExprCommandContext context)
        {
            string instrVal = context.GetText().Substring("=<".Length);

            instrVal = instrVal.Replace(">", "");

            Program.AddExpression(context.start.Line, context.start.StartIndex, instrVal);

            base.ExitExprCommand(context);
        }

        public override void ExitIfCommand([NotNull] ZeroCode2Template.IfCommandContext context)
        {
            string instrVal = context.GetText().Substring("%If:".Length).Trim();

            instrVal = RemoveLineEnd(instrVal).Trim();

            Program.AddIf(context.start.Line, context.start.StartIndex, instrVal);

            base.ExitIfCommand(context);
        }

        public override void ExitElseCommand([NotNull] ZeroCode2Template.ElseCommandContext context)
        {
            var instrVal = RemoveLineEnd(context.GetText()).Trim();

            Program.AddElse(context.start.Line, context.start.StartIndex, instrVal);

            base.ExitElseCommand(context);
        }

        public override void ExitEndIfCommand([NotNull] ZeroCode2Template.EndIfCommandContext context)
        {
            var instrVal = RemoveLineEnd(context.GetText()).Trim();

            Program.AddEndif(context.start.Line, context.start.StartIndex, instrVal);

            base.ExitEndIfCommand(context);
        }

        public override void ExitLogCommand([NotNull] ZeroCode2Template.LogCommandContext context)
        {
            var instrVal = RemoveLineEnd(context.GetText()).Trim();

            var logType = instrVal.Substring(0, instrVal.IndexOf(':') + 1);
            var val = instrVal.Substring(instrVal.IndexOf(':') + 1);

            Program.AddLogInstruction(context.start.Line, context.start.StartIndex, logType, val);

            base.ExitLogCommand(context);
        }

        public override void ExitVarCommand([NotNull] ZeroCode2Template.VarCommandContext context)
        {
            string instrVal = context.GetText().Substring("=<".Length);

            instrVal = instrVal.Replace(">", "");

            Program.AddVar(context.start.Line, context.start.StartIndex, instrVal);

            base.ExitVarCommand(context);
        }
        private string RemoveLineEnd(string inString)
        {
            string outString;

            outString = inString.Replace("\n", "");
            outString = outString.Replace("\r", "");
            return outString;
        }
    }


}
