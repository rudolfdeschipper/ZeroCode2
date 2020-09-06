using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeroCode2.Models
{

    public interface IModelObject : IComparable<IModelObject>
    {
        string Name { get; set; }

        bool Inherits { get; set; }
        string InheritsFrom { get; set; }
        IModelObject ParentObject { get; set; }
        bool IsResolved { get; set; }

        bool Modified { get; set; }
        string Modifier { get; set; }

        bool Is<P>();
        bool IsString();
        bool IsNumber();
        bool IsObject();
        bool IsBool();

        ModelObject<M> As<M>() where M : class;

        ModelObject<string> AsString();

        ModelObject<decimal> AsNumber();

        ModelObject<bool> AsBool();

        ModelObject<List<IModelObject>> AsComposite();

        string GetText();
        void Resolve();
    }

    public abstract class ModelObject<T> : IModelObject
    {
        public string Name { get; set; }
        public T Value { get; set; }

        public bool Inherits { get; set; }
        public string InheritsFrom { get; set; }
        public IModelObject ParentObject { get; set; }
        public bool IsResolved { get; set; } = false;

        public bool Modified { get; set; }
        public string Modifier { get; set; }

        public bool Is<P>()
        {
            return Value.GetType() == typeof(P);
        }

        public bool IsString()
        {
            return Is<string>();
        }

        public bool IsNumber()
        {
            return Is<double>();
        }

        public bool IsObject()
        {
            return Is<List<IModelObject>>();
        }

        public bool IsBool()
        {
            return Is<bool>();
        }

        public ModelObject<M> As<M>() where M : class
        {
            return this as ModelObject<M>;
        }

        public ModelObject<string> AsString()
        {
            return As<string>();
        }

        public ModelObject<decimal> AsNumber()
        {
            return this as ModelObject<decimal>;
        }

        public ModelObject<bool> AsBool()
        {
            return this as ModelObject<bool>;
        }

        public ModelObject<List<IModelObject>> AsComposite()
        {
            return As<List<IModelObject>>();
        }

        /// <summary>
        /// If the properties of this object are not yet resolved, resolve them, otherwise return
        /// </summary>
        public void Resolve()
        {
            if (!IsResolved) // not strictly needed as this check is also done in the PropertyResolved
            {
                PropertyResolver propResolver = new PropertyResolver();
                propResolver.PopulateProperties(this);
            }
        }

        public abstract string GetText();

        public int CompareTo(IModelObject other)
        {
            return this.ToString().CompareTo(other.ToString());
        }
    }

    public class ModelStringObject : ModelObject<string>
    {
        public override string GetText()
        {
            return Value;
        }
    }

    public class ModelBoolObject : ModelObject<bool>
    {
        public override string GetText()
        {
            return Value ? "true" : "false";
        }

    }

    public class ModelNumberObject : ModelObject<decimal>
    {

        public override string GetText()
        {
            return Value.ToString();
        }
    }

    public class ModelCompositeObject : ModelObject<List<IModelObject>>
    {
        public override string GetText()
        {
            var r = Value.Select(m => m.Name + " = " + m.GetText());
            string s = "{ ";

            return r.Aggregate(s, (f, run) => s += run + " ") + "}";
        }
    }

    public class ParameterModel : ModelCompositeObject
    {
        public ParameterModel(string name)
        {
            Name = name;
            Value = new List<IModelObject>();
        }

        public ParameterModel(string name, IEnumerable<IModelObject> properties)
        {
            Name = name;
            Value = new List<IModelObject>();
            Value.AddRange(properties);
        }

    }

    public class SingleModel : ModelCompositeObject
    {
        public SingleModel(string name, IEnumerable<IModelObject> properties)
        {
            Name = name;
            Value = new List<IModelObject>();

            Value.AddRange(properties);
        }
    }


    /// <summary>
    /// Resolves the inheritance in the model. Errors are reported through the Errors collection
    /// </summary>
    public class InheritanceResolver
    {
        public List<string> Errors { get; set; } = new List<string>();

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public bool ResolveInheritance(IModelObject pair, ModelCollector modelList)
        {
            var ok = true;
            // only try to populate it if the ParentObject is still set to null
            if (pair.Inherits == true && pair.ParentObject == null)
            {
                var locator = new Models.PropertyLocator("@" + pair.InheritsFrom, modelList, null);

                if (locator.Locate())
                {
                    pair.ParentObject = locator.LocatedProperty();
                }

                ok = pair.ParentObject != null;
                if (!ok)
                {
                    logger.Error(string.Format("{0} inheritance to {1} could not be resolved", pair.Name, pair.InheritsFrom));
                    Errors.Add(string.Format("{0} inheritance to {1} could not be resolved", pair.Name, pair.InheritsFrom));
                }
            }
            if (ok)
            {
                // check next parents for cycles:
                ok &= CheckForCycle(pair);
            }
            if (pair.IsObject())
            {
                // recurse:
                foreach (var item in pair.AsComposite().Value)
                {
                    ok &= ResolveInheritance(item, modelList);
                }
            }
            return ok;
        }

        public bool ResolveInheritance(SingleModel model, ModelCollector modelList)
        {
            var ok = true;
            // only try to populate it if the ParentObject is still set to null
            if (model.Inherits == true && model.ParentObject == null)
            {
                var locator = new Models.PropertyLocator("@" + model.InheritsFrom, modelList, null);

                if (locator.Locate())
                {
                    model.ParentObject = locator.LocatedProperty();
                }

                // return if resolved
                ok = model.ParentObject != null;
                if (!ok)
                {
                    logger.Error(string.Format("{0} inheritance to {1} could not be resolved", model.Name, model.InheritsFrom));
                    Errors.Add(string.Format("{0} inheritance to {1} could not be resolved", model.Name, model.InheritsFrom));
                }
            }
            if (ok)
            {
                // check next parents for cycle:
                ok &= CheckForCycle(model);
            }
            // dive into properties too
            foreach (var item in model.Value)
            {
                ok &= ResolveInheritance(item, modelList);
            }
            // no issue
            return ok;
        }

        private bool CheckForCycle(IModelObject model)
        {
            var parent = model.ParentObject;
            Stack<IModelObject> stack = new Stack<IModelObject>();
            while (parent != null)
            {
                var cycle = stack.FirstOrDefault(s => s == parent);
                if (cycle != null)
                {
                    logger.Error(string.Format("{0} inheritance defines a cyclic relation.", cycle.Name));
                    Errors.Add(string.Format("{0} inheritance defines a cyclic relation.", cycle.Name));
                    return false;
                }
                stack.Push(parent);

                parent = parent.ParentObject;
            }
            return true;
        }
    }


    /// <summary>
    /// Resolves the properties that are inherited.
    /// Resulting property list can be accessed through AllProperties
    /// Note that the resolution follows the inheritance patch, but does not resolve any inheritance of sub-objects
    /// PersonVM inheriting from Person will resolve to Person, 
    /// but a property within PersonVM that itself inherits from an object will not be resolved unless the PropertyResolver is called on the list
    /// containing that property
    /// 
    /// As the resolution is done as part of an Iterator, this works fine because each iterator will first resolve all the properties in the list it
    /// will iterate through
    /// </summary>
    public class PropertyResolver
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void PopulateProperties(IModelObject pair)
        {
            // already resolved
            if (pair.IsResolved)
            {
                return;
            }
            // not an object
            if (!pair.IsObject())
            {
                return;
            }

            // resolve properties first:
            foreach (var item in pair.AsComposite().Value)
            {
                PopulateProperties(item);
            }

            // no inheritance
            if (!pair.Inherits)
            {
                return;
            }

           // invalid inheritance chain
            if (pair.ParentObject == null)
            {
                logger.Error("{0} inherits from {1} but was not resolved", pair.Name, pair.InheritsFrom);
                
                return;
            }
            if (pair.ParentObject.IsObject())
            {
                List<IModelObject> newProps = new List<IModelObject>();
                List<IModelObject> addedProps = new List<IModelObject>();

                // only copy the "+" from the current list:
                addedProps.AddRange(pair.AsComposite().Value.Where( mp => mp.Modifier == "+"));
 
                // we need to resolve these too, so we do that here:
                foreach (var item in addedProps)
                {
                    PopulateProperties(item);
                }

                // then, recurse into the children of the ParentObject, to ensure all is resolved before we copy from it
                PopulateProperties(pair.ParentObject);

                foreach (var item in pair.ParentObject.AsComposite().Value)
                {
                    PopulateProperties(item);
                }

                // Now we can add all properties from the inheritance object
                newProps.AddRange(pair.ParentObject.AsComposite().Value.Where(mp => mp.Modifier != "-"));

                // and we add any changed properties back in too:
                foreach (var item in pair.AsComposite().Value.Where(p => !p.Modified || p.Modifier == "-"))
                {
                    // find if it is already there
                    int index = newProps.FindIndex(mp => mp.Name == item.Name);

                    // if the property already exists, we merge the two elements
                    if (index != -1)
                    {
                        // only if not modified, if it is modified either it is added and we don't 
                        // have to try to merge (nothing to do)
                        // or it is deleted, and then it shall be removed, so no merging needed either
                        if (!item.Modified)
                        {
                            var itemToUpdate = newProps[index];

                            MergeProperties(itemToUpdate, item);

                        }
                        if (item.Modifier == "-")
                        {
                            newProps.RemoveAt(index);
                        }
                    }
                    else
                    {
                        // item does not exist
                        if (!item.Modified)
                        {
                            // actually this means it was in fact a "+" but we tolerate this
                            newProps.Add(item);
                        }
                    }
                }

                // put added props at the end
                newProps.AddRange(addedProps);

                // set the new set as the value:
                pair.AsComposite().Value = newProps;
                pair.IsResolved = true;
            }
            else
            {
                logger.Error("{0} inherits from {1} but this is not an object", pair.Name, pair.InheritsFrom);
            }
        }

        private void MergeProperties(IModelObject itemToUpdate, IModelObject item)
        {
            // for each item, run through its properties
            // if a property exists in itemToUpdate, overwrite it
            // if it does not exist, add it
            if (itemToUpdate.IsObject() && item.IsObject())
            {
                foreach (var prop in item.AsComposite().Value)
                {
                    var propToUpdate = itemToUpdate.AsComposite().Value.FindIndex(p => p.Name == prop.Name);
                    if (propToUpdate != -1)
                    {
                        itemToUpdate.AsComposite().Value[propToUpdate] = prop;
                    }
                    else
                    {
                        itemToUpdate.AsComposite().Value.Add(prop);
                    }
                }
            }
        }
    }

    /// <summary>
    /// This class allows to iterate through the various collections. HasMore indicates if more items are present.
    /// If not, the Iterate method returns null. It also returns null if the iterations fails for another reason.
    /// TODO: include inherited properties and exclude the ones that are "-"-ed.
    /// </summary>
    public class Iterator
    {
        private int currentItem = -1;

        public bool HasMore { get; private set; }

        public IModelObject Iterate(IModelObject mp)
        {
            var obj = mp.AsComposite();

            if (obj == null)
            {
                return null;
            }

            HasMore = obj.Value.Count > (currentItem + 1);

            if (!HasMore)
            {
                return null;
            }
            currentItem++;

            HasMore = obj.Value.Count > (currentItem + 1);

            if (obj.Value[currentItem] != null)
            {
                obj.Value[currentItem].Resolve();
            }

            return obj.Value[currentItem];
        }

        public int Index => currentItem;

    }

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
