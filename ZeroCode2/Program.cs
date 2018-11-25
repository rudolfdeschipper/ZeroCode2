using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using ZeroCode2.Grammars;
using CommandLine;

namespace ZeroCode2
{
    public class CommandlineOptions
    {
        [Option(HelpText ="File with the input parameters for the ZeroCode generator", Required = true)]
        public string InputFile { get; set; }
        [Option(HelpText = "File with the master template for the ZeroCode generator", Required = true)]
        public string Template { get; set; }
        [Option(HelpText = "Path where the generated code will be stored", Required = false, Default = ".")]
        public string OutputPath { get; set; }
        [Option(HelpText = "Run the generator but do not create any output", Required = false, Default = false)]
        public bool Simulate { get; set; }
        [Option(HelpText = "Logging level: 0 = Trace, 1 = Debug, 2 = Info, 3 = Warn, 4 = Error, 5 = Fatal", Required = false, Default = 2)]
        public int LogLevel { get; set; }
        /*
         * Trace
         * Debug
         * Info;
         * Warn;
         * Error
         * Fatal
         */
    }

    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            CommandlineOptions cmdOptions = null;

            var pRes = CommandLine.Parser.Default.ParseArguments<CommandlineOptions>(args)
                .WithParsed<CommandlineOptions>(o => cmdOptions = o);

            if (cmdOptions != null)
            {
                if (cmdOptions.LogLevel < 0 ||cmdOptions.LogLevel > 5)
                {
                    Console.Error.WriteLine("LogLevel must be between 0 and 5");
                }
                else
                {
                    try
                    {
                        NLog.LogManager.GlobalThreshold = NLog.LogLevel.FromOrdinal(cmdOptions.LogLevel);

                        logger.Info("START");
                        var r = new Program();
                        ModelParser modelParser;
                        TemplateParser templateParser;
                        Interpreter.Emitter.IEmitter emitter;

                        logger.Info("Parsing model:");
#if DEBUG
                        modelParser = r.RunModelParser(cmdOptions, true);
#else
                        modelParser = r.RunModelParser(false);
#endif
                        logger.Info("Parsing Template:");
                        templateParser = r.RunTemplateParser(cmdOptions);

                        if (!cmdOptions.Simulate)
                        {
                            emitter = new Interpreter.Emitter.FileEmitter();
                        }
                        else
                        {
                            emitter = new Interpreter.Emitter.NullEmitter();
                        }

                        ModelExecutor executor = new ModelExecutor();

                        executor.ExecuteProgram(modelParser, templateParser, emitter);

                        if (executor.HasErrors)
                        {
                            logger.Error("Runtime errors");
                            foreach (var item in executor.Errors)
                            {
                                logger.Error(item);
                            }
                        }

                        Console.Write("DONE. Hit RETURN to exit: ");
                    }
                    catch (Exception ex)
                    {
                        logger.Fatal(ex, "ERROR:");
                        Console.Write("Hit RETURN to exit: ");
                    }
                }
            }

            Console.ReadLine();
        }

        private ModelParser RunModelParser(CommandlineOptions options, bool test)
        {
            var fIn = System.IO.File.OpenText(options.InputFile);

            var runner = new ModelParser();

            runner.ParseInputFile(fIn);

            runner.DumpErrors();

            if (test)
            {
                runner.RunTests();
            }

            fIn.Close();
            return runner;
        }

        private TemplateParser RunTemplateParser(CommandlineOptions options)
        {
            var parser = new TemplateParser(new Interpreter.InterpreterProgram(new List<Interpreter.InterpreterInstructionBase>()));

            parser.ParseTemplateFile(options.Template);

            return parser;
        }

    }

    public class ModelParser
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ModelCollector modelCollector { get; set; }

        public List<string> Errors { get; set; }
        public bool HasErrors { get; set; }

        public void ParstInputFile(string inFile)
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
            var parser = new Grammars.ZeroCode2Parser(tokenStream);


            var walker = new Antlr4.Runtime.Tree.ParseTreeWalker();

            var Listener = new ZeroCodeListener();

            walker.Walk(Listener, parser.zcDefinition());

            modelCollector = Listener.collector;

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

        }

        private bool ResolveInheritance()
        {
            var resolver = new Models.InheritanceResolver();
            var ok = true;

            modelCollector.SingleModels.ForEach(m => ok &= resolver.ResolveInheritance(m, modelCollector.SingleModels));

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
            if (!HasErrors)
            {
                var it = new Models.Iterator();
                do
                {
                    var c = it.IterateSection(modelCollector.SingleModels, "@Models");
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
                            logger.Info("\t\t" + c3.Name + " " + c3.Value.GetText());

                        } while (it3.HasMore);
                    } while (it2.HasMore);

                } while (it.HasMore);

                // test absolute locator:
                var locator = new Models.PropertyLocator();
                var el = locator.Locate("#Parameters.appName", modelCollector);
                logger.Info("Located: {0} = {1}", el.Name, el.Value.GetText());
                el = locator.Locate("@Models.Person.ID.Name", modelCollector);
                logger.Info("Located: {0} = {1}", el.Name, el.Value.GetText());

                el = locator.Locate("@Models.Stakeholder.Name.Title", modelCollector);
                logger.Info("Located: {0} = {1}", el.Name, el.Value.GetText());

                el = locator.Locate("@Models.Stakeholder.Name.Nullable", modelCollector);
                logger.Info("Not found: {0}", el?.Name ?? "Nullable");

                // locator from a specific point in the tree (iterator):
                it = new Models.Iterator();
                var c4 = it.IterateSection(modelCollector.SingleModels, "@ViewModels");
                el = locator.Locate("Name.Length", c4);
                logger.Info("Located: {0} = {1}", el.Name, el.Value.GetText());
                el = locator.Locate("Test.Title", c4);
                logger.Info("Located: {0} = {1}", el.Name, el.Value.GetText());
                el = locator.Locate("Title.SomeOtherProperty.Title", c4);
                logger.Info("Located: {0} = {1}", el.Name, el.Value.GetText());
                el = locator.Locate("Title.SomeOtherProperty.Name", c4);
                logger.Info("Located: {0} = {1}", el.Name, el.Value.GetText());
            }
            else
            {
                logger.Error("Errors were found: Cannot run tests");
            }
        }
    }

    public class TemplateParser
    {
        public Interpreter.InterpreterProgram Program { get; set; }

        public TemplateParser(Interpreter.InterpreterProgram program)
        {
            Program = program;
        }

        public void ParseTemplateFile(string inFile)
        {
            var fIn = System.IO.File.OpenText(inFile);

            ParseTemplateFile(fIn);

            fIn.Close();
        }

        public void ParseTemplateFile(System.IO.StreamReader fIn)
        {
            var input = new Antlr4.Runtime.AntlrInputStream(fIn);
            var lexer = new Grammars.ZeroCode2TemplateLexer(input);
            var tokenStream = new Antlr4.Runtime.CommonTokenStream(lexer);
            var parser = new Grammars.ZeroCode2Template(tokenStream);

            //var walker = new Antlr4.Runtime.Tree.ParseTreeWalker();
            var walker = new Antlr4.Runtime.Tree.ParseTreeWalker();

            var Listener = new ZeroCode2TemplateListener();
            Listener.Program = Program;

            walker.Walk(Listener, parser.template());

        }

    }

    public class ModelCollector
    {
        public List<Models.ParameterModel> ParameterModels { get; set; }
        public List<Models.SingleModel> SingleModels { get; set; }

        public ModelCollector()
        {
            ParameterModels = new List<Models.ParameterModel>();
            SingleModels = new List<Models.SingleModel>();
        }
    }

    public class ModelExecutor
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public List<string> Errors { get; set; } = new List<string>();
        public bool HasErrors { get; set; } = false;

        public void ExecuteProgram(ModelParser modelParser, TemplateParser templateParser, Interpreter.Emitter.IEmitter emitter)
        {
            var instructions = templateParser.Program.Instructions;
            var context = new Interpreter.InterpreterContext();

            context.Model = modelParser.modelCollector;
            context.Emitter = emitter;

            var PC = instructions[0];
            Interpreter.InterpreterInstructionBase next = null;

            while (PC != null )
            {
                try
                {
                    logger.Debug("Executing {0} on Line {1} Pos {2}", PC.Instruction, PC.Line, PC.Position);

                    next = PC.Execute(context);

                    context.EmitResult();

                }
                catch (Exception e)
                {
                    Errors.Add(string.Format("Error during execution: {0} Line {1}, Pos {2}: {3}", e.Message, PC.Line, PC.Position, PC.Instruction));
                    HasErrors = true;
                    //break;
                }

                PC = next;
            }

        }
    }

    public class ZeroCodeListener : ZeroCode2.Grammars.ZeroCode2BaseListener
    {
        // Logging
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ModelCollector collector { get; set; } = new ModelCollector();

        private string CurrentSection = "";

        public Antlr4.Runtime.Tree.ParseTreeProperty<Models.IObjectBase> ValueProps { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<Models.IObjectBase>();
        public Antlr4.Runtime.Tree.ParseTreeProperty<Models.ModelPair> PairProps { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<Models.ModelPair>();
        public Antlr4.Runtime.Tree.ParseTreeProperty<List<Models.ModelPair>> ObjProps { get; set; } = new Antlr4.Runtime.Tree.ParseTreeProperty<List<Models.ModelPair>>();

        public override void EnterParameters([NotNull] ZeroCode2Parser.ParametersContext context)
        {
            CurrentSection = context.ID().GetText();

            logger.Trace(" Enter Params={0}", CurrentSection);

            base.EnterParameters(context);
        }

        public override void ExitParameters([NotNull] ZeroCode2Parser.ParametersContext context)
        {
            logger.Trace(" Exit Params={0}", context._pairs.Aggregate("", (a, r) => a += r.ID().GetText() + " = " + r.value().GetText() + "\n"));

            var parameter = new Models.ParameterModel(context.ID().GetText());
            parameter.Section = CurrentSection;
            foreach (var item in context._pairs)
            {
                var p = PairProps.Get(item);
                parameter.Properties.Add(p);
            }
            collector.ParameterModels.Add(parameter);

            base.ExitParameters(context);
        }

        public override void EnterGenericModel([NotNull] ZeroCode2Parser.GenericModelContext context)
        {
            CurrentSection = context.ID().GetText();
        }

        public override void ExitPair([NotNull] ZeroCode2Parser.PairContext context)
        {
            Models.ModelPair pair = new Models.ModelPair(context.ID().GetText(), ValueProps.Get(context.value()));
            PairProps.Put(context, pair);
            if (context.inherits() != null)
            {
                pair.Inherits = true;
                pair.InheritsFrom = context.inherits().ID().GetText();
            }
            if (context.modifier != null)
            {
                pair.Modified = true;
                pair.Modifier = context.modifier.Text;
            }
            base.ExitPair(context);
        }

        public override void ExitSinglemodel([NotNull] ZeroCode2Parser.SinglemodelContext context)
        {
            var obj = ObjProps.Get(context.obj());
            var model = new Models.SingleModel(context.ID().GetText(), obj);
            model.Section = CurrentSection;
            if (context.inherits() != null)
            {
                model.Inherits = true;
                model.InheritsFrom = context.inherits().ID().GetText();
            }

            collector.SingleModels.Add(model);
            base.ExitSinglemodel(context);
        }

        public override void ExitObjFull([NotNull] ZeroCode2Parser.ObjFullContext context)
        {
            var obj = new List<Models.ModelPair>();
            foreach (var item in context._pairs)
            {
                var p = PairProps.Get(item);
                obj.Add(p);
            }
            ObjProps.Put(context, obj);
            base.ExitObjFull(context);
        }

        public override void ExitObjEmpty([NotNull] ZeroCode2Parser.ObjEmptyContext context)
        {
            var obj = new List<Models.ModelPair>();
            ObjProps.Put(context, obj);
            base.ExitObjEmpty(context);
        }

        public override void ExitValueString([NotNull] ZeroCode2Parser.ValueStringContext context)
        {
            base.ExitValueString(context);
            var newObj = new Models.StringObject();

            string val = context.STRING().GetText();
            val = val.Substring(1, val.Length - 2);

            newObj.Value = val;
            ValueProps.Put(context, newObj);
        }

        public override void ExitValueNumber([NotNull] ZeroCode2Parser.ValueNumberContext context)
        {
            base.ExitValueNumber(context);
            var newObj = new Models.NumberObject();
            newObj.Value = double.Parse(context.NUMBER().GetText());
            ValueProps.Put(context, newObj);
        }

        public override void ExitValueObject([NotNull] ZeroCode2Parser.ValueObjectContext context)
        {
            base.ExitValueObject(context);
            var newObj = new Models.ObjectObject();
            newObj.Value = ObjProps.Get(context.obj());
            ValueProps.Put(context, newObj);
        }

        public override void ExitValueFalse([NotNull] ZeroCode2Parser.ValueFalseContext context)
        {
            base.ExitValueFalse(context);
            var newObj = new Models.BoolObject();
            newObj.Value = false;
            ValueProps.Put(context, newObj);
        }

        public override void ExitValueTrue([NotNull] ZeroCode2Parser.ValueTrueContext context)
        {
            base.ExitValueTrue(context);
            var newObj = new Models.BoolObject();
            newObj.Value = true;
            ValueProps.Put(context, newObj);
        }

    }

    class ZeroCode2TemplateListener : ZeroCode2.Grammars.ZeroCode2TemplateBaseListener
    {
        // Logging
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public Interpreter.InterpreterProgram Program { get; set; }

        public override void ExitLiteralCommand([NotNull] ZeroCode2Template.LiteralCommandContext context)
        {
            Program.AddLiteral(context.start.Line, context.start.StartIndex, context.GetText());

            base.ExitLiteralCommand(context);
        }

        public override void ExitFileCreateCommand([NotNull] ZeroCode2Template.FileCreateCommandContext context)
        {
            string instrVal = context.IGNORE().Aggregate("", (s, t) => s += t.GetText());

            Program.AddFileCreate(context.start.Line, context.start.StartIndex, instrVal);

            base.ExitFileCreateCommand(context);
        }

        public override void ExitFileOverwriteCommand([NotNull] ZeroCode2Template.FileOverwriteCommandContext context)
        {
            string instrVal = context.IGNORE().Aggregate("", (s, t) => s += t.GetText());

            Program.AddFileOverwrite(context.start.Line, context.start.StartIndex, instrVal);

            base.ExitFileOverwriteCommand(context);
        }

        public override void ExitLoopCommand([NotNull] ZeroCode2Template.LoopCommandContext context)
        {

            Program.AddLoop(context.start.Line, context.start.StartIndex, context.IGNORE().GetText());

            base.ExitLoopCommand(context);
        }

        public override void ExitEndFileCommand([NotNull] ZeroCode2Template.EndFileCommandContext context)
        {

            Program.AddEndFile(context.start.Line, context.start.StartIndex, context.GetText());

            base.ExitEndFileCommand(context);
        }

        public override void ExitEndLoopCommand([NotNull] ZeroCode2Template.EndLoopCommandContext context)
        {
            Program.AddEndLoop(context.start.Line, context.start.StartIndex, context.GetText());

            base.ExitEndLoopCommand(context);
        }

        public override void ExitIncludeCommand([NotNull] ZeroCode2Template.IncludeCommandContext context)
        {
            string instrVal = context.IGNORE().Aggregate("", (s, t) => s += t.GetText());

            logger.Debug("Including {0}", instrVal);

            var newParser = new TemplateParser(Program);

            newParser.ParseTemplateFile(instrVal);

            logger.Debug("End Including {0}", instrVal);

            base.ExitIncludeCommand(context);
        }

        public override void ExitExprCommand([NotNull] ZeroCode2Template.ExprCommandContext context)
        {
            Program.AddExpression(context.start.Line, context.start.StartIndex, context.EXIGNORE().GetText());

            base.ExitExprCommand(context);
        }

        public override void ExitIfCommand([NotNull] ZeroCode2Template.IfCommandContext context)
        {
            Program.AddIf(context.start.Line, context.start.StartIndex, context.IFTEXT().GetText());

            base.ExitIfCommand(context);
        }

        public override void ExitElseCommand([NotNull] ZeroCode2Template.ElseCommandContext context)
        {
            Program.AddElse(context.start.Line, context.start.StartIndex, context.GetText());

            base.ExitElseCommand(context);
        }

        public override void ExitEndIfCommand([NotNull] ZeroCode2Template.EndIfCommandContext context)
        {
            Program.AddEndif(context.start.Line, context.start.StartIndex, context.GetText());

            base.ExitEndIfCommand(context);
        }

    }

}
