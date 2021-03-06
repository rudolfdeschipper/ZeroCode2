﻿namespace ZeroCode2.Interpreter
{
    public interface IInterpreterContext
    {
        Emitter.IEmitter Emitter { get; set; }
        ModelCollector Model { get; set; }
        bool PropertyExists(string expression);
        string EvaluateProperty(string expression);
        void EmitResult(string Result);
        IteratorManager EvaluateLoop(string expression);
        void EnterLoop(string expression);
        void ExitLoop(string expression);
    }
}