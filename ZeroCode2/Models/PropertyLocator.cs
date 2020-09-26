using System.Collections.Generic;
using System.Linq;

namespace ZeroCode2.Models
{
    public class PropertyLocator
    {
        public string Path { get; set; }
        public string[] PathElements { get; set; }

        private int currentPosition = 0;

        private ModelCollector Collector { get; set; }
        public IModelObject CurrentRoot { get; set; }
        private Stack<Interpreter.IteratorManager> LoopStack { get; set; }

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public PropertyLocator(string path, ModelCollector collector, Stack<Interpreter.IteratorManager> loopStack)
        {
            Path = path;
            PathElements = path.Split('.');
            CurrentRoot = null;
            Collector = collector;
            LoopStack = loopStack;
        }

        public IModelObject LocatedProperty()
        {
            return CurrentRoot;
        }

        public bool Locate()
        {
            return LocateElement();
        }

        private bool LocateElement()
        {
            if (!LocateModel())
            {
                logger.Error("{0} could not be located in the model", Path);
                return false;
            }
            // we have a model
            return LocateElementInModel();
        }

        private bool LocateElementInModel()
        {
            if (currentPosition > Path.Length)
            {
                return true;
            }
            // traverse the remainer of the path
            string remainingPath = Path.Substring(currentPosition);
            PathElements = remainingPath.Split('.');
            currentPosition = 0;
            foreach (var item in PathElements)
            {
                if (item == "$" && currentPosition == 0)
                {
                    break;
                }

                CurrentRoot = CurrentRoot.AsComposite()?.Value.SingleOrDefault(mp => mp.Name == item && !(mp.Modified && mp.Modifier == "-"));

                if (CurrentRoot == null)
                {
                    logger.Trace("{0} could not be located in the model", Path);
                    break;
                }
                currentPosition++;
            }
            return CurrentRoot != null;
        }

        private bool LocateModel()
        {
            CurrentRoot = null;
            currentPosition = 0;

            int nextPos = FindPrimaryModelElement();
            if (nextPos == -1)
            {
                return false;
            }
            currentPosition = nextPos;
            return CurrentRoot != null;
        }

        private int FindPrimaryModelElement()
        {
            if (Path.StartsWith("@"))
            {
                return FindFromSingleModel();
            }
            if (Path.StartsWith("#"))
            {
                return FindFromParameterModel();
            }
            // from here we are using the loop stack so first check if it has elements
            if (LoopStack.Count == 0)
            {
                logger.Error("{0} was evaluated as on Loop stack - but stack is empty", Path);
                return -1;
            }
            // single element, so this refers to the innermost loop
            if ((Path.Contains(".") == false))
            {
                return GetFromTopLoopStack();
            }
            if (Path.StartsWith("$"))
            {
                return GetFromTopLoopStack() + 2;
            }
            if (Path.StartsWith("Loop"))
            {
                return GetFromLoopX();
            }
            // refer to any other loop on the stack - go look for it
            var loopElement = LoopStack.FirstOrDefault(l => Path.StartsWith(l.LoopID));
            if (loopElement != null)
            {
                CurrentRoot = SetModelFromIterator(loopElement);
                logger.Trace("{0} was evaluated as on loop stack - CurrentRoot was {1}", Path, CurrentRoot == null ? "not found" : "found");
                return loopElement.LoopID.Length + 1;
            }
            // could not find anything - try this
            return GetFromTopLoopStack();
        }

        private int GetFromLoopX()
        {
            var loopIndex = int.Parse(PathElements[0].Substring(4));
            if (LoopStack.Count > loopIndex)
            {
                var mp = LoopStack.ElementAt(loopIndex);
                CurrentRoot = SetModelFromIterator(mp);
            }
            logger.Trace("{0} was evaluated as LoopX - CurrentRoot was {1}", Path, CurrentRoot == null ? "not found" : "found");
            return PathElements[0].Length + 1;
        }

        private int GetFromTopLoopStack()
        {
            CurrentRoot = SetModelFromIterator(LoopStack.Peek());
            logger.Trace("{0} was evaluated as top loop - CurrentRoot was {1}", Path, CurrentRoot == null ? "not found" : "found");
            return 0;
        }

        private int FindFromParameterModel()
        {
            var models = Collector.ParameterModels.SingleOrDefault(s => s.Name == PathElements[0]);
            if (models != null)
            {
                CurrentRoot = models;
            }
            logger.Trace("{0} was evaluated as Parameter - CurrentRoot was {1}", Path, CurrentRoot == null ? "not found" : "found");
            return PathElements[0].Length + 1;
        }

        private int FindFromSingleModel()
        {
            var models = Collector.SingleModels.SingleOrDefault(s => s.Name == PathElements[0]);
            if (models != null)
            {
                CurrentRoot = models;
            }
            logger.Trace("{0} was evaluated as Single model - CurrentRoot was {1}", Path, CurrentRoot == null ? "not found" : "found");
            return PathElements[0].Length + 1;
        }

        private IModelObject SetModelFromIterator(Interpreter.IteratorManager it)
        {
            if (it.CurrentModel == null)
            {
                it.Iterate();
            }
            // it may still be null but that is ok (empty loop)
            return it.CurrentModel;
        }
    }
}
