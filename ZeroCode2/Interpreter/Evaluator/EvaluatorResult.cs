using System;

namespace ZeroCode2.Interpreter.Evaluator
{
    public class EvaluatorResult
    {
        public EvaluationResultValues Result { get; set; }
        public string Value { get; set; }

        public EvaluatorResult(bool res, string val)
        {
            Result = res ? EvaluationResultValues.True : EvaluationResultValues.False;
            Value = val;
        }

        public EvaluatorResult(Exception ex)
        {
            Result = EvaluationResultValues.Failed;
            Value = ex.Message;
        }

        //public EvaluatorResult()
        //{

        //}
    }

}
