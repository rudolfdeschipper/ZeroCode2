using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using ZeroCode2.Grammars;
using static ZeroCode2.Grammars.ZeroCode2;

namespace ZeroCode2
{
    public class ZeroCodeListener : ZeroCode2BaseListener
    {
        // Logging
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ModelCollector Collector { get; set; } = new ModelCollector();

        public Antlr4.Runtime.Tree.ParseTreeProperty<Models.IModelObject> ValueProps { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<Models.IModelObject>();
        public Antlr4.Runtime.Tree.ParseTreeProperty<Models.IModelObject> PairProps { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<Models.IModelObject>();
        public Antlr4.Runtime.Tree.ParseTreeProperty<List<Models.IModelObject>> ObjProps { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<List<Models.IModelObject>>();
        public Antlr4.Runtime.Tree.ParseTreeProperty<List<string>> ObjOrderByProps { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<List<string>>();
        public Antlr4.Runtime.Tree.ParseTreeProperty<Models.SingleModel> SingleModels { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<Models.SingleModel>();

        public override void EnterParameters([NotNull] ParametersContext context)
        {
            var CurrentSection = context.ID().GetText();

            logger.Trace(" Enter Params={0}", CurrentSection);

            base.EnterParameters(context);
        }

        public override void ExitParameters([NotNull] ParametersContext context)
        {
            logger.Trace(" Exit Params={0}", context._pairs.Aggregate("", (a, r) => a += r.ID().GetText() + " = " + r.pairvalue().GetText() + "\n"));

            var parameter = new Models.ParameterModel("#" + context.ID().GetText());
            //parameter.Section = CurrentSection;
            foreach (var item in context._pairs)
            {
                var p = PairProps.Get(item);
                parameter.Value.Add(p);
            }
            // check for doubles
            var existing = Collector.ParameterModels.FirstOrDefault(p => p.Name == parameter.Name);
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
            var CurrentSection = "@" + context.ID().GetText();
            logger.Trace("Enter Section = {0}", CurrentSection);
        }

        public override void ExitGenericModel([NotNull] GenericModelContext context)
        {
            var list = new List<Models.IModelObject>();

            foreach (var item in context._smodels)
            {
                var sm = SingleModels.Get(item);
                list.Add(sm);
            }

            var TopLevelModel = new Models.SingleModel("@" + context.ID().GetText(), list);

            // check for doubles
            var existing = Collector.SingleModels.FirstOrDefault(p => p.Name == TopLevelModel.Name);
            if (existing != null)
            {
                existing.Value.AddRange(TopLevelModel.Value);
            }
            else
            {
                Collector.SingleModels.Add(TopLevelModel);
            }

            base.ExitGenericModel(context);
        }

        public override void ExitPair([NotNull] PairContext context)
        {
            Models.IModelObject pair = null;
            if (context.pairvalue()?.value() != null)
            {
                pair = ValueProps.Get(context.pairvalue().value());
            }
            if (pair == null)
            {
                var value = new List<Models.IModelObject>();
                pair = new Models.ModelCompositeObject
                {
                    Value = value
                };
            }
            pair.Name = context.ID().GetText();

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

            logger.Trace("Pair: {0}{1} = {2}{3}", pair.Modifier, pair.Name, pair.GetText() ?? "unknown", context.pairvalue()?.inherits() != null ? " : " + pair.InheritsFrom : "");

            base.ExitPair(context);
        }

        public override void ExitSinglemodel([NotNull] SinglemodelContext context)
        {
            var obj = ObjProps.Get(context.obj());

            var model = new Models.SingleModel(context.ID().GetText(), obj);

            var orderby = ObjOrderByProps.Get(context.obj());
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

            SingleModels.Put(context, model);
            base.ExitSinglemodel(context);
        }

        public override void ExitObjFull([NotNull] ObjFullContext context)
        {
            var obj = new List<Models.IModelObject>();
            var orderby = new List<string>();

            if (context.orderstatement() != null) {
                foreach (var item in context.orderstatement()._orderby)
                {
                    orderby.Add(item.Text);
                }
            }
            foreach (var item in context._pairs)
            {
                var p = PairProps.Get(item);
                obj.Add(p);
            }
            ObjProps.Put(context, obj);
            ObjOrderByProps.Put(context, orderby);
            base.ExitObjFull(context);
        }

        public override void ExitObjEmpty([NotNull] ObjEmptyContext context)
        {
            var obj = new List<Models.IModelObject>();
            var orderby = new List<string>();

            if (context.orderstatement() != null)
            {
                foreach (var item in context.orderstatement()._orderby)
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
            var newObj = new Models.ModelStringObject();

            string val = context.STRING().GetText();
            val = val.Substring(1, val.Length - 2);

            newObj.Value = val;
            ValueProps.Put(context, newObj);
        }

        public override void ExitValueNumber([NotNull] ValueNumberContext context)
        {
            base.ExitValueNumber(context);
            var newObj = new Models.ModelNumberObject
            {
                Value = decimal.Parse(context.NUMBER().GetText())
            };
            ValueProps.Put(context, newObj);
        }

        public override void ExitValueObject([NotNull] ValueObjectContext context)
        {
            base.ExitValueObject(context);
            var newObj = new Models.ModelCompositeObject
            {
                Value = ObjProps.Get(context.obj())
            };
            var orderby = ObjOrderByProps.Get(context.obj());
            newObj.OrderBy.AddRange(orderby);

            ValueProps.Put(context, newObj);
        }

        public override void ExitValueFalse([NotNull] ValueFalseContext context)
        {
            base.ExitValueFalse(context);
            var newObj = new Models.ModelBoolObject
            {
                Value = false
            };
            ValueProps.Put(context, newObj);
        }

        public override void ExitValueTrue([NotNull] ValueTrueContext context)
        {
            base.ExitValueTrue(context);
            var newObj = new Models.ModelBoolObject
            {
                Value = true
            };
            ValueProps.Put(context, newObj);
        }

        public override void ExitIncludeStatement([NotNull] IncludeStatementContext context)
        {
            base.ExitIncludeStatement(context);

            var mp = new ModelParser();

            mp.ParseInputFile(context.GetText().Substring(1));

            foreach (var item in mp.ModelCollector.ParameterModels)
            {
                var exists = Collector.ParameterModels.FirstOrDefault(m => m.Name == item.Name);
                if (exists != null)
                {
                    exists.Value.AddRange(item.Value);
                }
                else
                {
                    Collector.ParameterModels.Add(item);
                }
            }
            foreach (var item in mp.ModelCollector.SingleModels)
            {
                var exists = Collector.SingleModels.FirstOrDefault(m => m.Name == item.Name);
                if (exists != null)
                {
                    exists.Value.AddRange(item.Value);
                }
                else
                {
                    Collector.SingleModels.Add(item);
                }
            }

        }

    }


}
