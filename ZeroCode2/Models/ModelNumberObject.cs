namespace ZeroCode2.Models
{
    public class ModelNumberObject : ModelObject<decimal>
    {
        public override IModelObject Duplicate()
        {
            return new ModelNumberObject()
            {
                Inherits = Inherits,
                InheritsFrom = InheritsFrom,
                IsResolved = IsResolved,
                Modified = Modified,
                Modifier = Modifier,
                Name = Name,
                ParentObject = ParentObject,
                Value = Value
            };
        }

        public override string GetText()
        {
            return Value.ToString();
        }
    }
}
