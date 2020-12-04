using System.Collections.Generic;
using System.Linq;

namespace ZeroCode2.Models.Graph
{
    /// <summary>
    /// Resolves the properties that are inherited.
    /// Resulting property list can be accessed through AllProperties
    /// Note that the resolution follows the inheritance patch, but does not resolve any inheritance of sub-objects
    /// PersonVM inheriting from Person will resolve to Person, 
    /// but a property within PersonVM that itself inherits from an object will not be resolved unless the PropertyResolver is called on the list
    /// containing that property
    /// 
    /// As the resolution is done as part of an Iterator, this works fine because each iterator will first resolve all the properties in the list it
    /// will iterate through
    /// </summary>
    public class GraphPropertyResolver
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Dictionary<string, GraphElement> Elements { get; set; }

        public void PopulateProperties(GraphElement element)
        {
            var pair = element.Object;
            if (element.State != GraphElementSate.Processed)
            {
                logger.Trace("Populate Properties for {0} - element not processed yet", pair.Name);
                return;
            }
            // already resolved
            if (pair.IsResolved)
            {
                logger.Trace("Populate Properties for {0} - already resolved", pair.Name);
                return;
            }
            // not an object
            if (!pair.IsComposite())
            {
                logger.Trace("Populate Properties for {0} - not a composite", pair.Name);
                pair.IsResolved = true;
                return;
            }

            // resolve properties first:
            logger.Trace("Populate Properties for properties of {0}", pair.Name);
            foreach (var item in pair.AsComposite().Value)
            {
                logger.Trace("Populate Properties for {0}", item.Name);
                var childElement = Elements[item.Path];
                PopulateProperties(childElement);
            }

            // no inheritance
            if (!pair.Inherits)
            {
                logger.Trace("Populate Properties for {0} - no inheritance", pair.Name);
                // if ordering is present, issue a warning:
                if ((pair as ModelCompositeObject).OrderBy.Any())
                {
                    logger.Warn("Object {0} has ordering but does not inherit. No ordering was applied - please order the properties in the object definition", pair.Name);
                }
                pair.IsResolved = true;
                return;
            }

            // invalid inheritance chain
            if (pair.ParentObject == null)
            {
                logger.Error("{0} inherits from {1} but was not resolved", pair.Name, pair.InheritsFrom);
                //pair.IsResolved = true;
                return;
            }
            if (pair.ParentObject.IsComposite())
            {
                List<IModelObject> newProps = new List<IModelObject>();

                // only copy the "+" from the current list:
                logger.Trace("Populate Properties for {0} - adding {1} +-modifier properties", pair.Name, pair.AsComposite().Value.Count(mp => mp.Modifier == "+"));
                ResolveAddedProps(pair.AsComposite().Value.Where(mp => mp.Modifier == "+").ToList());

                // then, recurse into the children of the ParentObject, to ensure all is resolved before we copy from it
                PopulatePropertiesOfParentChildren(pair.Name, pair.ParentObject);

                logger.Trace("Populate Properties for {0} - adding {1} inherited properties", pair.Name, pair.ParentObject.AsComposite().Value.Count(mp => mp.Modifier != "-"));

                var inheritedProperties = pair.ParentObject.AsComposite().Value.Where(mp => mp.Modifier != "-").Select(p => p.Duplicate()).ToList();

                // and we add any changed properties back in too:
                MergePairWithInheritedPropsIntoNewProps(pair, inheritedProperties, newProps);

                // set the new set as the value:
                pair.AsComposite().Value = OrderProperties(pair as ModelCompositeObject, newProps);
                logger.Trace("Merged Properties for {0}: {1}", pair.Name, pair.GetText());
            }
            else
            {
                logger.Error("{0} inherits from {1} but this is not an object", pair.Name, pair.InheritsFrom);
            }
            pair.IsResolved = true;
        }

        private void MergePairWithInheritedPropsIntoNewProps(IModelObject pair, List<IModelObject> inheritedProps, List<IModelObject> newProps)
        {
            // need to run through the full set, which is the sum of both sets
            List<string> resultingList = inheritedProps.Select(p => p.Name).ToList();
            resultingList.AddRange(pair.AsComposite().Value.Select(p => p.Name));

            resultingList = resultingList.Distinct().ToList();

            foreach (var item in resultingList)
            {
                var itemFromInheritedList = inheritedProps.Find(p => p.Name == item);
                var itemFromPair = pair.AsComposite().Value.Find(p => p.Name == item);

                logger.Trace("Merge Properties for {0} - merging {1}", pair.Name, item);

                if (itemFromPair == null)
                {
                    // does not exist in the pair, just use the inherited item
                    logger.Trace("Merge Properties for {0} - adding it with value from inherited property {1}", item, itemFromInheritedList.GetText());
                    newProps.Add(itemFromInheritedList);
                }
                if (itemFromInheritedList == null)
                {
                    // not an inherited item, just use this one
                    if (itemFromPair.Modifier != "-")
                    {
                        logger.Trace("Merge Properties for {0} - adding non-modified property with value {1}", item, itemFromPair.GetText());
                        newProps.Add(itemFromPair);
                    }
                    else
                    {
                        // removing a non-inherited property
                        logger.Trace("Merge Properties for {0} - removing {1} which is not inherited - ignored", item, item);
                    }
                }
                if (itemFromPair != null && itemFromInheritedList != null)
                {
                    // need to do a proper merge
                    if (itemFromPair.Modifier != "-")
                    {
                        if (itemFromPair.IsComposite() && itemFromInheritedList.IsComposite())
                        {
                            logger.Trace("Merge Properties for {0} - Property {0} is composite - merging its properties", item);
                            // dive into the tree first
                            List<IModelObject> newSubProps = new List<IModelObject>();
                            MergePairWithInheritedPropsIntoNewProps(itemFromPair, itemFromInheritedList.AsComposite().Value, newSubProps);
                            itemFromPair.AsComposite().Value = OrderProperties(itemFromPair as ModelCompositeObject, newSubProps);
                        }
                        else
                        {
                            logger.Trace("Merge Properties for {0} - Property {0} is not composite - and exists in property and its parent - set value to {1}", item, itemFromPair.GetText());
                        }
                        logger.Trace("Merge Properties for {0} - setting merged property to {1}", itemFromPair.Name, itemFromPair.GetText());
                        newProps.Add(itemFromPair);
                    }
                    else
                    {
                        logger.Trace("Merge Properties for {0} - removing {1}", pair.Name, item);
                    }
                }
            }
            AddInheritedPropertiesToElements(pair.Path, newProps);
        }

        //private void MergeProperties(IModelObject itemToUpdate, IModelObject item)
        //{
        //    // for each item, run through its properties
        //    // if a property exists in itemToUpdate, do one of the following:
        //    // 1: item has no modifier - recursively apply MergeProperties
        //    // 2: item has a "+": it is an added property, so don't expect to find it in the item
        //    // 3: item has a "-": it must be removed from the tree and not be merged
        //    //   note that a removal cannot be done here, as we have no access to the parent
        //    // if it does not exist, add it
        //    if (itemToUpdate.IsComposite() && item.IsComposite())
        //    {
        //        foreach (var prop in item.AsComposite().Value)
        //        {
        //            var propToUpdateIndex = itemToUpdate.AsComposite().Value.FindIndex(p => p.Name == prop.Name);
        //            if (propToUpdateIndex != -1)
        //            {
        //                var propToUpdate = itemToUpdate.AsComposite().Value[propToUpdateIndex];
        //                if (prop.Modified == false)
        //                {
        //                    if (propToUpdate.IsComposite())
        //                    {
        //                        MergeProperties(propToUpdate, prop);
        //                    }
        //                    else
        //                    {
        //                        itemToUpdate.AsComposite().Value[propToUpdateIndex] = prop;
        //                    }
        //                }
        //                else
        //                {
        //                    if (prop.Modifier == "+")
        //                    {
        //                        // prop is added, so cannot inherit
        //                        // no action
        //                    }
        //                    if (prop.Modifier == "-")
        //                    {
        //                        // can remove this prop
        //                        itemToUpdate.AsComposite().Value.RemoveAt(propToUpdateIndex);
        //                        // update the Elements too
        //                        Elements.Remove(prop.Path);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                itemToUpdate.AsComposite().Value.Add(prop);
        //            }
        //        }
        //        return;
        //    }
        //    if (itemToUpdate.IsBool() && item.IsBool())
        //    {
        //        itemToUpdate.AsBool().Value = item.AsBool().Value;
        //        return;
        //    }
        //    if (itemToUpdate.IsNumber() && item.IsNumber())
        //    {
        //        itemToUpdate.AsNumber().Value = item.AsNumber().Value;
        //        return;
        //    }
        //    if (itemToUpdate.IsString() && item.IsString())
        //    {
        //        itemToUpdate.AsString().Value = item.AsString().Value;
        //        return;
        //    }
        //}

        private void PopulatePropertiesOfParentChildren(string Name, IModelObject ParentObject)
        {
            logger.Trace("Populate Properties for Parent of {0} - {1}", Name, ParentObject.Name);
            var parentElement = Elements[ParentObject.Path];
            PopulateProperties(parentElement);
        }

        private void ResolveAddedProps(List<IModelObject> addedProps)
        {
            // we need to resolve these too, so we do that here:
            foreach (var item in addedProps)
            {
                logger.Trace("Populate Properties for added property {0}", item.Name);
                var childElement = Elements[item.Path];
                PopulateProperties(childElement);
            }
        }

        private void AddInheritedPropertiesToElements(string basePath, IEnumerable<IModelObject> inheritedProperties)
        {
            foreach (var item in inheritedProperties)
            {
                // fixup the new Path
                item.Path = basePath + "." + item.Name;
                // the Duplicate was recursive, so we need to add all children too:
                if (item.IsComposite())
                {
                    AddInheritedPropertiesToElements(item.Path, item.AsComposite().Value);
                }
                // enter them in Elements - if already present, replace it
                if (Elements.ContainsKey(item.Path))
                {
                    Elements[item.Path] = new GraphElement(item.Path, item);
                }
                else
                {
                    Elements.Add(item.Path, new GraphElement(item.Path, item));
                }
            }
        }

        private List<IModelObject> OrderProperties(ModelCompositeObject pair, List<IModelObject> Props)
        {
            if (pair == null)
            {
                logger.Warn("Calling Ordering on non-composite object");
                return Props;
            }
            if (pair.OrderBy?.Count() == 0)
            {
                logger.Trace("No ordering statement on object {0}", pair.Name);
                return Props;
            }
            // we have some ordering:
            logger.Trace("Ordering statement on object {0}: {1}", pair.Name, pair.OrderBy.Count());
            var orderedProps = new List<IModelObject>();
            foreach (var item in pair.OrderBy)
            {
                logger.Trace("Ordering by {0}", item);
                var toOrder = Props.FirstOrDefault(o => o.Name == item);
                if (toOrder != null)
                {
                    logger.Trace("Ordering {0} found, enter in property list", item);
                    orderedProps.Add(toOrder);
                    Props.Remove(toOrder);
                }
                else
                {
                    logger.Warn("Orderby property {0} does not exist on this object", item);
                }
            }
            // add any remaining properties in their natural order
            logger.Trace("Adding remaining {0} properties", Props.Count());
            orderedProps.AddRange(Props);

            return orderedProps;
        }

    }
}
