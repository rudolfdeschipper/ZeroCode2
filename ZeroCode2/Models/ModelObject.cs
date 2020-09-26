using System.Collections.Generic;

namespace ZeroCode2.Models
{

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
            return Is<decimal>();
        }

        public bool IsComposite()
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

        public abstract IModelObject Duplicate();

        public int CompareTo(IModelObject other)
        {
            return this.ToString().CompareTo(other.ToString());
        }
    }
}
