﻿using System;
using System.Collections.Generic;
using System.Linq;

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
                var iterator = LoopStack.FirstOrDefault(l => l.Path == parts[0]);

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
            if (expression.StartsWith("@"))
            {
                expression = expression.Substring(1);
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
            }
            else
            {
                iterator.CurrentModel = iterator.Iterator.Iterate(iterator.Root);
            }

            return iterator;
        }

        public void EnterLoop(string expression)
        {
            var iterator = new IteratorManager();

            logger.Info("Enter loop: " + expression);

            iterator.Path = expression;
            iterator.Iterator = new Models.Iterator();

            // Need to figure this out here, preferably, to avoid lots of ifs when evaluating:
            // - what sort of loop are we trying to run here?
            var locator = new Models.PropertyLocator();

            // -- top level (%Loop:@Screen) - @ to start and no dots in the path - Iterate over the SingleModels
            if (expression[0] == '@')
            {
                iterator.Root = locator.Locate(expression, Model);
                // throw away "@"
                iterator.Path = iterator.Path.Substring(1);
            }
            else
            {
                // -- use of "Loopx" construct (%Loop:Loop1.Panel) - dots in the path, use of word "loop" in the path identifier
                if (expression.StartsWith("Loop"))
                {
                    int level = int.Parse(expression.Split('.')[0].Substring(4));
                    var parentIterator = LoopStack.Skip(level).FirstOrDefault();
                    var root = parentIterator.Root;
                    iterator.Root = locator.Locate(expression, root);
                }
                else
                {
                    // -- new loop with a path to a previous loop (%Loop:Screen.Panel) - dots in the path - locate that previous path and treat it as relative path
                    if (expression.Contains("."))
                    {
                        var parentPath = expression.Split('.')[0];
                        var parentIterator = LoopStack.FirstOrDefault(l => l.Path == parentPath);

                        expression = expression.Substring(parentPath.Length + 1);
                        // shorten the path
                        iterator.Path = expression;
                        iterator.Root = locator.Locate(expression, parentIterator.CurrentModel);
                    }
                    else
                    {
                        // -- relative to previous loop (%Loop:Panel) - no dots in the path - find that loop and the expression and iterate over its elements
                        var parentIterator = LoopStack.Peek();
                        iterator.Root = locator.Locate(expression, parentIterator.CurrentModel);
                    }
                }
            }

            LoopStack.Push(iterator);
        }

        public void ExitLoop(string expression)
        {
            if (LoopStack.Count == 0)
            {
                throw new Exception("Exit loop while not in a loop");
            }

            logger.Info("Exit loop: " + expression);

            var it = LoopStack.Pop();
            //if (it.Path != expression)
            //{
            //    throw new Exception("Exit loop does not match the expression: " + it.Path);
            //}
        }

        public bool EvaluteCondition(string expression)
        {
            logger.Info("Evaluation condition: " + expression);

            var IsNegate = expression[0] == '!';
            if (IsNegate == true)
            {
                expression = expression.Substring(1);
            }

            var leftSide =  expression.Contains("=") ? expression.Remove(expression.IndexOf("=")) : expression;
            var rightSide = (expression.Contains("=") ? expression.Substring(expression.IndexOf("=")+1) : "true").ToLower();

            var value = EvaluateProperty(leftSide).ToLower();
            var retVal = IsNegate == false ? value == rightSide : value != rightSide;

            return retVal;
        }
    }

    public class IteratorManager
    {
        public Models.Iterator Iterator { get; set; }
        public string Path { get; set; }
        public Models.ModelPair Root { get; set; }
        public Models.ModelPair CurrentModel { get; set; }
    }
}
