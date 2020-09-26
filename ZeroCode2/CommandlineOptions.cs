using CommandLine;

namespace ZeroCode2
{
    public class CommandlineOptions
    {
        [Option(HelpText = "File with the input parameters for the ZeroCode generator", Required = true)]
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


}
