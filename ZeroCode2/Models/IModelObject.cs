using System;
using System.Collections.Generic;

namespace ZeroCode2.Models
{
    public interface IModelObject : IComparable<IModelObject>
    {
        string Name { get; set; }

        string Path { get; set; }
        bool Inherits { get; set; }
        string InheritsFrom { get; set; }
        IModelObject ParentObject { get; set; }
        bool IsResolved { get; set; }

        bool IsBeingResolved { get; set; }

        bool Modified { get; set; }
        string Modifier { get; set; }

        bool Is<P>();
        bool IsString();
        bool IsNumber();
        bool IsComposite();
        bool IsBool();

        ModelObject<M> As<M>() where M : class;

        ModelObject<string> AsString();

        ModelObject<decimal> AsNumber();

        ModelObject<bool> AsBool();

        ModelObject<List<IModelObject>> AsComposite();

        string GetText();
        IModelObject Duplicate();
    }
}
