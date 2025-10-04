namespace ZeroCode2.Models
{
    public interface IIterator
    {
        bool HasMore { get; }
        int Index { get; }

        IModelObject Iterate(IModelObject mp);
    }
}