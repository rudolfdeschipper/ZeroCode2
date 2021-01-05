using System.Collections.Generic;
using System.Linq;

namespace ZeroCode2.Models
{
    public class ModelCompositeObject : ModelObject<List<IModelObject>>
    {
        public List<string> OrderBy { get; set; } = new List<string>();
        public override IModelObject Duplicate()
        {
            var obj = new ModelCompositeObject()
            {
                Inherits = Inherits,
                InheritsFrom = InheritsFrom,
                IsResolved = IsResolved,
                Modified = Modified,
                Modifier = Modifier,
                Name = Name,
                ParentObject = ParentObject,
                Value = new List<IModelObject>()
            };
            obj.Value.AddRange(Value.Select(p => p.Duplicate()));
            obj.OrderBy.AddRange(OrderBy);

            return obj;
        }

        public override string GetText()
        {
            var r = Value.Select(m => m.Name + " = " + m.GetText());
            string s = "{ ";

            string obs = "/ ";
            string orderings = OrderBy.Any() ? " " + OrderBy.Aggregate(obs, (f, run) => obs += run + ", ") + " " : "";

            return r.Aggregate(s, (f, run) => s += run + " ") + orderings + "}";
        }
    }
}
