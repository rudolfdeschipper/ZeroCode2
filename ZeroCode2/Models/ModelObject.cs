using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Models
{

    public class ParameterModel
    {
        public ParameterModel(string name)
        {
            Name = name;
            Properties = new List<ModelPair>();
        }

        public ParameterModel(string name, IEnumerable<ModelPair> properties)
        {
            Name = name;
            Properties = new List<ModelPair>();
            Properties.AddRange(properties);
        }

        public string Section { get; set; }
        public string Name { get; set; }
        public List<ModelPair> Properties { get; set; }
    }

    public class SingleModel : ModelPair
    {
        public string Section { get; set; }

        public SingleModel(string name, IEnumerable<ModelPair> properties) : base(name)
        {
            Value = new ObjectObject();

            AsObject().Value.AddRange(properties);
        }

        public Type GetObjectType() => typeof(SingleModel);

        public string GetText() => Name;

    }

    public class ModelPair
    {
        public string Name { get; set; }
        public IObjectBase Value { get; set; }

        public bool Inherits { get; set; }
        public string InheritsFrom { get; set; }
        public SingleModel ParentObject { get; set; }
        public bool IsResolved { get; set; } = false;

        public bool Modified { get; set; }
        public string Modifier { get; set; }

        public ModelPair(string name, IObjectBase value)
        {
            Name = name;
            Value = value;
        }

        public ModelPair(string name)
        {
            Name = name;
        }

        public bool Is<T>() => Value.GetObjectType() == typeof(T);
        public bool IsString() => Is<string>();
        public bool IsNumber() => Is<double>();
        public bool IsObject() => Is<ObjectObject>();
        public bool IsBool() => Is<bool>();

        public T As<T>() => (T)Value;

        public string AsString() => As<string>();
        public double AsNumber() => As<double>();
        public bool AsBOol() => As<bool>();
        public ObjectObject AsObject() => As<ObjectObject>();


    }

    public interface IObjectBase
    {
        string GetText();

        Type GetObjectType();
    }

    public class ObjectBase<T>
    {
        public T Value { get; set; }
    }

    public class StringObject : ObjectBase<string>, IObjectBase
    {
        public Type GetObjectType() => typeof(string);

        public string GetText() => Value;

    }

    public class ReferenceObject : ObjectBase<string>, IObjectBase
    {
        public string GetText() => Value;
        public Type GetObjectType() => typeof(string);

    }

    public class NumberObject : ObjectBase<double>, IObjectBase
    {
        public string GetText() => Value.ToString();
        public Type GetObjectType() => typeof(double);

    }

    public class ObjectObject : ObjectBase<List<ModelPair>>, IObjectBase
    {
        public ObjectObject()
        {
            Value = new List<ModelPair>();
        }

        public string GetText()
        {
            var r = Value.Select(m => m.Name + " = " + m.Value.GetText());
            string s = "{ ";

            return r.Aggregate(s, (f, run) => s += run + " " ) + "}";
        }
        public Type GetObjectType() => typeof(ObjectObject);

    }

    public class BoolObject : ObjectBase<bool>, IObjectBase
    {
        public string GetText() => Value.ToString();

        public Type GetObjectType() => typeof(bool);

    }

    /// <summary>
    /// Resolves the inheritance in the model. Errors are reported through the Errors collection
    /// </summary>
    public class InheritanceResolver
    {
        public List<string> Errors { get; set; } = new List<string>();

        public bool ResolveInheritance(ModelPair pair, string section, List<Models.SingleModel> modelList)
        {
            var ok = true;
            if (pair.Inherits == true && pair.Value != null)
            {
                string inheritedObjectname;
                // qualify name
                if (!pair.InheritsFrom.Contains("."))
                {
                    inheritedObjectname = section + "." + pair.InheritsFrom;
                }
                else
                {
                    inheritedObjectname = pair.InheritsFrom;
                }
                // check if can be resolved
                foreach (var item in modelList)
                {
                    if (inheritedObjectname.Equals(item.Section + "." + item.Name))
                    {
                        pair.ParentObject = item;
                        break;
                    }
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
                foreach (var item in pair.AsObject().Value)
                {
                    ok &= ResolveInheritance(item, section, modelList);
                }
            }
            return ok;
        }

        public bool ResolveInheritance(SingleModel model, List<Models.SingleModel> modelList)
        {
            var ok = true;
            if (model.Inherits == true)
            {
                string inheritedObjectname;

                // qualify name
                if (!model.InheritsFrom.Contains("."))
                {
                    inheritedObjectname = model.Section + "." + model.InheritsFrom;
                }
                else
                {
                    inheritedObjectname = model.InheritsFrom;
                }
                // check if can be resolved
                foreach (var item in modelList)
                {
                    if (inheritedObjectname.Equals(item.Section + "." + item.Name) && model != item)
                    {
                        model.ParentObject = item;
                        break;
                    }
                }
                // return if resolved
                ok = model.ParentObject != null;
                if (model.ParentObject == null)
                {
                    Errors.Add(string.Format("{0} inheritance {1} could not be resolved", model.Name, model.InheritsFrom));
                }
            }
            // dive into properties too
            foreach (var item in model.AsObject().Value)
            {
                ok &= ResolveInheritance(item, model.Section, modelList);
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
        public List<ModelPair> AllProperties { get; private set; } = new List<ModelPair>();

        //public void PopulateProperties(SingleModel model)
        //{
        //    if (model.ParentObject != null)
        //    {
        //        // recurse into parent
        //        PopulateProperties(model.ParentObject);
        //    }

        //    foreach (var item in model.AsObject().Value.Where(p => p.Modifier == "-"))
        //    {
        //        AllProperties.RemoveAll(p => p.Name == item.Name);
        //    }

        //    AllProperties.AddRange(model.AsObject().Value.Where(p => model.ParentObject == null || (model.ParentObject != null && ( p.Modified && p.Modifier != "-"))));
        //}

        public void PopulateProperties(ModelPair pair)
        {
            if (pair.Inherits && pair.ParentObject == null)
            {
                // toDO
            }
            if (pair.ParentObject != null)
            {
                // recurse into parent
                PopulateProperties(pair.ParentObject);
            }
            foreach (var item in pair.AsObject().Value.Where(p => p.Modifier == "-"))
            {
                AllProperties.RemoveAll(p => p.Name == item.Name);
            }

            AllProperties.AddRange(pair.AsObject().Value.Where(p => pair.ParentObject == null || (pair.ParentObject != null && ( p.Modified && p.Modifier != "-"))));
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

        private List<Models.SingleModel> modelsforSection = new List<SingleModel>();

        public bool HasMore { get; private set; }

        public SingleModel Iterate(List<Models.SingleModel> modelList, string section)
        {
            // set up
            if (currentItem == -1)
            {
                modelsforSection.AddRange(modelList.Where(m => m.Section == section));
            }
            HasMore = modelsforSection.Count > (currentItem+1);

            if (!HasMore)
            {
                return null;
            }

            currentItem++;

            HasMore = modelsforSection.Count > (currentItem + 1);

            return modelsforSection[currentItem];
        }

        public ModelPair Iterate(SingleModel model)
        {
            if (currentItem == -1)
            {
                PropertyResolver propResolver = new PropertyResolver();
                propResolver.PopulateProperties(model);
                model.AsObject().Value = propResolver.AllProperties;
                model.IsResolved = true;
            }

            HasMore = model.AsObject().Value.Count > (currentItem + 1);
            if (!HasMore)
            {
                return null;
            }
            currentItem++;

            HasMore = model.AsObject().Value.Count > (currentItem + 1);

            return model.AsObject().Value[currentItem];
        }

        public ModelPair Iterate(ModelPair mp)
        {
            var obj = mp.Value as ObjectObject;

            if (obj == null)
            {
                return null;
            }

            if (currentItem == -1)
            {
                PropertyResolver propResolver = new PropertyResolver();
                propResolver.PopulateProperties(mp);
                obj.Value = propResolver.AllProperties;
                mp.IsResolved = true;
            }
            // TODO: count should exclude the properties that are hidden
            // TODO: include inherited properties too
            HasMore = obj.Value.Count > (currentItem + 1);

            if (!HasMore)
            {
                return null;
            }
            currentItem++;

            HasMore = obj.Value.Count > (currentItem + 1);

            return obj.Value[currentItem];
        }

        public int Index { get => currentItem; }

    }

    public class PropertyLocator
    {
        public string[] pathElements { get; set; }
        int currentPosition = 0;

        public ModelPair Locate(string path, ModelCollector modelList)
        {
            pathElements = path.Split('.');
            currentPosition = 0;

            if (path.StartsWith("#")) // parameter
            {
                currentPosition++;
                return Locate(modelList.ParameterModels.Single(s => s.Name == pathElements[0].Substring(1)).Properties);
            }
            if (path.StartsWith("@")) // model
            {
                currentPosition++;
                return Locate(modelList.SingleModels.Where(s => s.Section == pathElements[0].Substring(1)));
            }
            return null;
        }

        public ModelPair Locate(string path, ModelPair root)
        {
            pathElements = path.Split('.');
            currentPosition = 0;

            return Locate(root);
        }

        private ModelPair Locate(IEnumerable<ModelPair> root)
        {
            var newRoot = root.SingleOrDefault(s => s.Name == pathElements[currentPosition]);
            if (newRoot != null)
            {
                currentPosition++;
                return Locate(newRoot);
            }
            return null;
        }

        private ModelPair Locate(ModelPair root)
        {
            if (currentPosition == pathElements.Length && root.Name == pathElements[currentPosition-1])
            {
                if (root.Modified && root.Modifier == "-")
                {
                    // does not exist
                    return null;
                }
                return root;
            }
            if (root.IsObject())
            {
                if (!root.IsResolved)
                {
                    PropertyResolver propResolver = new PropertyResolver();
                    propResolver.PopulateProperties(root);
                    root.AsObject().Value = propResolver.AllProperties;
                    root.IsResolved = true;
                }
                //currentPosition++;
                return Locate(root.AsObject().Value);
            }
            return null;
        }
    }

}
