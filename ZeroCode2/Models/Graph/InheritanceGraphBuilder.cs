using System.Collections.Generic;
using System.Linq;

namespace ZeroCode2.Models.Graph
{
    public class InheritanceGraphBuilder
    {
        public List<string> Errors { get; set; } = new List<string>();

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Dictionary<string, GraphElement> Elements { get; set; }
        private bool Changed = false;

        public bool BuildGraph()
        {
            do
            {
                Changed = false;
                foreach (var item in Elements)
                {
                    if (item.Value.State == GraphElementSate.Unvisited)
                    {
                        item.Value.State = GraphElementSate.Visited;
                        ProcessGraphElement(item.Value);
                    }
                }
                // here we must populate all the items that are processed
                // and insert the new properies in the Elements list
                PopulateProperties();
            } while (Changed);

            return !Elements.Values.Any(i => i.State != GraphElementSate.Processed);
        }

        private void PopulateProperties()
        {
            var propertyResolver = new GraphPropertyResolver
            {
                Elements = Elements
            };
            var toPopulate = Elements.Where(i => i.Value.State == GraphElementSate.Processed && !i.Value.Object.IsResolved).ToList();
            foreach (var item in toPopulate)
            {
                propertyResolver.PopulateProperties(item.Value);
            }
        }

        private void ProcessGraphElement(GraphElement item)
        {
            if (item.Object.Inherits)
            {
                var inheritedObject = FindElement("@" + item.Object.InheritsFrom);
                if (inheritedObject != null)
                {
                    item.Object.ParentObject = inheritedObject.Object;
                    item.State = GraphElementSate.Processed;
                    Changed = true;
                }
                else
                {
                    // did not find the parent because
                    // 1. Error in the model
                    // 2. The inheritance is into an object the is not yet resolved
                    // so we put the state to pending, to revisit later
                    item.State = GraphElementSate.Unvisited;
                }
            }
            else
            {
                item.State = GraphElementSate.Processed;
            }
        }

        public GraphElement FindElement(string path)
        {
            if (Elements.ContainsKey(path))
            {
                return Elements[path];
            }
            return null;
        }

        private bool CheckForCycle(IModelObject model)
        {
            var parent = model.ParentObject;
            Stack<IModelObject> stack = new Stack<IModelObject>();
            while (parent != null)
            {
                var cycle = stack.FirstOrDefault(s => s == parent);
                if (cycle != null)
                {
                    logger.Error(string.Format("{0} inheritance defines a cyclic relation.", cycle.Name));
                    Errors.Add(string.Format("{0} inheritance defines a cyclic relation.", cycle.Name));
                    return false;
                }
                stack.Push(parent);

                parent = parent.ParentObject;
            }
            return true;
        }
    }
}
