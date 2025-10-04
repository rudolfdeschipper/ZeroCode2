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
                var result = mp.GetText();

                // replace backslash-quote by quote
                result = result.Replace("\\\"", "\"");
                // replace double backslash by a single backslash
                result = result.Replace("\\\\", "\\");
                // combination of the two properly replaces 3 backslashes and a quote by a backslash and a quote

                return result;
            }
            else
            {
                throw new ApplicationException(string.Format("Could not evaluate expression {0}", expression));
            }
        }

        public IteratorManager EvaluateLoop(string expression)
        {
            if(expression.EndsWith("?"))
            {
                expression = expression.Substring(0, expression.Length - 1);
            }

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
            IIterator iter;
            bool checkExists = false;
            if (expression.EndsWith("?"))
            {
                expression = expression.Substring(0, expression.Length - 1);
                if (!PropertyExists(expression))
                {
                    NonIterableIterator niiterator = new NonIterableIterator
                    {
                        HasMore = false // special case, do not iterate
                    };
                    iter = niiterator;
                    checkExists = true;
                }
                else
                {
                    iter = new Models.Iterator();
                    checkExists = false; // we know it exists, so we can iterate
                }
            }
            else
            {
                iter = new Models.Iterator();
            }
            var iterator = new IteratorManager(iter);

            logger.Trace("Enter loop: " + expression);

            iterator.Path = expression;
            if (!checkExists)
            {
                var locator = new Models.PropertyLocator(expression, Model, LoopStack);

                if (locator.Locate() == false)
                {
                    throw new ApplicationException(string.Format("Could not understand this loop expression {0}", expression));
                }

                iterator.Root = locator.LocatedProperty();

            }
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
