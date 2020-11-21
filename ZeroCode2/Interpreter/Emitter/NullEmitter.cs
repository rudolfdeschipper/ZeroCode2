namespace ZeroCode2.Interpreter.Emitter
{
    class NullEmitter : FileEmitter
    {
        public new IFilePath FilePath { get; set; } = new NullFilePath();
    }
}
