using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    class EvaluateLogging : IEvaluator
    {
        public string LogType { get; set; }
        // Logging
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public EvaluateLogging(string logType)
        {
            LogType = logType.Substring(logType.IndexOf('%')+1);
            LogType = LogType.Substring(0, LogType.Length - 1);
        }

        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            var resolver = new FilepathResolver();
            string expr = resolver.ResolvePath(context, expression);

            switch (LogType)
            {
                case "Log":
                    System.Console.Out.WriteLine(expr);
                    break;
                case "Info":
                    logger.Info(expr);
                    break;
                case "Debug":
                    logger.Debug(expr);
                    break;
                case "Error":
                    logger.Error(expr);
                    break;
                case "Trace":
                    logger.Trace(expr);
                    break;
                default:
                    break;
            }
            return new EvaluatorResult(true, string.Empty);
        }
    }
}
