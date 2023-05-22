namespace AspDotNetLab3
{
    public class FileLogger : ILogger, IDisposable
    {
        private readonly string _filePath;
        private static object _lock = new();
        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => this;
        public void Dispose() { }
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            lock (_lock)
            {
                var a = state.ToString();
                File.AppendAllText(_filePath, formatter(state, exception) + Environment.NewLine);
            }
        }
    }
}
