namespace ZeroCode2.Interpreter
{
    public class IteratorManager
    {
        private Models.Iterator Iterator { get; set; }

        private string _path;
        private string _loopid;
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                if (value.StartsWith("@") || value.StartsWith("#"))
                {
                    _loopid = value.Substring(1);
                }
                else
                {
                    _loopid = value;
                }
            }
        }
        /// <summary>
        /// Returns the Loop ID of the iterator. Use this to refer to the loop
        /// </summary>
        public string LoopID { get { return _loopid; } }
        public Models.IModelObject Root { get; set; }
        public Models.IModelObject CurrentModel { get; private set; }

        public IteratorManager(Models.Iterator _iterator)
        {
            Iterator = _iterator;
        }

        public IteratorManager(Models.Iterator _iterator, Models.IModelObject _currentModel)
        {
            Iterator = _iterator;
            CurrentModel = _currentModel;
        }

        public bool Iterate()
        {
            CurrentModel = Iterator.Iterate(Root);

            return Iterator.HasMore;
        }

        public bool HasMore => Iterator.HasMore;

        public int Index => Iterator.Index;
    }

}
