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
                Inherits = this.Inherits,
                InheritsFrom = this.InheritsFrom,
                IsResolved = this.IsResolved,
                Modified = this.Modified,
                Modifier = this.Modifier,
                Name = this.Name,
                ParentObject = this.ParentObject,
                Value = new List<IModelObject>()
            };
            obj.Value.AddRange(this.Value.Select(p => p.Duplicate()));
            obj.OrderBy.AddRange(this.OrderBy);

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
