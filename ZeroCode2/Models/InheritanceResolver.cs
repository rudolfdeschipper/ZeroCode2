using System.Collections.Generic;
using System.Linq;

namespace ZeroCode2.Models
{
    /// <summary>
    /// Resolves the inheritance in the model. Errors are reported through the Errors collection
    /// </summary>
    public class InheritanceResolver
    {
        public List<string> Errors { get; set; } = new List<string>();

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public bool ResolveInheritance(IModelObject pair, ModelCollector modelList)
        {
            var ok = true;
            // only try to populate it if the ParentObject is still set to null
            if (pair.Inherits == true && pair.ParentObject == null)
            {
                var locator = new Models.PropertyLocator("@" + pair.InheritsFrom, modelList, null);

                if (locator.Locate())
                {
                    pair.ParentObject = locator.LocatedProperty();
                }

                ok = pair.ParentObject != null;
                if (!ok)
                {
                    logger.Error(string.Format("{0} inheritance to {1} could not be resolved", pair.Name, pair.InheritsFrom));
                    Errors.Add(string.Format("{0} inheritance to {1} could not be resolved", pair.Name, pair.InheritsFrom));
                }
            }
            if (ok)
            {
                // check next parents for cycles:
                ok &= CheckForCycle(pair);
            }
            if (pair.IsComposite())
            {
                // recurse:
                foreach (var item in pair.AsComposite().Value)
                {
                    ok &= ResolveInheritance(item, modelList);
                }
            }
            return ok;
        }

        public bool ResolveInheritance(SingleModel model, ModelCollector modelList)
        {
            var ok = true;
            // only try to populate it if the ParentObject is still set to null
            if (model.Inherits == true && model.ParentObject == null)
            {
                var locator = new Models.PropertyLocator("@" + model.InheritsFrom, modelList, null);

                if (locator.Locate())
                {
                    model.ParentObject = locator.LocatedProperty();
                }

                // return if resolved
                ok = model.ParentObject != null;
                if (!ok)
                {
                    logger.Error(string.Format("{0} inheritance to {1} could not be resolved", model.Name, model.InheritsFrom));
                    Errors.Add(string.Format("{0} inheritance to {1} could not be resolved", model.Name, model.InheritsFrom));
                }
            }
            if (ok)
            {
                // check next parents for cycle:
                ok &= CheckForCycle(model);
            }
            // dive into properties too
            foreach (var item in model.Value)
            {
                ok &= ResolveInheritance(item, modelList);
            }
            // no issue
            return ok;
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
