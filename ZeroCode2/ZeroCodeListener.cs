using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Linq;
using ZeroCode2.Grammars;
using ZeroCode2.Models.Graph;
using static ZeroCode2.Grammars.ZeroCode2;

namespace ZeroCode2
{
    public class ZeroCodeListener : ZeroCode2BaseListener
    {
        // Logging
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ModelCollector Collector { get; set; } = new ModelCollector();

        public Dictionary<string, GraphElement> GraphElements { get; set; } = new Dictionary<string, GraphElement>();

        public Antlr4.Runtime.Tree.ParseTreeProperty<Models.IModelObject> ValueProps { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<Models.IModelObject>();
        public Antlr4.Runtime.Tree.ParseTreeProperty<Models.IModelObject> PairProps { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<Models.IModelObject>();
        public Antlr4.Runtime.Tree.ParseTreeProperty<List<Models.IModelObject>> ObjProps { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<List<Models.IModelObject>>();
        public Antlr4.Runtime.Tree.ParseTreeProperty<List<string>> ObjOrderByProps { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<List<string>>();
        public Antlr4.Runtime.Tree.ParseTreeProperty<Models.SingleModel> SingleModels { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<Models.SingleModel>();

        private readonly ObjectPathStack path = new ObjectPathStack();

        public override void EnterParameters([NotNull] ParametersContext context)
        {
            string CurrentSection = context.ID().GetText();

            logger.Trace(" Enter Params={0}", CurrentSection);

            base.EnterParameters(context);
        }

        public override void ExitParameters([NotNull] ParametersContext context)
        {
            logger.Trace(" Exit Params={0}", context._pairs.Aggregate("", (a, r) => a += r.ID().GetText() + " = " + r.pairvalue().GetText() + "\n"));

            Models.ParameterModel parameter = new Models.ParameterModel("#" + context.ID().GetText());
            //parameter.Section = CurrentSection;
            foreach (PairContext item in context._pairs)
            {
                Models.IModelObject p = PairProps.Get(item);
                parameter.Value.Add(p);
            }
            // check for doubles
            Models.ParameterModel existing = Collector.ParameterModels.FirstOrDefault(p => p.Name == parameter.Name);
            if (existing != null)
            {
                existing.Value.AddRange(parameter.Value);
            }
            else
            {
                Collector.ParameterModels.Add(parameter);
            }

            base.ExitParameters(context);
        }

        public override void EnterGenericModel([NotNull] GenericModelContext context)
        {
            string CurrentSection = "@" + context.ID().GetText();
            logger.Trace("Enter Section = {0}", CurrentSection);
            path.Push(CurrentSection);
        }

        public override void ExitGenericModel([NotNull] GenericModelContext context)
        {
            List<Models.IModelObject> list = new List<Models.IModelObject>();

            foreach (SinglemodelContext item in context._smodels)
            {
                Models.SingleModel sm = SingleModels.Get(item);
                list.Add(sm);
            }

            Models.SingleModel TopLevelModel = new Models.SingleModel("@" + context.ID().GetText(), list)
            {
                Path = path.GetPath()
            };

            // check for doubles
            Models.SingleModel existing = Collector.SingleModels.FirstOrDefault(p => p.Name == TopLevelModel.Name);
            if (existing != null)
            {
                existing.Value.AddRange(TopLevelModel.Value);
            }
            else
            {
                Collector.SingleModels.Add(TopLevelModel);
            }
            GraphElements.Add(TopLevelModel.Path, new GraphElement(TopLevelModel.Path, TopLevelModel));
            path.Pop();

            base.ExitGenericModel(context);
        }

        public override void EnterPair([NotNull] PairContext context)
        {
            base.EnterPair(context);
            logger.Trace("Enter pair {0}", context.ID().GetText());
            path.Push(context.ID().GetText());
        }

        public override void ExitPair([NotNull] PairContext context)
        {
            string fullpath = path.GetPath();
            path.Pop();
            logger.Trace("Exit pair {0}", fullpath);

            Models.IModelObject pair = null;
            if (context.pairvalue()?.value() != null)
            {
                pair = ValueProps.Get(context.pairvalue().value());
            }
            if (pair == null)
            {
                List<Models.IModelObject> value = new List<Models.IModelObject>();
                pair = new Models.ModelCompositeObject
                {
                    Value = value
                };
            }
            pair.Name = context.ID().GetText();

            pair.Path = fullpath;

            PairProps.Put(context, pair);
            if (context.pairvalue()?.inherits() != null)
            {
                pair.Inherits = true;
                pair.InheritsFrom = context.pairvalue().inherits().ID().GetText();
            }
            if (context.modifier != null)
            {
                pair.Modified = true;
                pair.Modifier = context.modifier.Text;
            }
            GraphElements.Add(pair.Path, new GraphElement(pair.Path, pair));

            logger.Trace("Pair: {0}{1} = {2}{3}", pair.Modifier, pair.Name, pair.GetText() ?? "unknown", context.pairvalue()?.inherits() != null ? " : " + pair.InheritsFrom : "");

            base.ExitPair(context);
        }

        public override void EnterSinglemodel([NotNull] SinglemodelContext context)
        {
            base.EnterSinglemodel(context);
            path.Push(context.ID().GetText());
        }

        public override void ExitSinglemodel([NotNull] SinglemodelContext context)
        {
            string fullpath = path.GetPath();
            path.Pop();
            logger.Trace("Exit singlemodel {0}", fullpath);

            List<Models.IModelObject> obj = ObjProps.Get(context.obj());

            Models.SingleModel model = new Models.SingleModel(context.ID().GetText(), obj);

            List<string> orderby = ObjOrderByProps.Get(context.obj());
            model.Path = fullpath;
            model.OrderBy.AddRange(orderby);

            //model.Section = CurrentSection;
            if (context.inherits() != null)
            {
                model.Inherits = true;
                model.InheritsFrom = context.inherits().ID().GetText();
            }
            string obs = "";
            logger.Trace("Single model = {0}{1}{2}",
                model.Name,
                context.inherits() != null ? " : " + model.InheritsFrom : "",
                orderby.Any() ? " / " + orderby.Aggregate(obs, (f, run) => obs += run + ", ") : "");

            GraphElements.Add(model.Path, new GraphElement(model.Path, model));
            SingleModels.Put(context, model);
            base.ExitSinglemodel(context);
        }

        public override void ExitObjFull([NotNull] ObjFullContext context)
        {
            List<Models.IModelObject> obj = new List<Models.IModelObject>();
            List<string> orderby = new List<string>();

            if (context.orderstatement() != null)
            {
                foreach (Antlr4.Runtime.IToken item in context.orderstatement()._orderby)
                {
                    orderby.Add(item.Text);
                }
            }
            foreach (PairContext item in context._pairs)
            {
                Models.IModelObject p = PairProps.Get(item);
                obj.Add(p);
            }
            ObjProps.Put(context, obj);
            ObjOrderByProps.Put(context, orderby);
            base.ExitObjFull(context);
        }

        public override void ExitObjEmpty([NotNull] ObjEmptyContext context)
        {
            List<Models.IModelObject> obj = new List<Models.IModelObject>();
            List<string> orderby = new List<string>();

            if (context.orderstatement() != null)
            {
                foreach (Antlr4.Runtime.IToken item in context.orderstatement()._orderby)
                {
                    orderby.Add(item.Text);
                }
            }
            ObjOrderByProps.Put(context, orderby);
            ObjProps.Put(context, obj);
            base.ExitObjEmpty(context);
        }

        public override void ExitValueString([NotNull] ValueStringContext context)
        {
            base.ExitValueString(context);
            Models.ModelStringObject newObj = new Models.ModelStringObject();

            string val = context.STRING().GetText();
            val = val.Substring(1, val.Length - 2);

            newObj.Value = val;
            ValueProps.Put(context, newObj);
        }

        public override void ExitValueNumber([NotNull] ValueNumberContext context)
        {
            base.ExitValueNumber(context);
            Models.ModelNumberObject newObj = new Models.ModelNumberObject
            {
                Value = decimal.Parse(context.NUMBER().GetText())
            };
            ValueProps.Put(context, newObj);
        }

        public override void ExitValueObject([NotNull] ValueObjectContext context)
        {
            base.ExitValueObject(context);
            Models.ModelCompositeObject newObj = new Models.ModelCompositeObject
            {
                Value = ObjProps.Get(context.obj())
            };
            List<string> orderby = ObjOrderByProps.Get(context.obj());
            newObj.OrderBy.AddRange(orderby);

            ValueProps.Put(context, newObj);
        }

        public override void ExitValueFalse([NotNull] ValueFalseContext context)
        {
            base.ExitValueFalse(context);
            Models.ModelBoolObject newObj = new Models.ModelBoolObject
            {
                Value = false
            };
            ValueProps.Put(context, newObj);
        }

        public override void ExitValueTrue([NotNull] ValueTrueContext context)
        {
            base.ExitValueTrue(context);
            Models.ModelBoolObject newObj = new Models.ModelBoolObject
            {
                Value = true
            };
            ValueProps.Put(context, newObj);
        }

        private string RemoveLineEnd(string inString)
        {
            string outString;

            outString = inString.Replace(" ", "");
            outString = outString.Replace("\n", "");
            outString = outString.Replace("\r", "");
            return outString;
        }
        public override void ExitIncludeStatement([NotNull] IncludeStatementContext context)
        {
            base.ExitIncludeStatement(context);

            ModelParser mp = new ModelParser();

            string fName = RemoveLineEnd(context.GetText().Replace('&', ' '));

            logger.Debug("Include '{fn}'", fName);

            mp.ParseInputFile(fName, true);

            foreach (Models.ParameterModel item in mp.ModelCollector.ParameterModels)
            {
                Models.ParameterModel exists = Collector.ParameterModels.FirstOrDefault(m => m.Name == item.Name);
                if (exists != null)
                {
                    exists.Value.AddRange(item.Value);
                }
                else
                {
                    Collector.ParameterModels.Add(item);
                }
            }
            foreach (Models.SingleModel item in mp.ModelCollector.SingleModels)
            {
                Models.SingleModel exists = Collector.SingleModels.FirstOrDefault(m => m.Name == item.Name);
                if (exists != null)
                {
                    exists.Value.AddRange(item.Value);
                }
                else
                {
                    Collector.SingleModels.Add(item);
                }
            }
            // get GraphElements from included 
            foreach (KeyValuePair<string, GraphElement> item in mp.GraphElements)
            {
                if (!GraphElements.ContainsKey(item.Key))
                {
                    GraphElements.Add(item.Key, item.Value);
                }
            }
        }

    }


}
