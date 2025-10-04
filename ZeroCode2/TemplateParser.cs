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
            System.IO.StreamReader fIn = System.IO.File.OpenText(inFile);

            ParseTemplateFile(fIn);

            fIn.Close();
        }

        public void ParseTemplateFile(System.IO.StreamReader fIn)
        {
            Antlr4.Runtime.AntlrInputStream input = new Antlr4.Runtime.AntlrInputStream(fIn);
            Grammars.ZeroCode2TemplateLexer lexer = new Grammars.ZeroCode2TemplateLexer(input);
            Antlr4.Runtime.CommonTokenStream tokenStream = new Antlr4.Runtime.CommonTokenStream(lexer);
            Grammars.ZeroCode2Template parser = new Grammars.ZeroCode2Template(tokenStream);

            Antlr4.Runtime.Tree.ParseTreeWalker walker = new Antlr4.Runtime.Tree.ParseTreeWalker();

            ZeroCode2TemplateListener Listener = new ZeroCode2TemplateListener
            {
                Program = Program
            };

            walker.Walk(Listener, parser.template());

            Errors.AddRange(Program.Errors());
            HasErrors = Errors.Count > 0;

        }

    }


}
