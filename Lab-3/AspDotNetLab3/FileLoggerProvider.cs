﻿namespace AspDotNetLab3
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _path;
        public FileLoggerProvider(string path)
        {
            _path = path;
        }
        public ILogger CreateLogger(string categoryName) => new FileLogger(_path);
        public void Dispose() { }
    }
}
