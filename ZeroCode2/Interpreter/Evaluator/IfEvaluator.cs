using System;

namespace ZeroCode2.Interpreter.Evaluator
{
    public class IfEvaluator : IEvaluator
    {
        private readonly bool IsNegate;
        private readonly IEvaluator evaluator;
        private readonly string leftSide;
        private readonly string rightSide;
        private readonly bool checkForExistence = false;
        private readonly bool CompareEqual = true;
        private readonly bool NoRightSide = true;

        public IfEvaluator(string expression)
        {
            int ComparePosition = 0;

            IsNegate = expression[0] == '!';

            if (IsNegate == true)
            {
                expression = expression.Substring(1);
            }

            if (expression.Contains("="))
            {
                ComparePosition = expression.IndexOf("=");
                CompareEqual = true;
                NoRightSide = false;
            }
            if (expression.Contains("!="))
            {
                ComparePosition = expression.IndexOf("!=");
                CompareEqual = false;
                NoRightSide = false;
            }

            leftSide = ComparePosition > 0 ? expression.Remove(ComparePosition) : expression;
            rightSide = (ComparePosition > 0 ? expression.Substring(ComparePosition + (CompareEqual ? 1 : 2)) : "true").ToLower();

            // check for %If:SomeProperty?
            if (leftSide.EndsWith("?"))
            {
                leftSide = leftSide.Substring(0, leftSide.Length - 1); // strip off the "?" at the end
                checkForExistence = true;
            }
            evaluator = Interpreter.ExpressionBuilder.BuildExpressionEvaluator(leftSide);

        }

        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            string LeftValue;
            try
            {
                if (checkForExistence)
                {
                    bool res = context.PropertyExists(leftSide);

                    // just check existence, nothing else, or it does not exist
                    if (res == false || NoRightSide)
                    {
                        if (IsNegate)
                        {
                            res = !res;
                        }
                        return new EvaluatorResult(res, string.Empty);
                    }
                }
                // existence of property is assumed (it is either checked or no "?" was present
                if (evaluator is Evaluator.ExpressionEvaluator)
                {
                    LeftValue = context.EvaluateProperty(leftSide);
                }
                else
                {
                    EvaluatorResult evalRes = evaluator.Evaluate(context, leftSide);
                    if (evalRes.Result == EvaluationResultValues.True)
                    {
                        LeftValue = evalRes.Value;
                    }
                    else
                    {
                        LeftValue = "error";
                    }
                }
            }
            catch (Exception ex)
            {
                return new EvaluatorResult(ex);
            }

            LeftValue = LeftValue.ToLower();

            bool retVal;

            if (CompareEqual)
            {
                retVal = LeftValue == rightSide;
            }
            else
            {
                retVal = LeftValue != rightSide;
            }
            // leading "!" is valid for the complete if-clause
            if (IsNegate)
            {
                retVal = !retVal;
            }
            return new EvaluatorResult(retVal, string.Empty);
        }
    }
}
