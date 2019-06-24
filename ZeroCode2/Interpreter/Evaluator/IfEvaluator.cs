using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    public class IfEvaluator : IEvaluator
    {
        private bool IsNegate;
        private IEvaluator evaluator;
        private string leftSide, rightSide;
        private readonly bool checkForExistence = false;

        public IfEvaluator(string expression)
        {
            IsNegate = expression[0] == '!';

            if (IsNegate == true)
            {
                expression = expression.Substring(1);
            }

            leftSide = expression.Contains("=") ? expression.Remove(expression.IndexOf("=")) : expression;
            rightSide = (expression.Contains("=") ? expression.Substring(expression.IndexOf("=") + 1) : "true").ToLower();

            // check for %If:SomeProperty?
            if (leftSide.EndsWith("?"))
            {
                // rightside must be interpreted as "exists"
                leftSide = leftSide.Substring(0, leftSide.Length - 1); // strip off the "?" at the end
                checkForExistence = true;
            }
            evaluator = Interpreter.ExpressionBuilder.BuildExpressionEvaluator(leftSide);

        }

        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            string value = "";

            try
            {
                if (checkForExistence)
                {
                    var res = context.PropertyExists(leftSide);
                    if (IsNegate)
                    {
                        res = !res;
                    }
                    return new EvaluatorResult(res, string.Empty);
                }

                if (evaluator is Evaluator.ExpressionEvaluator)
                {
                    value = context.EvaluateProperty(leftSide);
                }
                else
                {
                    var evalRes = evaluator.Evaluate(context, leftSide);
                    if (evalRes.Result == EvaluationResultValues.True)
                    {
                        value = evalRes.Value;
                    }
                    else
                    {
                        value = "error";
                    }
                }
            }
            catch (Exception ex)
            {
                return new EvaluatorResult(ex);
            }

            value = value.ToLower();
            var retVal = IsNegate == false ? value == rightSide : value != rightSide;

            return new EvaluatorResult(retVal, string.Empty);
        }
    }
}
