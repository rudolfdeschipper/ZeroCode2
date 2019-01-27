using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeroCode2.Models
{

    public interface IModelObject
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

        public abstract string GetText();
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

        public bool ResolveInheritance(IModelObject pair, ModelCollector modelList)
        {
            var ok = true;
            if (pair.Inherits == true)
            {
                var locator = new Models.PropertyLocator("@" + pair.InheritsFrom, modelList, null);
                var mp = locator.Locate();

                if (mp != null)
                {
                    pair.ParentObject = mp;
                }

                ok &= pair.ParentObject != null;
                if (pair.ParentObject == null)
                {
                    Errors.Add(string.Format("{0} inheritance {1} could not be resolved", pair.Name, pair.InheritsFrom));
                }
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
            if (model.Inherits == true)
            {
                var locator = new Models.PropertyLocator("@" + model.InheritsFrom, modelList, null);
                var mp = locator.Locate();

                if (mp != null)
                {
                    model.ParentObject = mp;
                }

                // return if resolved
                ok = model.ParentObject != null;
                if (model.ParentObject == null)
                {
                    Errors.Add(string.Format("{0} inheritance {1} could not be resolved", model.Name, model.InheritsFrom));
                }
            }
            // dive into properties too
            foreach (var item in model.Value)
            {
                ok &= ResolveInheritance(item, modelList);
            }
            // no issue
            return ok;
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
        public void PopulateProperties(IModelObject pair)
        {
            List<IModelObject> newProps = new List<IModelObject>();

            // no inheritance
            if (!pair.Inherits)
            {
                return;
            }

             // already resolved
            if (pair.IsResolved)
            {
                return;
            }
           // no inheritance
            if (pair.Inherits && pair.ParentObject == null)
            {
                return;
            }
            // not an object
            if (!pair.IsObject())
            {
                return;
            }
            if (pair.ParentObject.IsObject())
            {
                // only copy the "+" from the current list:
                newProps.AddRange(pair.AsComposite().Value.Where( mp => mp.Modifier == "+"));
 
                // we need to resolve these too, so we do that here:
                foreach (var item in newProps)
                {
                    PopulateProperties(item);
                }

                // then, recurse into the children of the ParentObject, to ensure all is resolved before we copy from it
                foreach (var item in pair.ParentObject.AsComposite().Value)
                {
                    PopulateProperties(item);
                }

                // Now we can add all properties from the inheritance object
                newProps.AddRange(pair.ParentObject.AsComposite().Value.Where(mp => mp.Modifier != "-"));

                // and we add any changed properties back in too:
                foreach (var item in pair.AsComposite().Value.Where(p => !p.Modified || p.Modifier == "-"))
                {
                    // remove it if it is already there
                    newProps.RemoveAll(mp => mp.Name == item.Name);

                    // and add its replacement if it is a replacement (deletions we don't add back):
                    if (!item.Modified)
                    {
                        newProps.Add(item);
                    }
                }

                // set the new set as the value:
                pair.AsComposite().Value = newProps;
                pair.IsResolved = true;
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

            if (currentItem == -1 && !mp.IsResolved)
            {
                PropertyResolver propResolver = new PropertyResolver();
                propResolver.PopulateProperties(mp);
            }
            HasMore = obj.Value.Count > (currentItem + 1);

            if (!HasMore)
            {
                return null;
            }
            currentItem++;

            HasMore = obj.Value.Count > (currentItem + 1);

            return obj.Value[currentItem];
        }

        public int Index => currentItem;

    }

    public class PropertyLocator
    {
        public string[] pathElements { get; set; }

        private int currentPosition = 0;

        private ModelCollector Collector { get; set; }
        public IModelObject CurrentRoot { get; set; }
        private Stack<Interpreter.IteratorManager> LoopStack { get; set; }

        public PropertyLocator(string path, ModelCollector collector, Stack<Interpreter.IteratorManager> loopStack)
        {
            pathElements = path.Split('.');
            CurrentRoot = null;
            Collector = collector;
            LoopStack = loopStack;
        }

        public IModelObject Locate()
        {
            // 1. Check of CurrentRoot is set
            if (!DetermineRoot())
            {
                return null;
            }

            if (!CurrentRoot.IsResolved)
            {
                PropertyResolver propResolver = new PropertyResolver();
                propResolver.PopulateProperties(CurrentRoot);
            }

            // 2. Now we have a root, check if there is still a path to run
            if (currentPosition < pathElements.Length - 1)
            {
                // yes, so recurse
                currentPosition++;
                return Locate();
            }
            else
            {
                // we're at the end of the path
                // we're looking for the Name element, so return the current root
                if (pathElements[currentPosition] == "$")
                {
                    return CurrentRoot;
                }
                // if it is an object, find an element with the name we are looking for, or null
                if (CurrentRoot.Name != pathElements[currentPosition] && CurrentRoot.IsObject())
                {
                    var mp = CurrentRoot.AsComposite().Value.FirstOrDefault(m => m.Name == pathElements[currentPosition] && !(m.Modified && m.Modifier == "-"));
                    return mp;
                }
                else
                {
                    // return the root itself if it is the one we need, otherwise null
                    return CurrentRoot.Name == pathElements[currentPosition] ? CurrentRoot : null;
                }
            }
        }

        private bool DetermineRoot()
        {
            var oldRoot = CurrentRoot; // we will need it
            // set to Null to ensure we will fail in case we could not set a root.
            CurrentRoot = null;
            // Strategy:
            // work with the currentPos to work through the path. each iteration will change the current root
            // 1. Check if path starts with a section, in that case it is a full path
            // so look for the section and locate the second element in the path and set this as root
            if (currentPosition == 0)
            {
                if (pathElements[currentPosition].StartsWith("@"))
                {
                    var models = Collector.SingleModels.SingleOrDefault(s => s.Name == pathElements[currentPosition]);
                    if (models != null)
                    {
                        CurrentRoot = models;
                    }
                    return CurrentRoot != null;
                }
                if (pathElements[currentPosition].StartsWith("#"))
                {
                    var models = Collector.ParameterModels.SingleOrDefault(s => s.Name == pathElements[currentPosition]);
                    if (models != null)
                    {
                        CurrentRoot = models;
                    }
                    return CurrentRoot != null;
                }
                // 2. the Path has length of one: take top iterator as root
                // 3. Not a direct path, it is a relative one, so determine the iterator from the loopstack
                // 4. First look for a LoopX
                // 5. Then for an iterator identifier
                // 6. Last, take the top element
                if (LoopStack != null)
                {
                    // 4. Check for "Loopx" as path identifier, if so select the correct iterator as root
                    if (pathElements[currentPosition].StartsWith("Loop"))
                    {
                        var loopIndex = int.Parse(pathElements[currentPosition].Substring(4));
                        if (LoopStack.Count > loopIndex)
                        {
                            var mp = LoopStack.ElementAt(loopIndex);
                            if (mp.CurrentModel == null)
                            {
                                mp.Iterate();
                            }
                            CurrentRoot = mp.CurrentModel;
                        }
                        return CurrentRoot != null;
                    }
                    // The Path has length > one, the path may contain a reference to an iterator
                    // 5. Check if the first path identifier exists as an iterator - if so, get the root from there
                    if (LoopStack.Count > 0)
                    {
                        var it = LoopStack.SingleOrDefault(m => m.Root?.Name.Substring(1) == pathElements[currentPosition]);

                        // 6. No iterator was found, assume it is the top one, select that as root
                        if (it == null)
                        {
                            it = LoopStack.Peek();
                        }
                        if (it != null)
                        {
                            if (it.CurrentModel == null)
                            {
                                it.Iterate();

                                CurrentRoot = it.CurrentModel;
                            }

                            // we're looking for the Name element, so return the current root
                            if (currentPosition == pathElements.Length- 1 && pathElements.Last().EndsWith("$"))
                            {
                                return CurrentRoot != null;
                            }
                            // step one deeper in the model, as the iterated element is not part of the path

                            CurrentRoot = it.CurrentModel?.AsComposite()?.Value.SingleOrDefault(mp => mp.Name == pathElements[currentPosition] && !(mp.Modified && mp.Modifier == "-"));

                            if (CurrentRoot == null)
                            {
                                CurrentRoot = it.CurrentModel;
                            }
                        }
                    }
                    return CurrentRoot != null;
                }
            }
            else
            {
                // we are at second or higher element, go on looking
                if (CurrentRoot == null || currentPosition > 1) // once we are past the first one or two, we need to walk the object graph
                {
                    // 8. dive into the oldRoot
                    if (oldRoot != null && oldRoot.IsObject())
                    {
                        if (!oldRoot.IsResolved)
                        {
                            PropertyResolver propResolver = new PropertyResolver();
                            propResolver.PopulateProperties(oldRoot);
                        }

                        // we're looking for the Name element, so return the current root
                        if (currentPosition == pathElements.Length - 1 && pathElements.Last().EndsWith("$"))
                        {
                            CurrentRoot = oldRoot;
                            return CurrentRoot != null;
                        }

                        CurrentRoot = oldRoot.AsComposite().Value.SingleOrDefault(mp => mp.Name == pathElements[currentPosition] && !(mp.Modified && mp.Modifier == "-"));
                    }
                    else
                    {
                        CurrentRoot = oldRoot;
                    }
                }
            }
            // return true if the root was set. in case anything fails, return false
            return CurrentRoot != null;
        }

    }
}
