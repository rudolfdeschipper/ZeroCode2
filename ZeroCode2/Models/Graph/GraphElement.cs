namespace ZeroCode2.Models.Graph
{
    public enum GraphElementSate
    {
        Unvisited,
        Visited,
        Processed
    }

    public class GraphElement
    {
        public GraphElement(string key, IModelObject obj)
        {
            Key = key;
            Object = obj;
        }
        public string Key { get; set; }
        public IModelObject Object { get; set; }
        public GraphElementSate State { get; set; } = GraphElementSate.Unvisited;
    }
}
