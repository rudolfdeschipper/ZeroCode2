namespace ZeroCode2.Models
{
    public class ModelNumberObject : ModelObject<decimal>
    {
        public override IModelObject Duplicate()
        {
            return new ModelNumberObject()
            {
                Inherits = this.Inherits,
                InheritsFrom = this.InheritsFrom,
                IsResolved = this.IsResolved,
                Modified = this.Modified,
                Modifier = this.Modifier,
                Name = this.Name,
                ParentObject = this.ParentObject,
                Value = this.Value
            };
        }

        public override string GetText()
        {
            return Value.ToString();
        }
    }
}
