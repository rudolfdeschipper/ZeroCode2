using System;
using System.Collections.Generic;
using System.Linq;
using ZeroCode2.Models;

namespace ZeroCode2.Interpreter
{
    public class InterpreterContext : IInterpreterContext
    {
        public Emitter.IEmitter Emitter { get; set; }
        public ModelCollector Model { get; set; }

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Stack<IteratorManager> LoopStack { get; set; }

        public InterpreterContext()
        {
            LoopStack = new Stack<IteratorManager>();
        }

        public void EmitResult(string Result)
        {
            if (!string.IsNullOrEmpty(Result))
            {
                Emitter.Emit(Result);
            }
        }

        public bool PropertyExists(string expression)
        {
            var locator = new Models.PropertyLocator(expression, Model, LoopStack);

            return locator.Locate();
        }

        public string EvaluateProperty(string expression)
        {
            var locator = new Models.PropertyLocator(expression, Model, LoopStack);

            if (locator.Locate() == true)
            {
                var mp = locator.LocatedProperty();

                if (expression.EndsWith("$"))
                {
                    return mp.Name;
                }
                return mp.GetText();
            }
            else
            {
                throw new ApplicationException(string.Format("Could not evaluate expression {0}", expression));
            }
        }

        public IteratorManager EvaluateLoop(string expression)
        {
            var iterator = LoopStack.FirstOrDefault(l => l.Path == expression);

            if (iterator == null)
            {
                var parentPath = expression.Split('.')[0];

                if (expression.Length > parentPath.Length)
                {
                    expression = expression.Substring(parentPath.Length + 1);
                    // recurse until you find something
                    return EvaluateLoop(expression);
                }
                else
                {
                    // assume it is this one
                    return LoopStack.Peek();
                }
            }
            else
            {
                iterator.Iterate();
            }

            return iterator;
        }

        public void EnterLoop(string expression)
        {
            var iterator = new IteratorManager(new Models.Iterator());

            logger.Trace("Enter loop: " + expression);

            iterator.Path = expression;

            var locator = new Models.PropertyLocator(expression, Model, LoopStack);

            if (locator.Locate() == false)
            {
                throw new ApplicationException(string.Format("Could not understand this loop expression {0}", expression));
            }

            iterator.Root = locator.LocatedProperty();

            LoopStack.Push(iterator);
        }

        public void ExitLoop(string expression)
        {
            if (LoopStack.Count == 0)
            {
                throw new Exception("Exit loop while not in a loop");
            }

            logger.Trace("Exit loop: " + expression);
            _ = LoopStack.Pop();
            //if (it.Path != expression)
            //{
            //    throw new Exception("Exit loop does not match the expression: " + it.Path);
            //}
        }

    }

}
