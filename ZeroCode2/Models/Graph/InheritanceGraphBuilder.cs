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
            logger.Trace("Building graph");
            do
            {
                Changed = false;
                foreach (KeyValuePair<string, GraphElement> item in Elements)
                {
                    if (item.Value.State == GraphElementSate.Unvisited)
                    {
                        logger.Trace("Visiting item {0}", item.Key);
                        item.Value.State = GraphElementSate.Visited;
                        ProcessGraphElement(item.Value);
                    }
                }
                // here we must populate all the items that are processed
                // and insert the new properies in the Elements list
                logger.Trace("Call PopulateProperties");
                PopulateProperties();
            } while (Changed);
            logger.Trace("Done Building graph");

            bool cyclesOk = CheckForCycles();
            logger.Trace("Check for cycles: {0}", cyclesOk);

            bool elementsOk = !Elements.Values.Any(i => i.State != GraphElementSate.Processed);
            logger.Trace("Check if all elements processed: {0}", elementsOk);

            return cyclesOk && elementsOk;
        }

        private void PopulateProperties()
        {
            GraphPropertyResolver propertyResolver = new GraphPropertyResolver
            {
                Elements = Elements
            };
            List<KeyValuePair<string, GraphElement>> toPopulate = Elements.Where(i => i.Value.State == GraphElementSate.Processed && !i.Value.Object.IsResolved).ToList();
            foreach (KeyValuePair<string, GraphElement> item in toPopulate)
            {
                logger.Trace("Populate properties for item {0}", item.Key);
                propertyResolver.PopulateProperties(item.Value);
            }
            logger.Trace("Done PopulateProperties");
        }

        private void ProcessGraphElement(GraphElement item)
        {
            if (item.Object.Inherits)
            {
                logger.Trace("Finding parent {1} for {0}", item.Key, item.Object.InheritsFrom);
                GraphElement inheritedObject = FindElement("@" + item.Object.InheritsFrom);
                if (inheritedObject != null)
                {
                    logger.Trace("Setting parent object for {0} to {1}, item State is Processed", item.Key, inheritedObject.Key);
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
                    logger.Trace("Did not find parent for {0} - reset State to Unvisited", item.Key);
                    item.State = GraphElementSate.Unvisited;
                }
            }
            else
            {
                logger.Trace("Item {0} does not inherit, set to Processed", item.Key);
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

        private bool CheckForCycles()
        {
            bool ok = true;
            foreach (KeyValuePair<string, GraphElement> item in Elements)
            {
                if (item.Value.Object.Inherits && item.Value.State == GraphElementSate.Processed)
                {
                    ok &= CheckForCycle(item.Value.Object);
                }
            }
            return ok;
        }

        private bool CheckForCycle(IModelObject model)
        {
            IModelObject parent = model.ParentObject;
            Stack<IModelObject> stack = new Stack<IModelObject>();
            while (parent != null)
            {
                IModelObject cycle = stack.FirstOrDefault(s => s == parent);
                if (cycle != null)
                {
                    logger.Error(string.Format("{0} inheritance defines a cyclic relation.", cycle.Path));
                    Errors.Add(string.Format("{0} inheritance defines a cyclic relation.", cycle.Path));
                    return false;
                }
                stack.Push(parent);

                parent = parent.ParentObject;
            }
            return true;
        }
    }
}
