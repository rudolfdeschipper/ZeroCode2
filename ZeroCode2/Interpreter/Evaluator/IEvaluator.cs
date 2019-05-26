﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    public interface IEvaluator
    {
        EvaluatorResult Evaluate(IInterpreterContext context, string expression);
    }

    public enum EvaluationResultValues
    {
        True,
        False,
        Failed
    }

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
