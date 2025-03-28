﻿using Microsoft.Extensions.Logging;

namespace Packt.Shared {
    public class ConsoleLoggerProvider : ILoggerProvider {
        public ILogger CreateLogger(string categoryName) {
            return new ConsoleLogger();
        }

        public void Dispose() {
        }
    }
    public class ConsoleLogger : ILogger {
        public IDisposable? BeginScope<TState>(TState state) {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel) {
            switch (logLevel) {
                case LogLevel.Trace:
                case LogLevel.Information:
                case LogLevel.None:
                    return false;
                case LogLevel.Debug:
                case LogLevel.Warning:
                case LogLevel.Error:
                case LogLevel.Critical:
                default:
                    return true;

            };
        }

        public void Log<TState>(LogLevel logLevel,
            EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) {
            if (eventId.Id == 20100) {
                Console.Write($"Level: {logLevel}, Event: {eventId.Name}");
                if (state != null) {
                    Console.Write($", State: {state}");
                }
                if (exception != null) {
                    Console.Write($", Exception: {exception.Message}.");
                }
                Console.WriteLine();
            }
        }
    }
}
