using System.Collections.Generic;

namespace ZeroCode2.Models
{
    public class SingleModel : ModelCompositeObject
    {
        public SingleModel(string name, IEnumerable<IModelObject> properties)
        {
            Name = name;
            Value = new List<IModelObject>();

            Value.AddRange(properties);
        }
    }
}
