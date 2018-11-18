using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter
{
    public class InterpreterContext : IInterpreterContext
    {
        public Emitter.IEmitter Emitter { get; set; }
        public ModelCollector Model { get; set; }

        private Stack<IteratorManager> loopStack { get; set; }

        public InterpreterContext()
        {
            loopStack = new Stack<IteratorManager>();
        }

        public void SetResult(string result)
        {
            Emitter.Emit(result);
        }

        public string EvaluateProperty(string expression)
        {
            var locator = new Models.PropertyLocator();

            var mp = locator.Locate(expression, Model);

            if (mp == null)
            {
                // try the loops
                // locate loop identifier
                var parts = expression.Split('.');
                var iterator = loopStack.FirstOrDefault(l => l.Path == parts[0]);

                if (iterator != null)
                {
                    // use the rest of the expression as path into the loop element
                    expression = expression.Remove(0, parts[0].Length + 1);
                    mp = locator.Locate(expression, iterator.CurrentModel);
                }
                else
                {
                    throw new ApplicationException(string.Format("Could not find loop identifier {0}", parts[0]));
                }
            }

            if (mp != null)
            {
                return mp.Value.GetText();
            }
            else
            {
                throw new ApplicationException(string.Format("Could not evaluate expression {0}", expression));
            }
        }

        public IteratorManager EvaluateLoop(string expression)
        {
            var iterator = loopStack.FirstOrDefault(l => l.Path == expression);

            iterator.CurrentModel = iterator.Iterator.Iterate(Model.SingleModels, expression);

            return iterator;
        }

        public void EnterLoop(string expression)
        {
            var iterator = new IteratorManager();

            iterator.Path = expression;
            iterator.Iterator = new Models.Iterator();

            loopStack.Push(iterator);
        }

        public void ExitLoop(string expression)
        {
            if (loopStack.Count == 0)
            {
                throw new Exception("Exit loop while not in a loop");
            }

            var it = loopStack.Pop();
            //if (it.Path != expression)
            //{
            //    throw new Exception("Exit loop does not match the expression: " + it.Path);
            //}
        }
    }

    public class IteratorManager
    {
        public Models.Iterator Iterator { get; set; }
        public string Path { get; set; }
        public Models.ModelPair CurrentModel { get; set; }
    }
}
