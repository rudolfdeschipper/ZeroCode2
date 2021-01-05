using System.Collections.Generic;
using ZeroCode2.Models.Graph;

namespace ZeroCode2
{
    public class ModelParser
    {
        readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ModelCollector ModelCollector { get; set; }

        public Dictionary<string, GraphElement> GraphElements { get; set; }
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

            GraphElements = Listener.GraphElements;

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
            bool ok;
            // from the graph elements, build the inheritance graph
            var graphBuilder = new InheritanceGraphBuilder
            {
                Elements = GraphElements
            };
            ok = graphBuilder.BuildGraph();
            if (!ok)
            {
                foreach (var item in GraphElements)
                {
                    if (item.Value.State != GraphElementSate.Processed)
                    {
                        Errors.Add(string.Format("Element {0} was not resolved", item.Key));
                    }
                }
            }
            return Errors.Count == 0;
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

    }


}
