using System.Collections.Generic;

namespace ZeroCode2.Models
{
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
}
