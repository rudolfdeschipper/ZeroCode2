namespace ZeroCode2.Models
{
    /// <summary>
    /// This class allows to iterate through the various collections. HasMore indicates if more items are present.
    /// If not, the Iterate method returns null. It also returns null if the iterations fails for another reason.
    /// TODO: include inherited properties and exclude the ones that are "-"-ed.
    /// </summary>
    public class Iterator
    {
        private int currentItem = -1;

        public bool HasMore { get; private set; }

        public IModelObject Iterate(IModelObject mp)
        {
            var obj = mp.AsComposite();

            if (obj == null)
            {
                return null;
            }

            HasMore = obj.Value.Count > (currentItem + 1);

            if (!HasMore)
            {
                return null;
            }
            currentItem++;

            HasMore = obj.Value.Count > (currentItem + 1);

            if (obj.Value[currentItem] != null)
            {
                obj.Value[currentItem].Resolve();
            }

            return obj.Value[currentItem];
        }

        public int Index => currentItem;

    }
}
