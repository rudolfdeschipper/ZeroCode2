﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Emitter
{
    public interface IEmitter
    {
        string OutputPath { get; set; }
        IFilePath FilePath { get; set; }
        void Open(string uri);
        void Emit(string output);
        bool Exists(string fileName);
        void Close();
    }
}
