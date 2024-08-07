﻿using CommandLine;
using System;
using System.Collections.Generic;

namespace ZeroCode2
{

    class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            CommandlineOptions cmdOptions = null;

            var pRes = CommandLine.Parser.Default.ParseArguments<CommandlineOptions>(args)
                .WithParsed<CommandlineOptions>(o => cmdOptions = o);

            if (cmdOptions != null)
            {
                if (cmdOptions.LogLevel < 0 || cmdOptions.LogLevel > 5)
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
                        modelParser = r.RunModelParser(cmdOptions);

                        if (modelParser.HasErrors)
                        {
                            logger.Error("Model errors");
                            foreach (var item in modelParser.Errors)
                            {
                                logger.Error(item);
                            }
                            throw new ApplicationException("Model errors");
                        }
                        logger.Info("Parsing Template:");
                        templateParser = r.RunTemplateParser(cmdOptions);
                        if (templateParser.HasErrors)
                        {
                            logger.Error("Template errors");
                            foreach (var item in templateParser.Errors)
                            {
                                logger.Error(item);
                            }
                            throw new ApplicationException("Template errors");
                        }

                        if (cmdOptions.Simulate == true)
                        {
                            emitter = new Interpreter.Emitter.NullEmitter();
                        }
                        else
                        {
                            emitter = new Interpreter.Emitter.FileEmitter();
                        }

                        emitter.OutputPath = cmdOptions.OutputPath;

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
                    }
                    catch (Exception ex)
                    {
                        logger.Fatal(ex, "ERROR: {0}", ex);
                    }
                }

            }
            if (cmdOptions is null || cmdOptions.NoWaitAtExit == false)
            {
                Console.Write("Hit RETURN to exit: ");
                Console.ReadLine();
            }
        }

        private ModelParser RunModelParser(CommandlineOptions options)
        {
            var fIn = System.IO.File.OpenText(options.InputFile);

            var runner = new ModelParser();

            runner.ParseInputFile(fIn);

            runner.DumpErrors();

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


}
