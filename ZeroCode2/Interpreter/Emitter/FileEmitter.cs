﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCode2.Interpreter.Emitter
{
    public class FileEmitter : IEmitter
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private System.Text.StringBuilder _sb;
        private string _uri;

        public string OutputPath { get; set; }

        public IFilePath FilePath { get; set; } = new FilePath();

        public void EnsurePathExists()
        {

            if (!OutputPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                OutputPath += Path.DirectorySeparatorChar.ToString();
            }
            string fullPath = OutputPath + _uri;

            string[] pathParts = fullPath.Split(Path.DirectorySeparatorChar);

            // extract file
            string file = pathParts.Last();
            pathParts[pathParts.Length-1] = "";

            string currentPath = "";

            foreach (var item in pathParts.Where(s => s.Length > 0))
            {
                currentPath += item + Path.DirectorySeparatorChar.ToString();
                if (!FilePath.DirectoryExists(currentPath))
                {
                    FilePath.CreateDirectory(currentPath);
                }
            }
            OutputPath = currentPath;
            _uri = file;
        }

        public void Close()
        {
            logger.Info("Closing: " + _uri);

            // create path if not there yet
            EnsurePathExists();

            FilePath.WriteToFile(Path.Combine(OutputPath, _uri), _sb);
        }

        public void Emit(string output)
        {
            logger.Info("Emitting: " + output);
            if (_sb != null)
            {
                if (!string.IsNullOrWhiteSpace(output))
                {
                    logger.Info("Emitting: " + output);
                    _sb.Append(output);
                }
            }
            else
            {
                logger.Warn("Output: '" + output + "' was not emitted, no file open");
            }
        }

        public bool Exists(string fileName)
        {
            return FilePath.FileExists(fileName);
        }

        public void Open(string uri)
        {
            _uri = uri;

            logger.Info("Opening: " + _uri);

            _sb = new StringBuilder();
        }
    }
}
