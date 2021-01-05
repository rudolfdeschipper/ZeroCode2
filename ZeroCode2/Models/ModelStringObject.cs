namespace ZeroCode2.Models
{
    public class ModelStringObject : ModelObject<string>
    {
        public override IModelObject Duplicate()
        {
            return new ModelStringObject()
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
            return Value;
        }

    }
}
