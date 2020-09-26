using System.Collections.Generic;
using System.Linq;

namespace ZeroCode2.Models
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
    public class PropertyResolver
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void PopulateProperties(IModelObject pair)
        {
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
                PopulateProperties(item);
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
                pair.IsResolved = true;
                return;
            }
            if (pair.ParentObject.IsComposite())
            {
                List<IModelObject> newProps = new List<IModelObject>();
                List<IModelObject> addedProps = new List<IModelObject>();

                // only copy the "+" from the current list:
                logger.Trace("Populate Properties for {0} - adding {1} +-modifier properties", pair.Name, pair.AsComposite().Value.Count(mp => mp.Modifier == "+"));
                addedProps.AddRange(pair.AsComposite().Value.Where( mp => mp.Modifier == "+"));
 
                // we need to resolve these too, so we do that here:
                foreach (var item in addedProps)
                {
                    logger.Trace("Populate Properties for added property {0}", item.Name);
                    PopulateProperties(item);
                }

                // then, recurse into the children of the ParentObject, to ensure all is resolved before we copy from it
                logger.Trace("Populate Properties for Parent of {0} - {1}", pair.Name, pair.ParentObject.Name);
                PopulateProperties(pair.ParentObject);

                // Now we can add all properties from the inheritance object
                // ensure we make copies of them to ensure we don't overwrite the original

                logger.Trace("Populate Properties for {0} - adding {1} inherited properties", pair.Name, pair.ParentObject.AsComposite().Value.Count(mp => mp.Modifier != "-"));
                newProps.AddRange(pair.ParentObject.AsComposite().Value.Where(mp => mp.Modifier != "-").Select(p => p.Duplicate()));

                // and we add any changed properties back in too:
                foreach (var item in pair.AsComposite().Value.Where(p => !p.Modified || p.Modifier == "-"))
                {
                    logger.Trace("Populate Properties for {0} - adding {1}", pair.Name, item.Name);
                    // find if it is already there
                    int index = newProps.FindIndex(mp => mp.Name == item.Name);

                    // if the property already exists, we merge the two elements
                    if (index != -1)
                    {
                        // only if not modified, if it is modified either it is added and we don't 
                        // have to try to merge (nothing to do)
                        // or it is deleted, and then it shall be removed, so no merging needed either
                        if (!item.Modified)
                        {
                            var itemToUpdate = newProps[index];

                            logger.Trace("Populate Properties for {0} - merging {1} with inherited property", pair.Name, item.Name);
                            MergeProperties(itemToUpdate, item);

                        }
                        if (item.Modifier == "-")
                        {
                            logger.Trace("Populate Properties for {0} - removing {1}", pair.Name, item.Name);
                            newProps.RemoveAt(index);
                        }
                    }
                    else
                    {
                        logger.Trace("Populate Properties for {0} - {1} is not found", pair.Name, item.Name);
                        // item does not exist
                        if (!item.Modified)
                        {
                            logger.Trace("Populate Properties for {0} - adding non-modified {1}", pair.Name, item.Name);
                            // actually this means it was in fact a "+" but we tolerate this
                            newProps.Add(item);
                        }
                        else
                        {
                            logger.Warn("Populate Properties for {0} - {1} was not found", pair.Name, item.Name);
                        }
                    }
                }

                // put added props at the end
                logger.Trace("Populate Properties for {0} - adding remainng {1} properties", pair.Name, addedProps.Count());
                newProps.AddRange(addedProps);

                // set the new set as the value:
                pair.AsComposite().Value = OrderProperties(pair as ModelCompositeObject, newProps);
            }
            else
            {
                logger.Error("{0} inherits from {1} but this is not an object", pair.Name, pair.InheritsFrom);
            }
            pair.IsResolved = true;
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

        private void MergeProperties(IModelObject itemToUpdate, IModelObject item)
        {
            // for each item, run through its properties
            // if a property exists in itemToUpdate, overwrite it
            // if it does not exist, add it
            if (itemToUpdate.IsComposite() && item.IsComposite())
            {
                foreach (var prop in item.AsComposite().Value)
                {
                    var propToUpdate = itemToUpdate.AsComposite().Value.FindIndex(p => p.Name == prop.Name);
                    if (propToUpdate != -1)
                    {
                        itemToUpdate.AsComposite().Value[propToUpdate] = prop;
                    }
                    else
                    {
                        itemToUpdate.AsComposite().Value.Add(prop);
                    }
                }
                return;
            }
            if (itemToUpdate.IsBool() && item.IsBool())
            {
                itemToUpdate.AsBool().Value = item.AsBool().Value;
                return;
            }
            if (itemToUpdate.IsNumber() && item.IsNumber())
            {
                itemToUpdate.AsNumber().Value = item.AsNumber().Value;
                return;
            }
            if (itemToUpdate.IsString() && item.IsString())
            {
                itemToUpdate.AsString().Value = item.AsString().Value;
                return;
            }
        }
    }
}
