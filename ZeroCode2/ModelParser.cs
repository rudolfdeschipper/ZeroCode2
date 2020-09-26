using System.Collections.Generic;
using ZeroCode2.Models;

namespace ZeroCode2
{
    public class ModelParser
    {
        readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ModelCollector ModelCollector { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
        public bool HasErrors { get; set; }

        public void ParseInputFile(string inFile)
        {
            var fIn = System.IO.File.OpenText(inFile);

            ParseInputFile(fIn);

            fIn.Close();
        }



        public void ParseInputFile(System.IO.StreamReader fIn)
        {
            var input = new Antlr4.Runtime.AntlrInputStream(fIn);
            var lexer = new Grammars.ZeroCode2Lexer(input);
            var tokenStream = new Antlr4.Runtime.CommonTokenStream(lexer);
            var parser = new Grammars.ZeroCode2(tokenStream);


            var walker = new Antlr4.Runtime.Tree.ParseTreeWalker();

            var Listener = new ZeroCodeListener();

            walker.Walk(Listener, parser.zcDefinition());

            ModelCollector = Listener.Collector;

            if (ResolveInheritance() == false)
            {
                logger.Error("Not all Inheritances were resolved");
                HasErrors = true;
            }

            if (parser.NumberOfSyntaxErrors > 0)
            {
                HasErrors = true;
                logger.Error("Errors exist in the input file");
            }
            // dump all errors:
            Errors.ForEach(e => logger.Error(e));
        }

        private bool ResolveInheritance()
        {
            var resolver = new Models.InheritanceResolver();
            var ok = true;

            ModelCollector.SingleModels.ForEach(m => ok &= resolver.ResolveInheritance(m, ModelCollector));
            this.Errors.AddRange(resolver.Errors);

            ModelCollector.SingleModels.ForEach(m => m.Resolve());

            return ok;
        }

        public void DumpErrors()
        {
            if (HasErrors)
            {
                foreach (var item in Errors)
                {
                    logger.Error(item);
                }
            }
        }

        public void RunTests()
        {
            // TODO: move to unit tests
            if (!HasErrors)
            {
                var it = new Models.Iterator();
                do
                {
                    var itlocator = new Models.PropertyLocator("@Models", ModelCollector, null);
                    itlocator.Locate();
                    var c = it.Iterate(itlocator.LocatedProperty());
                    logger.Info("Model: " + c.Name);

                    var it2 = new Models.Iterator();
                    do
                    {
                        var c2 = it2.Iterate(c);
                        logger.Info("\t" + c2.Name);

                        var it3 = new Models.Iterator();
                        do
                        {
                            var c3 = it3.Iterate(c2);
                            logger.Info("\t\t" + c3.Name + " " + c3.GetText());

                        } while (it3.HasMore);
                    } while (it2.HasMore);

                } while (it.HasMore);

                // test absolute locator:
                var locator = new Models.PropertyLocator("#Parameters.appName", ModelCollector, null);
                locator.Locate();
                _ = locator.LocatedProperty();
                //logger.Info("Located: {0} = {1}", el.Name, el.Value.GetText());
                //el = locator.Locate("@Models.Person.ID.Name", modelCollector);
                //logger.Info("Located: {0} = {1}", el.Name, el.Value.GetText());

                //el = locator.Locate("@Models.Stakeholder.Name.Title", modelCollector);
                //logger.Info("Located: {0} = {1}", el.Name, el.Value.GetText());

                //el = locator.Locate("@DataDictionary.IDField.Name", modelCollector);
                //logger.Info("Located: {0} = {1}", el.Name, el.Value.GetText());

                //el = locator.Locate("@Models.Stakeholder.Name.Nullable", modelCollector);
                //logger.Info("Not found: {0}", el?.Name ?? "Nullable");

                // locator from a specific point in the tree (iterator):

                locator = new Models.PropertyLocator("@ViewModels", ModelCollector, null);
                it = new Models.Iterator();
                locator.Locate();
                var c4 = it.Iterate(locator.LocatedProperty());

                var loopStack = new Stack<Interpreter.IteratorManager>();
                loopStack.Push(new Interpreter.IteratorManager(it, c4) { Path = "@ViewModels" });
                locator = new Models.PropertyLocator("Name.Length", ModelCollector, loopStack);
                locator.Locate();
                IModelObject el = locator.LocatedProperty();
                logger.Info("Located: {0} = {1}", el.Name, el.GetText());

                locator = new Models.PropertyLocator("Test.Title", ModelCollector, loopStack);
                locator.Locate();
                el = locator.LocatedProperty();
                logger.Info("Located: {0} = {1}", el.Name, el.GetText());

                locator = new Models.PropertyLocator("Title.SomeOtherProperty.Title", ModelCollector, loopStack);
                locator.Locate();
                el = locator.LocatedProperty();
                logger.Info("Located: {0} = {1}", el.Name, el.GetText());

                locator = new Models.PropertyLocator("Title.SomeOtherProperty.Name", ModelCollector, loopStack);
                locator.Locate();
                el = locator.LocatedProperty();
                logger.Info("Located: {0} = {1}", el.Name, el.GetText());
                locator = new Models.PropertyLocator("$", ModelCollector, loopStack);
                locator.Locate();
                el = locator.LocatedProperty();
                logger.Info("Located: $ = {0}", el.Name);
            }
            else
            {
                logger.Error("Errors were found: Cannot run tests");
            }
        }
    }


}
