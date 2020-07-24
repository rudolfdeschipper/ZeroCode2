using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Evaluator
{
    public class ExpressionEvaluator : IEvaluator
    {
        /// <summary>
        /// example:
        /// xyz[abc][def[ghi]]
        /// 1) xyz[abc][def[ghi]]
        /// 2) abc][def[ghi]]
        /// 3) def[ghi]]
        /// 4) ghi]] --> refghi
        /// </summary>
        /// <param name="subExpr"></param>
        /// <returns></returns>
        private string EvaluateReferences(IInterpreterContext context, string subExpr)
        {
            var start = subExpr.IndexOf('[');
            var end = subExpr.IndexOf(']');

            if (subExpr.Substring(start + 1, end - start - 1).Contains("["))
            {
                // we have nested references, resolve this first
                var refres = EvaluateReferences(context, subExpr.Substring(start + 1));

                // add the evaluate sub-expression to the current prefix
                subExpr = subExpr.Substring(0, start + 1) + refres;
                // recalc the end position in the sub-expression of the end of the sub-expression
                end = subExpr.IndexOf(']');
            }
            // evaluate the sub-expression
            var result = Evaluate(context, subExpr.Substring(start + 1, end - start - 1));
            // insert the result in the current sub-expression
            subExpr = subExpr.Substring(0, start) + result.Value + subExpr.Substring(end + 1);

            return subExpr;
        }

        public EvaluatorResult Evaluate(IInterpreterContext context, string expression)
        {
            // check for references:
            while(expression.Contains("["))
            {
                // evaluate the sub-expression into the current expression
                expression = EvaluateReferences(context, expression);
            }
            string result = context.EvaluateProperty(expression);

            // check for references in the result:
            while (result.Contains("["))
            {
                // evaluate the sub-expression into the current expression
                result = EvaluateReferences(context, result);
            }

            return new EvaluatorResult(true, result);
        }
    }
}
