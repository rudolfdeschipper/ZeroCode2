namespace ZeroCode2.Models
{
    public class ModelBoolObject : ModelObject<bool>
    {
        public override IModelObject Duplicate()
        {
            return new ModelBoolObject()
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
            return Value ? "true" : "false";
        }

    }
}
