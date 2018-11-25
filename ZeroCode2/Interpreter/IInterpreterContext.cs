namespace ZeroCode2.Interpreter
{
    public interface IInterpreterContext
    {
        Emitter.IEmitter Emitter { get; set; }
        ModelCollector Model { get; set; }
        string EvaluateProperty(string expression);
        void SetResult(string result);
        void EmitResult();
        IteratorManager EvaluateLoop(string expression);
        void EnterLoop(string expression);
        void ExitLoop(string expression);
        bool EvaluteCondition(string expression);
    }
}