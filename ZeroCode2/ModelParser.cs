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

        public void ParseInputFile(string inFile, bool isInclude = false)
        {
            System.IO.StreamReader fIn = System.IO.File.OpenText(inFile);

            ParseInputFile(fIn, isInclude);

            fIn.Close();
        }



        public void ParseInputFile(System.IO.StreamReader fIn, bool isInclude = false)
        {
            Antlr4.Runtime.AntlrInputStream input = new Antlr4.Runtime.AntlrInputStream(fIn);
            Grammars.ZeroCode2Lexer lexer = new Grammars.ZeroCode2Lexer(input);
            Antlr4.Runtime.CommonTokenStream tokenStream = new Antlr4.Runtime.CommonTokenStream(lexer);
            Grammars.ZeroCode2 parser = new Grammars.ZeroCode2(tokenStream);


            Antlr4.Runtime.Tree.ParseTreeWalker walker = new Antlr4.Runtime.Tree.ParseTreeWalker();

            ZeroCodeListener Listener = new ZeroCodeListener();

            walker.Walk(Listener, parser.zcDefinition());

            ModelCollector = Listener.Collector;

            GraphElements = Listener.GraphElements;

            if (!isInclude)
            {
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
        }

        private bool ResolveInheritance()
        {
            bool ok;
            // from the graph elements, build the inheritance graph
            InheritanceGraphBuilder graphBuilder = new InheritanceGraphBuilder
            {
                Elements = GraphElements
            };
            ok = graphBuilder.BuildGraph();
            if (!ok)
            {
                foreach (KeyValuePair<string, GraphElement> item in GraphElements)
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
                foreach (string item in Errors)
                {
                    logger.Error(item);
                }
            }
        }

    }


}
