using System.Collections.Generic;

namespace ZeroCode2
{
    public class TemplateParser
    {
        public Interpreter.InterpreterProgram Program { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
        public bool HasErrors { get; set; }

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

            var walker = new Antlr4.Runtime.Tree.ParseTreeWalker();

            var Listener = new ZeroCode2TemplateListener
            {
                Program = Program
            };

            walker.Walk(Listener, parser.template());

            Errors.AddRange(Program.Errors());
            HasErrors = Errors.Count > 0;

        }

    }


}
